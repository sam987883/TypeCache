// Copyright (c) 2021 Samuel Abraham

using System.Collections.Frozen;
using System.Reflection;
using TypeCache.Collections;
using TypeCache.Extensions;

namespace TypeCache.Reflection;

public abstract class Field
{
	private readonly RuntimeTypeHandle _FieldTypeHandle;
	private readonly RuntimeTypeHandle _TypeHandle;

	public Field(FieldInfo fieldInfo)
	{
		fieldInfo.ThrowIfNull();
		fieldInfo.IsLiteral.ThrowIfTrue();
		fieldInfo.DeclaringType.ThrowIfNull();

		this._FieldTypeHandle = fieldInfo.FieldType.TypeHandle;
		this._TypeHandle = fieldInfo.DeclaringType.TypeHandle;

		this.Attributes = fieldInfo.GetCustomAttributes().ToFrozenSet();
		if (!fieldInfo.IsLiteral)
			this.Handle = fieldInfo.FieldHandle;

		this.IsPublic = fieldInfo.IsPublic;
		this.Name = fieldInfo.Name;
	}

	public IReadOnlySet<Attribute> Attributes { get; }

	public Type FieldType => this._FieldTypeHandle.ToType();

	public RuntimeFieldHandle Handle { get; }

	public bool IsPublic { get; }

	public string Name { get; }

	public Type Type => this._TypeHandle.ToType();

	public FieldInfo ToFieldInfo()
		=> this.Handle.ToFieldInfo(this._TypeHandle);
}
