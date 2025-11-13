// Copyright (c) 2021 Samuel Abraham

using System.Collections.Frozen;
using System.Reflection;
using TypeCache.Extensions;

namespace TypeCache.Reflection;

public sealed class Literal
{
	private readonly RuntimeTypeHandle _TypeHandle;

	public Literal(FieldInfo fieldInfo)
	{
		fieldInfo.ThrowIfNull();
		fieldInfo.IsLiteral.ThrowIfFalse();
		fieldInfo.DeclaringType.ThrowIfNull();

		this._TypeHandle = fieldInfo.DeclaringType.TypeHandle;

		this.Attributes = fieldInfo.GetCustomAttributes().ToFrozenSet();
		this.IsPublic = fieldInfo.IsPublic;
		this.Name = fieldInfo.Name;
		this.Value = fieldInfo.GetRawConstantValue();
	}

	public IReadOnlySet<Attribute> Attributes { get; }

	public bool IsPublic { get; }

	public string Name { get; }

	public Type Type => this._TypeHandle.ToType();

	public object? Value { get; }
}
