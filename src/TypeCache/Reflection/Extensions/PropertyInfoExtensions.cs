// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Reflection.Extensions
{
	public static class PropertyInfoExtensions
	{
		public static (InstanceMethodMember? getter, InstanceMethodMember? setter) CreateAccessorMethods(this PropertyInfo @this)
		{
			InstanceMethodMember? getter = null;
			if (@this.GetMethod is not null)
			{
				@this.GetMethod.IsStatic.Assert($"{nameof(PropertyInfo)}.{nameof(@this.GetMethod)}.{nameof(@this.GetMethod.IsStatic)}", false);
				getter = @this.GetMethod.ToMember();
			}

			InstanceMethodMember? setter = null;
			if (@this.SetMethod is not null)
			{
				@this.SetMethod.IsStatic.Assert($"{nameof(PropertyInfo)}.{nameof(@this.SetMethod)}.{nameof(@this.SetMethod.IsStatic)}", false);
				setter = @this.SetMethod.ToMember();
			}

			return (getter, setter);
		}

		public static (GetValue? getValue, SetValue? setValue) CreateAccessors(this PropertyInfo @this)
		{
			ParameterExpression instance = nameof(instance).Parameter<object>();

			GetValue? getValue = null;
			if (@this.GetMethod is not null && @this.GetMethod.ReturnType.IsInvokable())
			{
				@this.GetMethod.IsStatic.Assert($"{nameof(PropertyInfo)}.{nameof(@this.GetMethod)}.{nameof(@this.GetMethod.IsStatic)}", false);
				getValue = instance.Cast(@this.DeclaringType!).Property(@this).As<object>().Lambda<GetValue>(instance).Compile();
			}

			SetValue? setValue = null;
			if (@this.SetMethod is not null && @this.SetMethod.IsInvokable())
			{
				ParameterExpression value = nameof(value).Parameter<object>();
				@this.SetMethod.IsStatic.Assert($"{nameof(PropertyInfo)}.{nameof(@this.SetMethod)}.{nameof(@this.SetMethod.IsStatic)}", false);
				setValue = instance.Cast(@this.DeclaringType!).Property(@this).Assign(value.SystemConvert(@this.PropertyType)).Lambda<SetValue>(instance, value).Compile();
			}

			return (getValue, setValue);
		}

		public static (StaticMethodMember? getter, StaticMethodMember? setter) CreateStaticAccessorMethods(this PropertyInfo @this)
		{
			StaticMethodMember? getter = null;
			if (@this.GetMethod is not null)
			{
				@this.GetMethod.IsStatic.Assert($"{nameof(PropertyInfo)}.{nameof(@this.GetMethod)}.{nameof(@this.GetMethod.IsStatic)}", true);
				getter = @this.GetMethod.ToStaticMember();
			}

			StaticMethodMember? setter = null;
			if (@this.SetMethod is not null)
			{
				@this.SetMethod.IsStatic.Assert($"{nameof(PropertyInfo)}.{nameof(@this.SetMethod)}.{nameof(@this.SetMethod.IsStatic)}", true);
				setter = @this.SetMethod.ToStaticMember();
			}

			return (getter, setter);
		}

		public static (StaticGetValue? getValue, StaticSetValue? setValue) CreateStaticAccessors(this PropertyInfo @this)
		{
			StaticGetValue? getValue = null;
			if (@this.GetMethod is not null && @this.GetMethod.ReturnType.IsInvokable())
			{
				@this.GetMethod.IsStatic.Assert($"{nameof(PropertyInfo)}.{nameof(@this.GetMethod)}.{nameof(@this.GetMethod.IsStatic)}", true);
				getValue = @this.StaticProperty().As<object>().Lambda<StaticGetValue>().Compile();
			}

			StaticSetValue? setValue = null;
			if (@this.SetMethod is not null && @this.SetMethod.IsInvokable())
			{
				ParameterExpression value = nameof(value).Parameter<object>();
				setValue = @this.StaticProperty().Assign(value.SystemConvert(@this.PropertyType)).Lambda<StaticSetValue>(value).Compile();
			}

			return (getValue, setValue);
		}

		public static IndexerMember ToIndexerMember(this PropertyInfo @this)
		{
			@this.GetIndexParameters().Any().Assert($"{nameof(PropertyInfo)}.{nameof(@this.GetIndexParameters)}().Any()", true);

			var attributes = @this.GetCustomAttributes<Attribute>(true).ToImmutableArray();
			var methodInfo = @this.GetAccessors(true).First()!;
			var getMethod = @this.GetMethod is not null ? @this.GetMethod.ToMember() : null;
			var setMethod = @this.SetMethod is not null ? @this.SetMethod.ToMember() : null;
			var propertyType = @this.PropertyType.GetTypeMember();
			var type = @this.DeclaringType!.GetTypeMember();

			return new IndexerMember(@this.GetName(), propertyType, attributes, methodInfo!.IsAssembly, methodInfo.IsPublic, getMethod, setMethod, type);
		}

		public static InstancePropertyMember ToMember(this PropertyInfo @this)
		{
			var attributes = @this.GetCustomAttributes<Attribute>(true).ToImmutableArray();
			var (getter, setter) = @this.CreateAccessorMethods();
			var (getValue, setValue) = @this.CreateAccessors();
			var methodInfo = @this.GetAccessors(true).First()!;
			var propertyType = @this.PropertyType.GetTypeMember();
			var type = @this.DeclaringType!.GetTypeMember();

			return new InstancePropertyMember(@this.GetName(), propertyType, attributes, methodInfo.IsAssembly, methodInfo.IsPublic, getter, setter, getValue, setValue, type);
		}

		public static StaticPropertyMember ToStaticMember(this PropertyInfo @this)
		{
			var attributes = @this.GetCustomAttributes<Attribute>(true).ToImmutableArray();
			var (getter, setter) = @this.CreateStaticAccessorMethods();
			var (getValue, setValue) = @this.CreateStaticAccessors();
			var methodInfo = @this.GetAccessors(true).First()!;
			var propertyType = @this.PropertyType.GetTypeMember();
			var type = @this.DeclaringType!.GetTypeMember();

			return new StaticPropertyMember(@this.GetName(), propertyType, attributes, methodInfo.IsAssembly, methodInfo.IsPublic, getter, setter, getValue, setValue, type);
		}
	}
}
