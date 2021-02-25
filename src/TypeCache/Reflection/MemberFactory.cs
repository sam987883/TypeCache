// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Reflection
{
	public static class MemberFactory
	{
		public const BindingFlags INSTANCE_BINDINGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		public const BindingFlags STATIC_BINDINGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

		private static readonly IComparer<ParameterInfo> ParameterSortComparer = Comparer<ParameterInfo>.Create((x, y) => x.Position - y.Position)!;

		public static ConstructorMember CreateConstructorMember(ConstructorInfo constructorInfo)
			=> CreateConstructorMember(constructorInfo, MemberCache.Types[constructorInfo.DeclaringType!.TypeHandle]);

		private static ConstructorMember CreateConstructorMember(ConstructorInfo constructorInfo, TypeMember typeMember)
		{
			var parameterInfos = constructorInfo.GetParameters();
			parameterInfos.Sort(ParameterSortComparer);

			ParameterExpression parameters = nameof(parameters).Parameter<object?[]?>();
			var invoke = parameterInfos.Any()
				? constructorInfo.New(parameters.ToParameterArray(parameterInfos)).Lambda<StaticInvokeType>(parameters).Compile()
				: constructorInfo.New().Lambda<StaticInvokeType>(parameters).Compile();

			var methodParameters = parameterInfos.To(parameterInfo => parameterInfo.Parameter()).ToArray(parameterInfos.Length);
			var method = parameterInfos.Any()
				? constructorInfo.New(methodParameters).Lambda(methodParameters).Compile()
				: constructorInfo.New().Lambda().Compile();

			return new ConstructorMember
			{
				Attributes = constructorInfo.GetCustomAttributes<Attribute>(true).ToImmutableArray(),
				Handle = constructorInfo.MethodHandle,
				Invoke = invoke,
				IsInternal = constructorInfo.IsAssembly,
				IsPublic = constructorInfo.IsPublic,
				Method = method,
				Parameters = parameterInfos.To(MemberFactory.CreateParameter).ToImmutableArray(),
				Type = typeMember
			};
		}

		public static IImmutableList<ConstructorMember> CreateConstructorMembers(RuntimeTypeHandle typeHandle)
		{
			var typeMember = MemberCache.Types[typeHandle];
			return typeHandle.ToType().GetConstructors(INSTANCE_BINDINGS)
				.To(constructorInfo => CreateConstructorMember(constructorInfo, typeMember)).ToImmutable();
		}

		public static FieldMember CreateFieldMember(FieldInfo fieldInfo)
		{
			fieldInfo.IsLiteral.Assert($"{nameof(fieldInfo)}.{nameof(fieldInfo.IsLiteral)}", false);
			fieldInfo.IsStatic.Assert($"{nameof(fieldInfo)}.{nameof(fieldInfo.IsStatic)}", false);

			ParameterExpression instance = nameof(instance).Parameter(fieldInfo.DeclaringType!);
			var field = instance.Field(fieldInfo);
			var getter = field.Lambda(instance).Compile();
			var getValue = field.As<object>().Lambda<Func<object, object?>>(instance).Compile();

			Delegate? setter = null;
			Action<object, object?>? setValue = null;
			if (!fieldInfo.IsInitOnly)
			{
				ParameterExpression value = nameof(value).Parameter(fieldInfo.FieldType);
				setter = field.Assign(value).LambdaAction(instance, value).Compile();

				value = nameof(value).Parameter<object>();
				setValue = field.Assign(value.SystemConvert(fieldInfo.FieldType)).Lambda<Action<object, object?>>(instance, value).Compile();
			}

			return new FieldMember
			{
				Attributes = fieldInfo.GetCustomAttributes<Attribute>(true).ToImmutableArray(),
				Getter = getter,
				GetValue = getValue,
				Handle = fieldInfo.FieldHandle,
				IsInternal = fieldInfo.IsAssembly,
				Name = fieldInfo.GetName(),
				IsPublic = fieldInfo.IsPublic,
				Setter = setter,
				SetValue = setValue,
				Type = MemberCache.Types[fieldInfo.FieldType.TypeHandle]
			};
		}

		public static IImmutableDictionary<string, FieldMember> CreateFieldMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetFields(INSTANCE_BINDINGS)
				.If(fieldInfo => !fieldInfo!.IsLiteral)
				.To(fieldInfo => KeyValuePair.Create(fieldInfo!.Name, CreateFieldMember(fieldInfo)))
				.ToImmutable(StringComparer.Ordinal);

		public static IndexerMember CreateIndexerMember(PropertyInfo propertyInfo)
		{
			propertyInfo.GetIndexParameters().Any().Assert($"{nameof(propertyInfo)}.{nameof(propertyInfo.GetIndexParameters)}().Any()", true);

			var methodInfo = propertyInfo.GetMethod ?? propertyInfo.SetMethod;
			return new IndexerMember
			{
				Attributes = propertyInfo.GetCustomAttributes<Attribute>(true).ToImmutableArray(),
				GetMethod = propertyInfo.GetMethod != null ? CreateMethodMember(propertyInfo.GetMethod) : null,
				IsInternal = methodInfo!.IsAssembly,
				IsPublic = methodInfo.IsPublic,
				SetMethod = propertyInfo.SetMethod != null ? CreateMethodMember(propertyInfo.SetMethod) : null,
				Type = MemberCache.Types[propertyInfo.PropertyType.TypeHandle]
			};
		}

		public static IImmutableList<IndexerMember> CreateIndexerMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetProperties(INSTANCE_BINDINGS)
				.If(propertyInfo => propertyInfo!.GetIndexParameters().Any())
				.To(CreateIndexerMember!)
				.ToImmutable();

		public static MethodMember CreateMethodMember(MethodInfo methodInfo)
		{
			methodInfo.IsStatic.Assert($"{nameof(methodInfo)}.{nameof(methodInfo.IsStatic)}", false);

			var parameterInfos = methodInfo.GetParameters()
				.If(parameterInfo => parameterInfo != methodInfo.ReturnParameter)
				.Sort(ParameterSortComparer!);

			var callParameters = parameterInfos.To(parameterInfo => parameterInfo!.Parameter()).ToArray(parameterInfos.Length);
			var methodParameters = new ParameterExpression[parameterInfos.Length + 1];
			ParameterExpression instance = nameof(instance).Parameter(typeof(object));
			methodParameters[0] = instance;
			callParameters.CopyTo(methodParameters, 1);
			var instanceCast = instance.Cast(methodInfo.DeclaringType!);
			ParameterExpression arguments = nameof(arguments).Parameter<object?[]?>();
			var call = parameterInfos.Any()
				? instanceCast.Call(methodInfo, arguments.ToParameterArray(parameterInfos!))
				: instanceCast.Call(methodInfo);
			var method = parameterInfos.Any()
				? instanceCast.Call(methodInfo, callParameters).Lambda(methodParameters).Compile()
				: call.Lambda(instance).Compile();

			var parameters = parameterInfos.To(parameterInfo => CreateParameter(parameterInfo!)).ToImmutableArray();
			var invoke = methodInfo.ReturnType == typeof(void)
				? Expression.Block(call, Expression.Constant(null)).Lambda<InvokeType>(instance, arguments).Compile()
				: call.As<object>().Lambda<InvokeType>(instance, arguments).Compile();

			var returnType = MemberCache.Types[methodInfo.ReturnType.TypeHandle];

			return new MethodMember
			{
				Attributes = methodInfo.GetCustomAttributes<Attribute>(true).ToImmutableArray(),
				Handle = methodInfo.MethodHandle,
				Invoke = invoke,
				IsInternal = methodInfo.IsAssembly,
				IsPublic = methodInfo.IsPublic,
				Method = method,
				Parameters = parameters,
				Name = methodInfo.GetName(),
				Return = new ReturnParameter
				{
					Attributes = methodInfo.ReturnParameter.GetCustomAttributes<Attribute>(true).ToImmutableArray(),
					IsTask = returnType.SystemType == SystemType.Task,
					IsValueTask = returnType.SystemType == SystemType.ValueTask,
					IsVoid = returnType.SystemType == SystemType.Void,
					Type = returnType
				}
			};
		}

		public static IImmutableDictionary<string, IImmutableList<MethodMember>> CreateMethodMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetMethods(INSTANCE_BINDINGS)
				.If(methodInfo => !methodInfo!.ContainsGenericParameters && !methodInfo.IsSpecialName)
				.To(CreateMethodMember!)
				.Group(method => method!.Name, StringComparer.Ordinal)
				.ToImmutableDictionary(_ => _.Key, _ => _.Value.ToImmutable());

		public static PropertyMember CreatePropertyMember(PropertyInfo propertyInfo)
		{
			var methodInfo = propertyInfo.GetMethod ?? propertyInfo.SetMethod;
			methodInfo!.IsStatic.Assert(nameof(methodInfo.IsStatic), false);

			return new PropertyMember
			{
				Attributes = propertyInfo.GetCustomAttributes<Attribute>(true).ToImmutableArray(),
				Getter = propertyInfo.GetMethod != null ? CreateMethodMember(propertyInfo.GetMethod) : null,
				IsInternal = methodInfo.IsAssembly,
				Name = propertyInfo.GetName(),
				IsPublic = methodInfo.IsPublic,
				Setter = propertyInfo.SetMethod != null ? CreateMethodMember(propertyInfo.SetMethod) : null,
				Type = MemberCache.Types[propertyInfo.PropertyType.TypeHandle]
			};
		}

		public static IImmutableDictionary<string, PropertyMember> CreatePropertyMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetProperties(INSTANCE_BINDINGS)
				.If(propertyInfo => !propertyInfo!.GetIndexParameters().Any())
				.To(propertyInfo => KeyValuePair.Create(propertyInfo!.Name, CreatePropertyMember(propertyInfo)))
				.ToImmutable(StringComparer.Ordinal);

		public static StaticFieldMember CreateStaticFieldMember(FieldInfo fieldInfo)
		{
			fieldInfo.IsLiteral.Assert($"{nameof(fieldInfo)}.{nameof(fieldInfo.IsLiteral)}", false);
			fieldInfo.IsStatic.Assert($"{nameof(fieldInfo)}.{nameof(fieldInfo.IsStatic)}", true);

			var field = Expression.Field(null, fieldInfo);
			var getter = field.Lambda().Compile();
			var getValue = field.As<object>().Lambda<Func<object?>>().Compile();

			Delegate? setter = null;
			Action<object?>? setValue = null;
			if (!fieldInfo.IsInitOnly && !fieldInfo.IsLiteral)
			{
				ParameterExpression value = nameof(value).Parameter(fieldInfo.FieldType);
				setter = field.Assign(value).LambdaAction(value).Compile();

				value = nameof(value).Parameter<object>();
				setValue = field.Assign(value.SystemConvert(fieldInfo.FieldType)).Lambda<Action<object?>>(value).Compile();
			}

			return new StaticFieldMember
			{
				Attributes = fieldInfo.GetCustomAttributes<Attribute>(true).ToImmutableArray(),
				Getter = getter,
				GetValue = getValue,
				Handle = fieldInfo.FieldHandle,
				Internal = fieldInfo.IsAssembly,
				Name = fieldInfo.GetName(),
				Public = fieldInfo.IsPublic,
				Setter = setter,
				SetValue = setValue,
				Type = MemberCache.Types[fieldInfo.FieldType.TypeHandle]
			};
		}

		public static IImmutableDictionary<string, StaticFieldMember> CreateStaticFieldMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetFields(STATIC_BINDINGS)
				.If(fieldInfo => !fieldInfo!.IsLiteral)
				.To(fieldInfo => KeyValuePair.Create(fieldInfo!.Name, CreateStaticFieldMember(fieldInfo)))
				.ToImmutable(StringComparer.Ordinal);

		public static StaticMethodMember CreateStaticMethodMember(MethodInfo methodInfo)
		{
			methodInfo.IsStatic.Assert($"{nameof(methodInfo)}.{nameof(methodInfo.IsStatic)}", true);

			var parameterInfos = methodInfo.GetParameters()
				.If(parameterInfo => parameterInfo != methodInfo.ReturnParameter)
				.Sort(ParameterSortComparer!);

			var parameters = parameterInfos.To(MemberFactory.CreateParameter!).ToImmutable();

			ParameterExpression arguments = nameof(arguments).Parameter<object?[]?>();
			var call = parameterInfos.Any()
				? methodInfo.CallStatic(arguments.ToParameterArray(parameterInfos!))
				: methodInfo.CallStatic();
			var invoke = methodInfo.ReturnType == typeof(void)
				? Expression.Block(call, Expression.Constant(null)).Lambda<StaticInvokeType>(arguments).Compile()
				: call.As<object>().Lambda<StaticInvokeType>(arguments).Compile();
			var methodParameters = parameterInfos.To(parameterInfo => parameterInfo!.Parameter()).ToArray(parameterInfos.Length);
			var method = parameterInfos.Any()
				? methodInfo.CallStatic(methodParameters).Lambda(methodParameters).Compile()
				: call.Lambda().Compile();

			var returnType = MemberCache.Types[methodInfo.ReturnType.TypeHandle];

			return new StaticMethodMember
			{
				Attributes = methodInfo.GetCustomAttributes<Attribute>(true).ToImmutableArray(),
				Handle = methodInfo.MethodHandle,
				Invoke = invoke,
				IsInternal = methodInfo.IsAssembly,
				IsPublic = methodInfo.IsPublic,
				Method = method,
				Parameters = parameters,
				Name = methodInfo.GetName(),
				Return = new ReturnParameter
				{
					Attributes = methodInfo.ReturnParameter.GetCustomAttributes<Attribute>(true).ToImmutableArray(),
					IsTask = returnType.SystemType == SystemType.Task,
					IsValueTask = returnType.SystemType == SystemType.ValueTask,
					IsVoid = returnType.SystemType == SystemType.Void,
					Type = returnType
				}
			};
		}

		public static IImmutableDictionary<string, IImmutableList<StaticMethodMember>> CreateStaticMethodMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetMethods(STATIC_BINDINGS)
				.If(methodInfo => !methodInfo!.ContainsGenericParameters && !methodInfo.IsSpecialName)
				.To(CreateStaticMethodMember!)
				.Group(method => method!.Name, StringComparer.Ordinal)
				.ToImmutableDictionary(_ => _.Key, _ => _.Value.ToImmutable());

		public static Parameter CreateParameter(ParameterInfo parameterInfo)
			=> new Parameter
			{
				Type = MemberCache.Types[parameterInfo.ParameterType.TypeHandle],
				Attributes = parameterInfo.GetCustomAttributes<Attribute>(true).ToImmutableArray(),
				Name = parameterInfo.GetName(),
				DefaultValue = parameterInfo.DefaultValue,
				HasDefaultValue = parameterInfo.HasDefaultValue,
				IsOptional = parameterInfo.IsOptional,
				IsOut = parameterInfo.IsOut
			};

		public static StaticPropertyMember CreateStaticPropertyMember(PropertyInfo propertyInfo)
		{
			var methodInfo = propertyInfo.GetMethod ?? propertyInfo.SetMethod;
			methodInfo!.IsStatic.Assert(nameof(methodInfo.IsStatic), true);

			return new StaticPropertyMember
			{
				Attributes = propertyInfo.GetCustomAttributes<Attribute>(true).ToImmutableArray(),
				Getter = propertyInfo.GetMethod != null ? CreateStaticMethodMember(propertyInfo.GetMethod) : null,
				IsInternal = methodInfo.IsAssembly,
				Name = propertyInfo.GetName(),
				IsPublic = methodInfo.IsPublic,
				Setter = propertyInfo.SetMethod != null ? CreateStaticMethodMember(propertyInfo.SetMethod) : null,
				Type = MemberCache.Types[propertyInfo.PropertyType.TypeHandle]
			};
		}

		public static IImmutableDictionary<string, StaticPropertyMember> CreateStaticPropertyMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetProperties(STATIC_BINDINGS)
				.To(propertyInfo => KeyValuePair.Create(propertyInfo.Name, CreateStaticPropertyMember(propertyInfo)))
				.ToImmutable(StringComparer.Ordinal);

		public static TypeMember CreateTypeMember(Type type)
		{
			var interfaces = type.GetInterfaces();
			var kind = type.GetKind();
			var systemType = type.GetSystemType();
			return new TypeMember
			{
				Attributes = type.GetCustomAttributes<Attribute>(true).ToImmutableArray(),
				BaseTypeHandle = type.BaseType?.TypeHandle,
				GenericTypeHandles = type.GenericTypeArguments.To(_ => _.TypeHandle).ToImmutable(),
				Handle = type.TypeHandle,
				InterfaceTypeHandles = interfaces.To(_ => _.TypeHandle).ToImmutable(interfaces.Length),
				IsEnumerable = type.IsEnumerable(),
				IsInternal = !type.IsVisible,
				IsNullable = kind == Kind.Class || kind == Kind.Delegate || kind == Kind.Interface || systemType == SystemType.Nullable,
				IsPublic = type.IsPublic,
				Kind = kind,
				Name = type.GetName(),
				SystemType = systemType
			};
		}
	}
}
