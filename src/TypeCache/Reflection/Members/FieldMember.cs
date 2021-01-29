// Copyright (c) 2021 Samuel Abraham

using System;
using System.Linq.Expressions;
using System.Reflection;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Reflection.Members
{
	internal sealed class FieldMember : Member, IFieldMember
	{
		public FieldMember(FieldInfo fieldInfo) : base(fieldInfo)
		{
			fieldInfo.IsLiteral.Assert($"{nameof(fieldInfo)}.{nameof(fieldInfo.IsLiteral)}", false);
			fieldInfo.IsStatic.Assert($"{nameof(fieldInfo)}.{nameof(fieldInfo.IsStatic)}", false);

			ParameterExpression instance = nameof(instance).Parameter(fieldInfo.DeclaringType!);
			var field = instance.Field(fieldInfo);

			this.Getter = field.Lambda(instance).Compile();
			this.GetValue = field.As<object>().Lambda<Func<object, object?>>(instance).Compile();

			if (!fieldInfo.IsInitOnly)
			{
				ParameterExpression value = nameof(value).Parameter(fieldInfo.FieldType);
				this.Setter = field.Assign(value).LambdaAction(instance, value).Compile();

				value = nameof(value).Parameter<object>();
				this.SetValue = field.Assign(value.SystemConvert(fieldInfo.FieldType)).Lambda<Action<object, object?>>(instance, value).Compile();
			}
		}

		public object? this[object instance]
		{
			get => this.GetValue?.Invoke(instance);
			set => this.SetValue?.Invoke(instance, value);
		}

		public Delegate? Getter { get; }

		public Func<object, object?>? GetValue { get; }

		public Delegate? Setter { get; }

		public Action<object, object?>? SetValue { get; }
	}
}
