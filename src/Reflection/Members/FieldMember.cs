// Copyright (c) 2020 Samuel Abraham

using Sam987883.Reflection.Extensions;
using System;
using System.Linq.Expressions;
using System.Reflection;
using static Sam987883.Common.Extensions.ExpressionExtensions;
using static Sam987883.Common.Extensions.ObjectExtensions;

namespace Sam987883.Reflection.Members
{
	internal sealed class FieldMember<T> : Member, IFieldMember<T>
		where T : class
	{
		public FieldMember(FieldInfo fieldInfo) : base(fieldInfo)
		{
			fieldInfo.IsLiteral.Assert($"{nameof(fieldInfo)}.{nameof(fieldInfo.IsLiteral)}", false);
			fieldInfo.IsStatic.Assert($"{nameof(fieldInfo)}.{nameof(fieldInfo.IsStatic)}", false);

			ParameterExpression instance = nameof(instance).Parameter<T>();
			var field = instance.Field(fieldInfo);

			this.Getter = field.Lambda(instance).Compile();
			this.GetValue = field.As<object>().Lambda<Func<T, object?>>(instance).Compile();

			if (!fieldInfo.IsInitOnly)
			{
				ParameterExpression value = nameof(value).Parameter(fieldInfo.FieldType);
				this.Setter = field.Assign(value).LambdaAction(instance, value).Compile();

				value = nameof(value).Parameter<object>();
				this.SetValue = field.Assign(value.SystemConvert(fieldInfo.FieldType)).Lambda<Action<T, object?>>(instance, value).Compile();
			}
		}

		public object? this[T instance]
		{
			get => this.GetValue?.Invoke(instance);
			set => this.SetValue?.Invoke(instance, value);
		}

		public Delegate? Getter { get; }

		public Func<T, object?>? GetValue { get; }

		public Delegate? Setter { get; }

		public Action<T, object?>? SetValue { get; }
	}
}
