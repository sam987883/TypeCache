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
				Type = MemberCache.Types[constructorInfo.DeclaringType!.TypeHandle]
			};
		}

		public static IImmutableList<ConstructorMember> CreateConstructorMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetConstructors(INSTANCE_BINDINGS).To(CreateConstructorMember).ToImmutable();

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
				Name = fieldInfo.GetCustomAttribute<NameAttribute>(false)?.Name ?? fieldInfo.Name,
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

			return new MethodMember
			{
				Attributes = methodInfo.GetCustomAttributes<Attribute>(true).ToImmutableArray(),
				Handle = methodInfo.MethodHandle,
				Invoke = invoke,
				IsInternal = methodInfo.IsAssembly,
				IsPublic = methodInfo.IsPublic,
				IsVoid = methodInfo.ReturnType.IsVoid(),
				Method = method,
				Parameters = parameters,
				Name = methodInfo.GetCustomAttribute<NameAttribute>()?.Name ?? methodInfo.Name,
				ReturnAttributes = methodInfo.ReturnParameter.GetCustomAttributes<Attribute>(true).ToImmutableArray(),
				Type = MemberCache.Types[methodInfo.ReturnType.TypeHandle]
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
				Name = propertyInfo.GetCustomAttribute<NameAttribute>()?.Name ?? propertyInfo.Name,
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
				Name = fieldInfo.GetCustomAttribute<NameAttribute>(false)?.Name ?? fieldInfo.Name,
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

			return new StaticMethodMember
			{
				Attributes = methodInfo.GetCustomAttributes<Attribute>(true).ToImmutableArray(),
				Handle = methodInfo.MethodHandle,
				Invoke = invoke,
				IsInternal = methodInfo.IsAssembly,
				IsPublic = methodInfo.IsPublic,
				IsVoid = methodInfo.ReturnType.IsVoid(),
				Method = method,
				Parameters = parameters,
				Name = methodInfo.GetCustomAttribute<NameAttribute>()?.Name ?? methodInfo.Name,
				ReturnAttributes = methodInfo.ReturnParameter.GetCustomAttributes<Attribute>(true).ToImmutableArray(),
				Type = MemberCache.Types[methodInfo.ReturnType.TypeHandle]
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
				Name = parameterInfo.GetCustomAttribute<NameAttribute>()?.Name ?? parameterInfo.Name!,
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
				Name = propertyInfo.GetCustomAttribute<NameAttribute>()?.Name ?? propertyInfo.Name,
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
			=> new TypeMember
			{
				Attributes = type.GetCustomAttributes<Attribute>(true).ToImmutableArray(),
				BaseHandle = type.BaseType?.TypeHandle,
				CollectionType = type.ToCollectionType(),
				Handle = type.TypeHandle,
				InterfaceHandles = type.GetInterfaces().To(_ => _.TypeHandle).ToImmutable(type.GetInterfaces().Length),
				IsInternal = !type.IsVisible,
				IsNullable = type.IsNullable(),
				IsPublic = type.IsPublic,
				IsTask = type.IsTask(),
				IsValueTask = type.IsValueTask(),
				Kind = type.ToKind(),
				Name = type.GetCustomAttribute<NameAttribute>(false)?.Name ?? type.Name,
				NativeType = type.ToNativeType()
			};
	}
}
