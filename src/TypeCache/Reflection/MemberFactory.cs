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

		private static bool IsInvokable(this Type @this)
			=> !@this.IsPointer && !@this.IsByRef && !@this.IsByRefLike;

		private static bool IsInvokable(this MethodBase @this)
			=> @this.GetParameters().All(_ => !_!.IsOut && _.ParameterType.IsInvokable());

		public static ConstructorMember CreateConstructorMember(ConstructorInfo constructorInfo)
			=> CreateConstructorMember(constructorInfo, MemberCache.Types[constructorInfo.DeclaringType!.TypeHandle]);

		private static ConstructorMember CreateConstructorMember(ConstructorInfo constructorInfo, TypeMember typeMember)
		{
			var parameterInfos = constructorInfo.GetParameters();
			parameterInfos.Sort(ParameterSortComparer);

			CreateType? invoke = null;
			Delegate? method = null;
			if (constructorInfo.IsInvokable())
			{
				var methodParameters = parameterInfos.To(parameterInfo => parameterInfo.Parameter()).ToArray(parameterInfos.Length);
				method = parameterInfos.Any()
					? constructorInfo.New(methodParameters).Lambda(methodParameters).Compile()
					: constructorInfo.New().Lambda().Compile();

				ParameterExpression callParameters = nameof(callParameters).Parameter<object?[]?>();
				invoke = parameterInfos.Any()
					? constructorInfo.New(callParameters.ToParameterArray(parameterInfos)).Cast<object>().Lambda<CreateType>(callParameters).Compile()
					: constructorInfo.New().Cast<object>().Lambda<CreateType>(callParameters).Compile();
			}

			var parameters = parameterInfos.To(CreateParameter!).ToImmutable();
			return new ConstructorMember
			{
				Attributes = constructorInfo.GetCustomAttributes<Attribute>(true).ToImmutable(),
				Create = invoke,
				Handle = constructorInfo.MethodHandle,
				IsInternal = constructorInfo.IsAssembly,
				IsPublic = constructorInfo.IsPublic,
				Method = method,
				Parameters = parameters,
				Type = typeMember
			};
		}

		public static IImmutableList<ConstructorMember> CreateConstructorMembers(RuntimeTypeHandle typeHandle)
		{
			var typeMember = MemberCache.Types[typeHandle];
			return typeHandle.ToType().GetConstructors(INSTANCE_BINDINGS)
				.If(constructorInfo => !constructorInfo!.GetParameters().Any(_ => _!.ParameterType.IsPointer || _.ParameterType.IsByRefLike))
				.To(constructorInfo => CreateConstructorMember(constructorInfo!, typeMember)).ToImmutable();
		}

		public static (Delegate getter, Delegate? setter) CreateFieldAccessorDelegates(FieldInfo fieldInfo)
		{
			ParameterExpression instance = nameof(instance).Parameter(fieldInfo.DeclaringType!);
			var field = instance.Field(fieldInfo);
			var getter = field.Lambda(instance).Compile();

			if (!fieldInfo.IsInitOnly)
			{
				ParameterExpression value = nameof(value).Parameter(fieldInfo.FieldType);
				var setter = field.Assign(value).LambdaAction(instance, value).Compile();
				return (getter, setter);
			}
			return (getter, null);
		}

		public static (GetValue getValue, SetValue? setValue) CreateFieldAccessors(FieldInfo fieldInfo)
		{
			ParameterExpression instance = nameof(instance).Parameter<object>();
			var field = instance.Cast(fieldInfo.DeclaringType!).Field(fieldInfo);
			var getValue = field.As<object>().Lambda<GetValue>(instance).Compile();

			if (!fieldInfo.IsInitOnly)
			{
				ParameterExpression value = nameof(value).Parameter<object>();
				var setValue = field.Assign(value.SystemConvert(fieldInfo.FieldType)).Lambda<SetValue>(instance, value).Compile();
				return (getValue, setValue);
			}
			return (getValue, null);
		}

		public static FieldMember CreateFieldMember(FieldInfo fieldInfo)
		{
			fieldInfo.IsLiteral.Assert($"{nameof(fieldInfo)}.{nameof(fieldInfo.IsLiteral)}", false);
			fieldInfo.IsStatic.Assert($"{nameof(fieldInfo)}.{nameof(fieldInfo.IsStatic)}", false);

			var (getter, setter) = CreateFieldAccessorDelegates(fieldInfo);
			var (getValue, setValue) = CreateFieldAccessors(fieldInfo);

			return new FieldMember
			{
				Attributes = fieldInfo.GetCustomAttributes<Attribute>(true).ToImmutable(),
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
				.If(fieldInfo => !fieldInfo!.IsLiteral && !fieldInfo.FieldType.IsByRefLike)
				.To(fieldInfo => KeyValuePair.Create(fieldInfo!.Name, CreateFieldMember(fieldInfo)))
				.ToImmutable(StringComparer.Ordinal);

		public static IndexerMember CreateIndexerMember(PropertyInfo propertyInfo)
		{
			propertyInfo.GetIndexParameters().Any().Assert($"{nameof(propertyInfo)}.{nameof(propertyInfo.GetIndexParameters)}().Any()", true);

			var methodInfo = propertyInfo.GetMethod ?? propertyInfo.SetMethod;
			return new IndexerMember
			{
				Attributes = propertyInfo.GetCustomAttributes<Attribute>(true).ToImmutable(),
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
			InvokeType? invoke = null;
			Delegate? method = null;

			if (methodInfo.IsInvokable())
			{
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

				method = parameterInfos.Any()
					? instanceCast.Call(methodInfo, callParameters).Lambda(methodParameters).Compile()
					: call.Lambda(instance).Compile();
				invoke = methodInfo.ReturnType == typeof(void)
					? Expression.Block(call, Expression.Constant(null)).Lambda<InvokeType>(instance, arguments).Compile()
					: call.As<object>().Lambda<InvokeType>(instance, arguments).Compile();
			}

			var parameters = parameterInfos.To(CreateParameter!).ToImmutable();
			var returnType = MemberCache.Types[methodInfo.ReturnType.TypeHandle];

			return new MethodMember
			{
				Attributes = methodInfo.GetCustomAttributes<Attribute>(true).ToImmutable(),
				Handle = methodInfo.MethodHandle,
				Invoke = invoke,
				IsInternal = methodInfo.IsAssembly,
				IsPublic = methodInfo.IsPublic,
				Method = method,
				Parameters = parameters,
				Name = methodInfo.GetName(),
				Return = new ReturnParameter
				{
					Attributes = methodInfo.ReturnParameter.GetCustomAttributes<Attribute>(true).ToImmutable(),
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
			GetValue? getValue = null;
			MethodMember? getter = null;
			if (propertyInfo.GetMethod != null)
			{
				ParameterExpression instance = nameof(instance).Parameter<object>();
				propertyInfo.GetMethod.IsStatic.Assert($"{nameof(propertyInfo)}.{nameof(propertyInfo.GetMethod)}.{nameof(propertyInfo.GetMethod.IsStatic)}", false);
				getter = CreateMethodMember(propertyInfo.GetMethod);
				if (propertyInfo.GetMethod.IsInvokable())
					getValue = instance.Cast(propertyInfo.DeclaringType!).Property(propertyInfo).As<object>().Lambda<GetValue>(instance).Compile();
			}

			SetValue? setValue = null;
			MethodMember? setter = null;
			if (propertyInfo.SetMethod != null)
			{
				ParameterExpression instance = nameof(instance).Parameter<object>();
				propertyInfo.SetMethod.IsStatic.Assert($"{nameof(propertyInfo)}.{nameof(propertyInfo.SetMethod)}.{nameof(propertyInfo.SetMethod.IsStatic)}", false);
				setter = CreateMethodMember(propertyInfo.SetMethod);
				var property = instance.Cast(propertyInfo.DeclaringType!).Property(propertyInfo);
				ParameterExpression value = nameof(value).Parameter<object>();
				if (propertyInfo.SetMethod.IsInvokable())
					setValue = instance.Cast(propertyInfo.DeclaringType!).Property(propertyInfo).Assign(value.SystemConvert(propertyInfo.PropertyType)).Lambda<SetValue>(instance, value).Compile();
			}

			var methodInfo = propertyInfo.GetAccessors(true).First()!;
			return new PropertyMember
			{
				Attributes = propertyInfo.GetCustomAttributes<Attribute>(true).ToImmutable(),
				Getter = getter,
				GetValue = getValue,
				IsInternal = methodInfo.IsAssembly,
				Name = propertyInfo.GetName(),
				IsPublic = methodInfo.IsPublic,
				Setter = setter,
				SetValue = setValue,
				Type = MemberCache.Types[propertyInfo.PropertyType.TypeHandle]
			};
		}

		public static IImmutableDictionary<string, PropertyMember> CreatePropertyMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetProperties(INSTANCE_BINDINGS)
				.If(propertyInfo => !propertyInfo!.GetIndexParameters().Any())
				.To(propertyInfo => KeyValuePair.Create(propertyInfo!.Name, CreatePropertyMember(propertyInfo)))
				.ToImmutable(StringComparer.Ordinal);

		public static (Delegate getter, Delegate? setter) CreateStaticFieldAccessorDelegates(FieldInfo fieldInfo)
		{
			var getter = fieldInfo.StaticField().Lambda().Compile();

			if (!fieldInfo.IsInitOnly)
			{
				ParameterExpression value = nameof(value).Parameter(fieldInfo.FieldType);
				var setter = fieldInfo.StaticField().Assign(value).LambdaAction(value).Compile();
				return (getter, setter);
			}
			return (getter, null);
		}

		public static (StaticGetValue getValue, StaticSetValue? setValue) CreateStaticFieldAccessors(FieldInfo fieldInfo)
		{
			var getValue = fieldInfo.StaticField().As<object>().Lambda<StaticGetValue>().Compile();

			if (!fieldInfo.IsInitOnly)
			{
				ParameterExpression value = nameof(value).Parameter<object>();
				var setValue = fieldInfo.StaticField().Assign(value.SystemConvert(fieldInfo.FieldType)).Lambda<StaticSetValue>(value).Compile();
				return (getValue, setValue);
			}
			return (getValue, null);
		}

		public static StaticFieldMember CreateStaticFieldMember(FieldInfo fieldInfo)
		{
			fieldInfo.IsLiteral.Assert($"{nameof(fieldInfo)}.{nameof(fieldInfo.IsLiteral)}", false);
			fieldInfo.IsStatic.Assert($"{nameof(fieldInfo)}.{nameof(fieldInfo.IsStatic)}", true);

			var (getter, setter) = CreateStaticFieldAccessorDelegates(fieldInfo);
			var (getValue, setValue) = CreateStaticFieldAccessors(fieldInfo);

			return new StaticFieldMember
			{
				Attributes = fieldInfo.GetCustomAttributes<Attribute>(true).ToImmutable(),
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
				.If(fieldInfo => !fieldInfo!.IsLiteral && !fieldInfo.FieldType.IsByRefLike)
				.To(fieldInfo => KeyValuePair.Create(fieldInfo!.Name, CreateStaticFieldMember(fieldInfo)))
				.ToImmutable(StringComparer.Ordinal);

		public static StaticMethodMember CreateStaticMethodMember(MethodInfo methodInfo)
		{
			methodInfo.IsStatic.Assert($"{nameof(methodInfo)}.{nameof(methodInfo.IsStatic)}", true);

			var parameterInfos = methodInfo.GetParameters()
				.If(parameterInfo => parameterInfo != methodInfo.ReturnParameter)
				.Sort(ParameterSortComparer!);
			StaticInvokeType? invoke = null;
			Delegate? method = null;

			if (methodInfo.IsInvokable())
			{
				ParameterExpression arguments = nameof(arguments).Parameter<object?[]?>();
				var call = parameterInfos.Any()
					? methodInfo.CallStatic(arguments.ToParameterArray(parameterInfos!))
					: methodInfo.CallStatic();
				var methodParameters = parameterInfos.To(parameterInfo => parameterInfo!.Parameter()).ToArray(parameterInfos.Length);
				method = parameterInfos.Any()
					? methodInfo.CallStatic(methodParameters).Lambda(methodParameters).Compile()
					: call.Lambda().Compile();
				invoke = methodInfo.ReturnType == typeof(void)
					? Expression.Block(call, Expression.Constant(null)).Lambda<StaticInvokeType>(arguments).Compile()
					: call.As<object>().Lambda<StaticInvokeType>(arguments).Compile();
			}

			var parameters = parameterInfos.To(MemberFactory.CreateParameter!).ToImmutable();
			var returnType = MemberCache.Types[methodInfo.ReturnType.TypeHandle];

			return new StaticMethodMember
			{
				Attributes = methodInfo.GetCustomAttributes<Attribute>(true).ToImmutable(),
				Handle = methodInfo.MethodHandle,
				Invoke = invoke,
				IsInternal = methodInfo.IsAssembly,
				IsPublic = methodInfo.IsPublic,
				Method = method,
				Parameters = parameters,
				Name = methodInfo.GetName(),
				Return = new ReturnParameter
				{
					Attributes = methodInfo.ReturnParameter.GetCustomAttributes<Attribute>(true).ToImmutable(),
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
				Attributes = parameterInfo.GetCustomAttributes<Attribute>(true).ToImmutable(),
				Name = parameterInfo.GetName(),
				DefaultValue = parameterInfo.DefaultValue,
				HasDefaultValue = parameterInfo.HasDefaultValue,
				IsOptional = parameterInfo.IsOptional,
				IsOut = parameterInfo.IsOut
			};

		public static StaticPropertyMember CreateStaticPropertyMember(PropertyInfo propertyInfo)
		{
			StaticGetValue? getValue = null;
			StaticMethodMember? getter = null;
			if (propertyInfo.GetMethod != null)
			{
				propertyInfo.GetMethod.IsStatic.Assert($"{nameof(propertyInfo)}.{nameof(propertyInfo.GetMethod)}.{nameof(propertyInfo.GetMethod.IsStatic)}", true);
				getter = CreateStaticMethodMember(propertyInfo.GetMethod);
				if (propertyInfo.GetMethod.IsInvokable())
					getValue = propertyInfo.StaticProperty().As<object>().Lambda<StaticGetValue>().Compile();
			}

			StaticSetValue? setValue = null;
			StaticMethodMember? setter = null;
			if (propertyInfo.SetMethod != null)
			{
				propertyInfo.SetMethod.IsStatic.Assert($"{nameof(propertyInfo)}.{nameof(propertyInfo.SetMethod)}.{nameof(propertyInfo.SetMethod.IsStatic)}", true);
				setter = CreateStaticMethodMember(propertyInfo.SetMethod);
				if (propertyInfo.SetMethod.IsInvokable())
				{
					ParameterExpression value = nameof(value).Parameter<object>();
					setValue = propertyInfo.StaticProperty().Assign(value.SystemConvert(propertyInfo.PropertyType)).Lambda<StaticSetValue>(value).Compile();
				}
			}

			var methodInfo = propertyInfo.GetAccessors(true).First()!;
			return new StaticPropertyMember
			{
				Attributes = propertyInfo.GetCustomAttributes<Attribute>(true).ToImmutable(),
				Getter = getter,
				GetValue = getValue,
				IsInternal = methodInfo.IsAssembly,
				Name = propertyInfo.GetName(),
				IsPublic = methodInfo.IsPublic,
				Setter = setter,
				SetValue = setValue,
				Type = MemberCache.Types[propertyInfo.PropertyType.TypeHandle]
			};
		}

		public static IImmutableDictionary<string, StaticPropertyMember> CreateStaticPropertyMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetProperties(STATIC_BINDINGS)
				//.If(propertyInfo => !propertyInfo!.PropertyType.IsByRefLike)
				.To(propertyInfo => KeyValuePair.Create(propertyInfo!.Name, CreateStaticPropertyMember(propertyInfo)))
				.ToImmutable(StringComparer.Ordinal);

		public static TypeMember CreateTypeMember(Type type)
		{
			var interfaces = type.GetInterfaces();
			var kind = type.GetKind();
			var systemType = type.GetSystemType();
			return new TypeMember
			{
				Attributes = type.GetCustomAttributes<Attribute>(true).ToImmutable(),
				BaseTypeHandle = type.BaseType?.TypeHandle,
				GenericTypeHandles = type.GenericTypeArguments.To(_ => _.TypeHandle).ToImmutable(),
				Handle = type.TypeHandle,
				InterfaceTypeHandles = interfaces.To(_ => _.TypeHandle).ToImmutable(interfaces.Length),
				IsEnumerable = type.IsEnumerable(),
				IsInternal = !type.IsVisible,
				IsNullable = kind == Kind.Class || kind == Kind.Delegate || kind == Kind.Interface || systemType == SystemType.Nullable,
				IsPointer = type.IsPointer,
				IsPublic = type.IsPublic,
				IsRef = type.IsByRefLike,
				Kind = kind,
				Name = type.GetName(),
				SystemType = systemType
			};
		}
	}
}
