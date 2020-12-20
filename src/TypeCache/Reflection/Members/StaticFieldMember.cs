// Copyright (c) 2021 Samuel Abraham

using System;
using System.Linq.Expressions;
using System.Reflection;
using TypeCache.Extensions;

namespace TypeCache.Reflection.Members
{
	internal sealed class StaticFieldMember : Member, IStaticFieldMember
	{
		public StaticFieldMember(FieldInfo fieldInfo) : base(fieldInfo)
		{
			fieldInfo.IsStatic.Assert($"{nameof(fieldInfo)}.{nameof(fieldInfo.IsStatic)}", true);
			fieldInfo.IsLiteral.Assert($"{nameof(fieldInfo)}.{nameof(fieldInfo.IsLiteral)}", false);

			var field = Expression.Field(null, fieldInfo);
			
			this.Getter = field.Lambda().Compile();
			this.GetValue = field.As<object>().Lambda<Func<object?>>().Compile();

			if (!fieldInfo.IsInitOnly && !fieldInfo.IsLiteral)
			{
				ParameterExpression value = nameof(value).Parameter(fieldInfo.FieldType);
				this.Setter = field.Assign(value).LambdaAction(value).Compile();

				value = nameof(value).Parameter<object>();
				this.SetValue = field.Assign(value.SystemConvert(fieldInfo.FieldType)).Lambda<Action<object?>>(value).Compile();
			}
		}

		public Delegate? Getter { get; }

		public Func<object?>? GetValue { get; }

		public Delegate? Setter { get; }

		public Action<object?>? SetValue { get; }

		public object? Value
		{
			get => this.GetValue?.Invoke();
			set => this.SetValue?.Invoke(value);
		}
	}
}
