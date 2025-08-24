// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using System.Reflection;
using TypeCache.Extensions;

namespace TypeCache.Reflection;

[DebuggerDisplay("Static field: {Name}")]
public sealed class StaticFieldEntity : Field
{
	private readonly Lazy<Func<object?>> _GetValue;
	private readonly Lazy<Action<object?>>? _SetValue;

	public StaticFieldEntity(FieldInfo fieldInfo) : base(fieldInfo)
	{
		(fieldInfo.IsLiteral || fieldInfo.IsStatic).ThrowIfFalse();

		this._GetValue = new(() =>
		{
			var fieldInfo = this.ToFieldInfo();
			var literalValue = fieldInfo.IsLiteral ? fieldInfo.GetRawConstantValue() : null;
			return fieldInfo.IsLiteral
				? () => literalValue
				: fieldInfo.ToExpression(null).Cast<object>().Lambda<Func<object?>>().Compile();
		});

		if (!fieldInfo.IsInitOnly && !fieldInfo.IsLiteral)
		{
			this._SetValue = new(() =>
			{
				ParameterExpression value = nameof(value).ToParameterExpression<object>();
				var fieldInfo = this.ToFieldInfo();
				return fieldInfo
					.ToExpression(null)
					.Assign(value.Convert(fieldInfo.FieldType))
					.Lambda<Action<object?>>([value])
					.Compile();
			});
		}
	}

	public Func<object?> GetValue => this._GetValue.Value;

	public Action<object?>? SetValue => this._SetValue?.Value;
}
