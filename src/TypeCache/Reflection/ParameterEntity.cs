// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using System.Reflection;
using TypeCache.Collections;
using TypeCache.Extensions;

namespace TypeCache.Reflection;

[DebuggerDisplay("Parameter: {Name}")]
public sealed class ParameterEntity
{
	private readonly RuntimeTypeHandle _ParameterTypeHandle;

	public ParameterEntity(ParameterInfo parameterInfo)
	{
		this._ParameterTypeHandle = parameterInfo.ParameterType.TypeHandle;
		this.Attributes = new ReadOnlyCollection<Attribute>(parameterInfo.GetCustomAttributes());
		this.DefaultValue = parameterInfo.DefaultValue;
		this.HasDefaultValue = parameterInfo.HasDefaultValue;
		this.IsNullable = parameterInfo.ParameterType.IsNullable();
		this.IsOptional = parameterInfo.IsOptional;
		this.IsOut = parameterInfo.IsOut;
		this.IsRetval = parameterInfo.IsRetval;
		this.Name = parameterInfo.Name!;
	}

	public IReadOnlyCollection<Attribute> Attributes { get; }

	public object? DefaultValue { get; }

	public bool HasDefaultValue { get; }

	public bool IsNullable { get; }

	public bool IsOptional { get; }

	public bool IsOut { get; }

	public bool IsRetval { get; }

	public string Name { get; }

	public Type ParameterType => this._ParameterTypeHandle.ToType();

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public bool Supports(Type type)
		=> this.ParameterType.IsAssignableFrom(type);

	public bool SupportsValue(object? value)
		=> value is not null ? this.Supports(value.GetType()) : this.IsNullable;

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public ParameterExpression ToExpression()
		=> Expression.Parameter(this.ParameterType, this.Name);
}
