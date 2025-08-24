// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using System.Reflection;
using TypeCache.Extensions;

namespace TypeCache.Reflection;

[DebuggerDisplay("Field: {Name}")]
public sealed class FieldEntity : Field
{
	private readonly Lazy<Func<object, object?>> _GetValue;
	private readonly Lazy<Action<object, object?>> _SetValue;

	public FieldEntity(FieldInfo fieldInfo) : base(fieldInfo)
	{
		fieldInfo.IsLiteral.ThrowIfTrue();
		fieldInfo.IsStatic.ThrowIfTrue();

		this._GetValue = new(() =>
		{
			ParameterExpression instance = nameof(instance).ToParameterExpression<object>();
			var fieldInfo = this.ToFieldInfo();
			return instance
				.Cast(fieldInfo.DeclaringType!)
				.Field(fieldInfo)
				.As<object>()
				.Lambda<Func<object, object?>>([instance])
				.Compile();
		});

		this._SetValue = new(() =>
		{
			ParameterExpression instance = nameof(instance).ToParameterExpression<object>();
			ParameterExpression value = nameof(value).ToParameterExpression<object>();
			var fieldInfo = this.ToFieldInfo();
			return instance
				.Convert(fieldInfo.DeclaringType!)
				.Field(fieldInfo)
				.Assign(value.Convert(fieldInfo.FieldType))
				.Lambda<Action<object, object?>>([instance, value])
				.Compile();
		});
	}

	public Func<object, object?> GetValue => this._GetValue.Value;

	public Action<object, object?> SetValue => this._SetValue.Value;
}
