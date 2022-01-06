// Copyright (c) 2021 Samuel Abraham

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection;

public class FieldMember : Member, IEquatable<FieldMember>
{
	internal FieldMember(FieldInfo fieldInfo, TypeMember type) : base(fieldInfo)
	{
		this.Type = type;
		this.FieldType = fieldInfo.FieldType.GetTypeMember();
		this.Handle = fieldInfo.FieldHandle;
		this.Static = fieldInfo.IsStatic;

		this.Getter = fieldInfo.FieldGetter().Compile();
		this._GetValue = fieldInfo.FieldGetValue().Compile();

		var canSet = !fieldInfo.IsInitOnly && !fieldInfo.IsLiteral;
		this.Setter = canSet ? fieldInfo.FieldSetter().Compile() : null;
		this._SetValue = canSet ? fieldInfo.FieldSetValue().Compile() : null;
	}

	private readonly GetValue? _GetValue;

	private readonly SetValue? _SetValue;

	public TypeMember Type { get; }

	public TypeMember FieldType { get; }

	public RuntimeFieldHandle Handle { get; }

	public Delegate? Getter { get; }

	public Delegate? Setter { get; }

	public bool Static { get; }

	/// <param name="instance">Pass null if the field is static.</param>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public object? GetValue(object? instance)
		=> this._GetValue?.Invoke(instance);

	/// <param name="instance">Pass null if the field is static.</param>
	/// <param name="value">The value to set the property to.</param>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public void SetValue(object? instance, object? value)
		=> this._SetValue?.Invoke(instance, value);

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public bool Equals(FieldMember? other)
		=> other is not null && this.Handle.Equals(other.Handle);

	/// <summary>
	/// <c>=&gt; <paramref name="member"/>.Handle.ToFieldInfo(<paramref name="member"/>.Type.Handle)!;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static implicit operator FieldInfo(FieldMember member)
		=> member.Handle.ToFieldInfo(member.Type.Handle);
}
