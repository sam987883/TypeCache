﻿// Copyright (c) 2021 Samuel Abraham

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection;

[DebuggerDisplay("{Type}.{Name,nq}", Name = "{Name}")]
public class FieldMember : Member, IEquatable<FieldMember>
{
	internal FieldMember(FieldInfo fieldInfo, TypeMember type) : base(fieldInfo)
	{
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

	public TypeMember FieldType { get; }

	public RuntimeFieldHandle Handle { get; }

	public Delegate? Getter { get; }

	public Delegate? Setter { get; }

	public bool Static { get; }

	/// <param name="instance">Pass null if the field is static.</param>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public object? GetValue(object? instance)
		=> this._GetValue?.Invoke(instance);

	/// <param name="instance">Pass null if the field is static.</param>
	/// <param name="value">The value to set the property to.</param>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public void SetValue(object? instance, object? value)
		=> this._SetValue?.Invoke(instance, value);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public bool Equals([NotNullWhen(true)] FieldMember? other)
		=> other is not null && this.Handle.Equals(other.Handle);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override bool Equals([NotNullWhen(true)] object? item)
		=> this.Equals(item as FieldMember);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override int GetHashCode()
		=> this.Handle.GetHashCode();

	/// <summary>
	/// <c>=&gt; <paramref name="member"/>.Handle.ToFieldInfo(<paramref name="member"/>.Type.Handle)!;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static implicit operator FieldInfo(FieldMember member)
		=> member.Handle.ToFieldInfo(member.Type!.Handle);
}
