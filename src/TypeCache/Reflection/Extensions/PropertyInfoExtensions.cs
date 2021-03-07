// Copyright (c) 2021 Samuel Abraham

using System;
using System.Linq.Expressions;
using System.Reflection;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Reflection.Extensions
{
	public static class PropertyInfoExtensions
	{
		public static (MethodMember? getter, MethodMember? setter) CreateAccessorMethods(this PropertyInfo @this)
		{
			MethodMember? getter = null;
			if (@this.GetMethod != null)
			{
				@this.GetMethod.IsStatic.Assert($"{nameof(PropertyInfo)}.{nameof(@this.GetMethod)}.{nameof(@this.GetMethod.IsStatic)}", false);
				getter = @this.GetMethod.CreateMember();
			}

			MethodMember? setter = null;
			if (@this.SetMethod != null)
			{
				@this.SetMethod.IsStatic.Assert($"{nameof(PropertyInfo)}.{nameof(@this.SetMethod)}.{nameof(@this.SetMethod.IsStatic)}", false);
				setter = @this.SetMethod.CreateMember();
			}

			return (getter, setter);
		}

		public static (GetValue? getValue, SetValue? setValue) CreateAccessors(this PropertyInfo @this)
		{
			ParameterExpression instance = nameof(instance).Parameter<object>();

			GetValue? getValue = null;
			if (@this.GetMethod != null && @this.GetMethod.ReturnType.IsInvokable())
			{
				@this.GetMethod.IsStatic.Assert($"{nameof(PropertyInfo)}.{nameof(@this.GetMethod)}.{nameof(@this.GetMethod.IsStatic)}", false);
				getValue = instance.Cast(@this.DeclaringType!).Property(@this).As<object>().Lambda<GetValue>(instance).Compile();
			}

			SetValue? setValue = null;
			if (@this.SetMethod != null && @this.SetMethod.IsInvokable())
			{
				ParameterExpression value = nameof(value).Parameter<object>();
				@this.SetMethod.IsStatic.Assert($"{nameof(PropertyInfo)}.{nameof(@this.SetMethod)}.{nameof(@this.SetMethod.IsStatic)}", false);
				setValue = instance.Cast(@this.DeclaringType!).Property(@this).Assign(value.SystemConvert(@this.PropertyType)).Lambda<SetValue>(instance, value).Compile();
			}

			return (getValue, setValue);
		}

		public static IndexerMember CreateIndexerMember(this PropertyInfo @this)
		{
			@this.GetIndexParameters().Any().Assert($"{nameof(PropertyInfo)}.{nameof(@this.GetIndexParameters)}().Any()", true);

			var attributes = @this.GetCustomAttributes<Attribute>(true).ToImmutable();
			var methodInfo = @this.GetAccessors(true).First()!;
			var getMethod = @this.GetMethod != null ? @this.GetMethod.CreateMember() : null;
			var setMethod = @this.SetMethod != null ? @this.SetMethod.CreateMember() : null;
			var type = MemberCache.Types[@this.PropertyType.TypeHandle];

			return new IndexerMember(@this.GetName(), attributes, methodInfo!.IsAssembly, methodInfo.IsPublic, getMethod, setMethod, type);
		}

		public static PropertyMember CreateMember(this PropertyInfo @this)
		{
			var attributes = @this.GetCustomAttributes<Attribute>(true).ToImmutable();
			var (getter, setter) = @this.CreateAccessorMethods();
			var (getValue, setValue) = @this.CreateAccessors();
			var methodInfo = @this.GetAccessors(true).First()!;
			var type = MemberCache.Types[@this.PropertyType.TypeHandle];

			return new PropertyMember(@this.GetName(), attributes, methodInfo.IsAssembly, methodInfo.IsPublic, getter, getValue, setter, setValue, type);
		}

		public static (StaticMethodMember? getter, StaticMethodMember? setter) CreateStaticAccessorMethods(this PropertyInfo @this)
		{
			StaticMethodMember? getter = null;
			if (@this.GetMethod != null)
			{
				@this.GetMethod.IsStatic.Assert($"{nameof(PropertyInfo)}.{nameof(@this.GetMethod)}.{nameof(@this.GetMethod.IsStatic)}", true);
				getter = @this.GetMethod.CreateStaticMember();
			}

			StaticMethodMember? setter = null;
			if (@this.SetMethod != null)
			{
				@this.SetMethod.IsStatic.Assert($"{nameof(PropertyInfo)}.{nameof(@this.SetMethod)}.{nameof(@this.SetMethod.IsStatic)}", true);
				setter = @this.SetMethod.CreateStaticMember();
			}

			return (getter, setter);
		}

		public static (StaticGetValue? getValue, StaticSetValue? setValue) CreateStaticAccessors(this PropertyInfo @this)
		{
			StaticGetValue? getValue = null;
			if (@this.GetMethod != null && @this.GetMethod.ReturnType.IsInvokable())
			{
				@this.GetMethod.IsStatic.Assert($"{nameof(PropertyInfo)}.{nameof(@this.GetMethod)}.{nameof(@this.GetMethod.IsStatic)}", true);
				getValue = @this.StaticProperty().As<object>().Lambda<StaticGetValue>().Compile();
			}

			StaticSetValue? setValue = null;
			if (@this.SetMethod != null && @this.SetMethod.IsInvokable())
			{
				ParameterExpression value = nameof(value).Parameter<object>();
				setValue = @this.StaticProperty().Assign(value.SystemConvert(@this.PropertyType)).Lambda<StaticSetValue>(value).Compile();
			}

			return (getValue, setValue);
		}

		public static StaticPropertyMember CreateStaticMember(this PropertyInfo @this)
		{
			var attributes = @this.GetCustomAttributes<Attribute>(true).ToImmutable();
			var (getter, setter) = @this.CreateStaticAccessorMethods();
			var (getValue, setValue) = @this.CreateStaticAccessors();
			var methodInfo = @this.GetAccessors(true).First()!;
			var type = MemberCache.Types[@this.PropertyType.TypeHandle];

			return new StaticPropertyMember(@this.GetName(), attributes, methodInfo.IsAssembly, methodInfo.IsPublic, getter, getValue, setter, setValue, type);
		}
	}
}
