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

		private static ConstructorMember CreateConstructorMember(ConstructorInfo constructorInfo, TypeMember type)
		{
			var parameterInfos = constructorInfo.GetParameters();
			parameterInfos.Sort(ParameterSortComparer);

			CreateType? create = null;
			Delegate? method = null;
			if (constructorInfo.IsInvokable())
			{
				var methodParameters = parameterInfos.To(parameterInfo => parameterInfo.Parameter()).ToArray(parameterInfos.Length);
				method = parameterInfos.Any()
					? constructorInfo.New(methodParameters).Lambda(methodParameters).Compile()
					: constructorInfo.New().Lambda().Compile();

				ParameterExpression callParameters = nameof(callParameters).Parameter<object?[]?>();
				create = parameterInfos.Any()
					? constructorInfo.New(callParameters.ToParameterArray(parameterInfos)).Cast<object>().Lambda<CreateType>(callParameters).Compile()
					: constructorInfo.New().Cast<object>().Lambda<CreateType>(callParameters).Compile();
			}

			var attributes = constructorInfo.GetCustomAttributes<Attribute>(true).ToImmutable();
			var parameters = parameterInfos.To(CreateParameter!).ToImmutable();

			return new ConstructorMember(constructorInfo.Name, attributes, constructorInfo.IsAssembly, constructorInfo.IsPublic, constructorInfo.MethodHandle, create, method, parameters, type);
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

			var attributes = fieldInfo.GetCustomAttributes<Attribute>(true).ToImmutable();
			var (getter, setter) = CreateFieldAccessorDelegates(fieldInfo);
			var (getValue, setValue) = CreateFieldAccessors(fieldInfo);
			var type = MemberCache.Types[fieldInfo.FieldType.TypeHandle];

			return new FieldMember(fieldInfo.GetName(), attributes, fieldInfo.IsAssembly, fieldInfo.IsPublic, fieldInfo.FieldHandle, getter, getValue, setter, setValue, type);
		}

		public static IImmutableDictionary<string, FieldMember> CreateFieldMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetFields(INSTANCE_BINDINGS)
				.If(fieldInfo => !fieldInfo!.IsLiteral && !fieldInfo.FieldType.IsByRefLike)
				.To(fieldInfo => KeyValuePair.Create(fieldInfo!.Name, CreateFieldMember(fieldInfo)))
				.ToImmutable(StringComparer.Ordinal);

		public static IndexerMember CreateIndexerMember(PropertyInfo propertyInfo)
		{
			propertyInfo.GetIndexParameters().Any().Assert($"{nameof(propertyInfo)}.{nameof(propertyInfo.GetIndexParameters)}().Any()", true);

			var attributes = propertyInfo.GetCustomAttributes<Attribute>(true).ToImmutable();
			var methodInfo = propertyInfo.GetAccessors(true).First()!;
			var getMethod = propertyInfo.GetMethod != null ? CreateMethodMember(propertyInfo.GetMethod) : null;
			var setMethod = propertyInfo.SetMethod != null ? CreateMethodMember(propertyInfo.SetMethod) : null;
			var type = MemberCache.Types[propertyInfo.PropertyType.TypeHandle];

			return new IndexerMember(propertyInfo.GetName(), attributes, methodInfo!.IsAssembly, methodInfo.IsPublic, getMethod, setMethod, type);
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

				if (methodInfo.ReturnType.IsInvokable())
					invoke = methodInfo.ReturnType == typeof(void)
						? Expression.Block(call, Expression.Constant(null)).Lambda<InvokeType>(instance, arguments).Compile()
						: call.As<object>().Lambda<InvokeType>(instance, arguments).Compile();
			}

			var attributes = methodInfo.GetCustomAttributes<Attribute>(true).ToImmutable();
			var parameters = parameterInfos.To(CreateParameter!).ToImmutable();
			var returnAttributes = methodInfo.ReturnParameter.GetCustomAttributes<Attribute>(true).ToImmutable();
			var returnType = MemberCache.Types[methodInfo.ReturnType.TypeHandle];
			var returnParameter = new ReturnParameter(returnAttributes, returnType);

			return new MethodMember(methodInfo.GetName(), attributes, methodInfo.IsAssembly, methodInfo.IsPublic, methodInfo.MethodHandle, invoke, method, parameters, returnParameter);
		}

		public static IImmutableDictionary<string, IImmutableList<MethodMember>> CreateMethodMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetMethods(INSTANCE_BINDINGS)
				.If(methodInfo => !methodInfo!.ContainsGenericParameters && !methodInfo.IsSpecialName)
				.To(CreateMethodMember!)
				.Group(method => method!.Name, StringComparer.Ordinal)
				.ToImmutableDictionary(_ => _.Key, _ => _.Value.ToImmutable());

		public static (MethodMember? getter, MethodMember? setter) CreatePropertyAccessorMethods(PropertyInfo propertyInfo)
		{
			MethodMember? getter = null;
			if (propertyInfo.GetMethod != null)
			{
				propertyInfo.GetMethod.IsStatic.Assert($"{nameof(propertyInfo)}.{nameof(propertyInfo.GetMethod)}.{nameof(propertyInfo.GetMethod.IsStatic)}", false);
				getter = CreateMethodMember(propertyInfo.GetMethod);
			}

			MethodMember? setter = null;
			if (propertyInfo.SetMethod != null)
			{
				propertyInfo.SetMethod.IsStatic.Assert($"{nameof(propertyInfo)}.{nameof(propertyInfo.SetMethod)}.{nameof(propertyInfo.SetMethod.IsStatic)}", false);
				setter = CreateMethodMember(propertyInfo.SetMethod);
			}

			return (getter, setter);
		}

		public static (GetValue? getValue, SetValue? setValue) CreatePropertyAccessors(PropertyInfo propertyInfo)
		{
			ParameterExpression instance = nameof(instance).Parameter<object>();

			GetValue? getValue = null;
			if (propertyInfo.GetMethod != null && propertyInfo.GetMethod.ReturnType.IsInvokable())
			{
				propertyInfo.GetMethod.IsStatic.Assert($"{nameof(propertyInfo)}.{nameof(propertyInfo.GetMethod)}.{nameof(propertyInfo.GetMethod.IsStatic)}", false);
				getValue = instance.Cast(propertyInfo.DeclaringType!).Property(propertyInfo).As<object>().Lambda<GetValue>(instance).Compile();
			}

			SetValue? setValue = null;
			if (propertyInfo.SetMethod != null && propertyInfo.SetMethod.IsInvokable())
			{
				ParameterExpression value = nameof(value).Parameter<object>();
				propertyInfo.SetMethod.IsStatic.Assert($"{nameof(propertyInfo)}.{nameof(propertyInfo.SetMethod)}.{nameof(propertyInfo.SetMethod.IsStatic)}", false);
				setValue = instance.Cast(propertyInfo.DeclaringType!).Property(propertyInfo).Assign(value.SystemConvert(propertyInfo.PropertyType)).Lambda<SetValue>(instance, value).Compile();
			}

			return (getValue, setValue);
		}

		public static PropertyMember CreatePropertyMember(PropertyInfo propertyInfo)
		{
			var attributes = propertyInfo.GetCustomAttributes<Attribute>(true).ToImmutable();
			var (getter, setter) = CreatePropertyAccessorMethods(propertyInfo);
			var (getValue, setValue) = CreatePropertyAccessors(propertyInfo);
			var methodInfo = propertyInfo.GetAccessors(true).First()!;
			var type = MemberCache.Types[propertyInfo.PropertyType.TypeHandle];

			return new PropertyMember(propertyInfo.GetName(), attributes, methodInfo.IsAssembly, methodInfo.IsPublic, getter, getValue, setter, setValue, type);
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

			var attributes = fieldInfo.GetCustomAttributes<Attribute>(true).ToImmutable();
			var (getter, setter) = CreateStaticFieldAccessorDelegates(fieldInfo);
			var (getValue, setValue) = CreateStaticFieldAccessors(fieldInfo);
			var type = MemberCache.Types[fieldInfo.FieldType.TypeHandle];

			return new StaticFieldMember(fieldInfo.GetName(), attributes, fieldInfo.IsAssembly, fieldInfo.IsPublic, fieldInfo.FieldHandle, getter, getValue, setter, setValue, type);
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

				if (methodInfo.ReturnType.IsInvokable())
					invoke = methodInfo.ReturnType == typeof(void)
						? Expression.Block(call, Expression.Constant(null)).Lambda<StaticInvokeType>(arguments).Compile()
						: call.As<object>().Lambda<StaticInvokeType>(arguments).Compile();
			}

			var attributes = methodInfo.GetCustomAttributes<Attribute>(true).ToImmutable();
			var parameters = parameterInfos.To(CreateParameter!).ToImmutable();
			var returnAttributes = methodInfo.ReturnParameter.GetCustomAttributes<Attribute>(true).ToImmutable();
			var returnType = MemberCache.Types[methodInfo.ReturnType.TypeHandle];
			var returnParameter = new ReturnParameter(returnAttributes, returnType);

			return new StaticMethodMember(methodInfo.GetName(), attributes, methodInfo.IsAssembly, methodInfo.IsPublic, methodInfo.MethodHandle, invoke, method, parameters, returnParameter);
		}

		public static IImmutableDictionary<string, IImmutableList<StaticMethodMember>> CreateStaticMethodMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetMethods(STATIC_BINDINGS)
				.If(methodInfo => !methodInfo!.ContainsGenericParameters && !methodInfo.IsSpecialName)
				.To(CreateStaticMethodMember!)
				.Group(method => method!.Name, StringComparer.Ordinal)
				.ToImmutableDictionary(_ => _.Key, _ => _.Value.ToImmutable());

		public static Parameter CreateParameter(ParameterInfo parameterInfo)
		{
			var attributes = parameterInfo.GetCustomAttributes<Attribute>(true).ToImmutable();
			var type = MemberCache.Types[parameterInfo.ParameterType.TypeHandle];

			return new Parameter(parameterInfo.GetName(), attributes, parameterInfo.IsOptional, parameterInfo.IsOut, parameterInfo.DefaultValue, parameterInfo.HasDefaultValue, type);
		}

		public static (StaticMethodMember? getter, StaticMethodMember? setter) CreateStaticPropertyAccessorMethods(PropertyInfo propertyInfo)
		{
			StaticMethodMember? getter = null;
			if (propertyInfo.GetMethod != null)
			{
				propertyInfo.GetMethod.IsStatic.Assert($"{nameof(propertyInfo)}.{nameof(propertyInfo.GetMethod)}.{nameof(propertyInfo.GetMethod.IsStatic)}", true);
				getter = CreateStaticMethodMember(propertyInfo.GetMethod);
			}

			StaticMethodMember? setter = null;
			if (propertyInfo.SetMethod != null)
			{
				propertyInfo.SetMethod.IsStatic.Assert($"{nameof(propertyInfo)}.{nameof(propertyInfo.SetMethod)}.{nameof(propertyInfo.SetMethod.IsStatic)}", true);
				setter = CreateStaticMethodMember(propertyInfo.SetMethod);
			}

			return (getter, setter);
		}

		public static (StaticGetValue? getValue, StaticSetValue? setValue) CreateStaticPropertyAccessors(PropertyInfo propertyInfo)
		{
			StaticGetValue? getValue = null;
			if (propertyInfo.GetMethod != null && propertyInfo.GetMethod.ReturnType.IsInvokable())
			{
				propertyInfo.GetMethod.IsStatic.Assert($"{nameof(propertyInfo)}.{nameof(propertyInfo.GetMethod)}.{nameof(propertyInfo.GetMethod.IsStatic)}", true);
				getValue = propertyInfo.StaticProperty().As<object>().Lambda<StaticGetValue>().Compile();
			}

			StaticSetValue? setValue = null;
			if (propertyInfo.SetMethod != null && propertyInfo.SetMethod.IsInvokable())
			{
				ParameterExpression value = nameof(value).Parameter<object>();
				setValue = propertyInfo.StaticProperty().Assign(value.SystemConvert(propertyInfo.PropertyType)).Lambda<StaticSetValue>(value).Compile();
			}

			return (getValue, setValue);
		}

		public static StaticPropertyMember CreateStaticPropertyMember(PropertyInfo propertyInfo)
		{
			var attributes = propertyInfo.GetCustomAttributes<Attribute>(true).ToImmutable();
			var (getter, setter) = CreateStaticPropertyAccessorMethods(propertyInfo);
			var (getValue, setValue) = CreateStaticPropertyAccessors(propertyInfo);
			var methodInfo = propertyInfo.GetAccessors(true).First()!;
			var type = MemberCache.Types[propertyInfo.PropertyType.TypeHandle];

			return new StaticPropertyMember(propertyInfo.GetName(), attributes, methodInfo.IsAssembly, methodInfo.IsPublic, getter, getValue, setter, setValue, type);
		}

		public static IImmutableDictionary<string, StaticPropertyMember> CreateStaticPropertyMembers(RuntimeTypeHandle typeHandle)
			=> typeHandle.ToType().GetProperties(STATIC_BINDINGS)
				.To(propertyInfo => KeyValuePair.Create(propertyInfo!.Name, CreateStaticPropertyMember(propertyInfo)))
				.ToImmutable(StringComparer.Ordinal);

		public static TypeMember CreateTypeMember(Type type)
		{
			var interfaces = type.GetInterfaces();
			var kind = type.GetKind();
			var systemType = type.GetSystemType();
			var isNullable = kind == Kind.Class || kind == Kind.Delegate || kind == Kind.Interface || systemType == SystemType.Nullable;
			var attributes = type.GetCustomAttributes<Attribute>(true).ToImmutable();
			var genericTypeHandles = type.GenericTypeArguments.To(_ => _.TypeHandle).ToImmutable();
			var interfaceTypeHandles = interfaces.To(_ => _.TypeHandle).ToImmutable(interfaces.Length);

			return new TypeMember(type.GetName(), attributes, type.IsPointer, type.IsPublic, kind, systemType, type.TypeHandle, type.BaseType?.TypeHandle,
				genericTypeHandles, interfaceTypeHandles, type.IsEnumerable(), !type.IsVisible, isNullable, type.IsByRef || type.IsByRefLike);
		}
	}
}
