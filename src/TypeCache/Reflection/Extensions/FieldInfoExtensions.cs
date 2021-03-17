// Copyright (c) 2021 Samuel Abraham

using System;
using System.Linq.Expressions;
using System.Reflection;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Reflection.Extensions
{
	public static class FieldInfoExtensions
	{
		public static (Delegate getter, Delegate? setter) CreateAccessorDelegates(this FieldInfo @this)
		{
			ParameterExpression instance = nameof(instance).Parameter(@this.DeclaringType!);
			var field = !@this.IsStatic ? instance.Field(@this) : @this.StaticField();
			var getter = !@this.IsStatic ? field.Lambda(instance).Compile() : field.Lambda().Compile();
			Delegate? setter = null;

			if (!@this.IsInitOnly && !@this.IsLiteral)
			{
				ParameterExpression value = nameof(value).Parameter(@this.FieldType);
				setter = !@this.IsStatic ? field.Assign(value).LambdaAction(instance, value).Compile() : field.Assign(value).LambdaAction(value).Compile();
			}
			return (getter, setter);
		}

		public static (GetValue getValue, SetValue? setValue) CreateAccessors(this FieldInfo @this)
		{
			@this.IsStatic.Assert($"{nameof(FieldInfo)}.{nameof(@this.IsStatic)}", false);

			ParameterExpression instance = nameof(instance).Parameter<object>();
			var field = instance.Cast(@this.DeclaringType!).Field(@this);
			var getValue = field.As<object>().Lambda<GetValue>(instance).Compile();
			SetValue? setValue = null;

			if (!@this.IsInitOnly && !@this.IsLiteral)
			{
				ParameterExpression value = nameof(value).Parameter<object>();
				setValue = field.Assign(value.SystemConvert(@this.FieldType)).Lambda<SetValue>(instance, value).Compile();
			}
			return (getValue, setValue);
		}

		public static FieldMember CreateMember(this FieldInfo @this)
		{
			@this.IsLiteral.Assert($"{nameof(FieldInfo)}.{nameof(@this.IsLiteral)}", false);
			@this.IsStatic.Assert($"{nameof(FieldInfo)}.{nameof(@this.IsStatic)}", false);

			var attributes = @this.GetCustomAttributes<Attribute>(true).ToImmutable();
			var (getter, setter) = @this.CreateAccessorDelegates();
			var (getValue, setValue) = @this.CreateAccessors();
			var type = MemberCache.Types[@this.FieldType.TypeHandle];

			return new FieldMember(@this.GetName(), attributes, @this.IsAssembly, @this.IsPublic, @this.FieldHandle, getter, getValue, setter, setValue, type);
		}

		public static (StaticGetValue getValue, StaticSetValue? setValue) CreateStaticAccessors(this FieldInfo @this)
		{
			@this.IsStatic.Assert($"{nameof(FieldInfo)}.{nameof(@this.IsStatic)}", true);

			var getValue = @this.StaticField().As<object>().Lambda<StaticGetValue>().Compile();
			StaticSetValue? setValue = null;

			if (!@this.IsInitOnly && !@this.IsLiteral)
			{
				ParameterExpression value = nameof(value).Parameter<object>();
				setValue = @this.StaticField().Assign(value.SystemConvert(@this.FieldType)).Lambda<StaticSetValue>(value).Compile();
			}
			return (getValue, setValue);
		}

		public static StaticFieldMember CreateStaticMember(this FieldInfo @this)
		{
			@this.IsStatic.Assert($"{nameof(FieldInfo)}.{nameof(@this.IsStatic)}", true);

			var attributes = @this.GetCustomAttributes<Attribute>(true).ToImmutable();
			var (getter, setter) = @this.CreateAccessorDelegates();
			var (getValue, setValue) = @this.CreateStaticAccessors();
			var type = MemberCache.Types[@this.FieldType.TypeHandle];

			return new StaticFieldMember(@this.GetName(), attributes, @this.IsAssembly, @this.IsPublic, !@this.IsLiteral ? @this.FieldHandle : null, getter, getValue, setter, setValue, type);
		}
	}
}
