// Copyright (c) 2021 Samuel Abraham

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
	private readonly Lazy<Func<object?, object?>?> _GetInvoke;
	private readonly Lazy<Delegate?> _GetMethod;
	private readonly Lazy<Action<object?, object?>?> _SetInvoke;
	private readonly Lazy<Delegate?> _SetMethod;

	internal FieldMember(FieldInfo fieldInfo, TypeMember type) : base(fieldInfo)
	{
		this.FieldTypeHandle = fieldInfo.FieldType.TypeHandle;
		this.Handle = fieldInfo.FieldHandle;
		this.Static = fieldInfo.IsStatic;
		this.Type = type;

#nullable disable
		this._GetInvoke = Lazy.Create(() => this.Handle.ToFieldInfo(this.Type.Handle).FieldGetInvoke().Compile());
		this._GetMethod = Lazy.Create(() => this.Handle.ToFieldInfo(this.Type.Handle).FieldGetMethod().Compile());

		var canSet = !fieldInfo.IsInitOnly && !fieldInfo.IsLiteral;
		this._SetInvoke = canSet ? Lazy.Create(() => this.Handle.ToFieldInfo(this.Type.Handle).FieldSetInvoke().Compile()) : Lazy.Null<Action<object, object>>();
		this._SetMethod = canSet ? Lazy.Create(() => this.Handle.ToFieldInfo(this.Type.Handle).FieldSetMethod().Compile()) : Lazy.Null<Delegate>();
#nullable enable
	}

	public TypeMember FieldType => this.FieldTypeHandle.GetTypeMember();

	public RuntimeTypeHandle FieldTypeHandle { get; }

	/// <inheritdoc cref="FieldInfo.FieldHandle"/>
	public RuntimeFieldHandle Handle { get; }

	public Delegate? GetMethod => this._GetMethod.Value;

	public Delegate? SetMethod => this._SetMethod.Value;

	/// <inheritdoc cref="FieldInfo.IsStatic"/>
	public bool Static { get; }

	/// <summary>
	/// The <see cref="TypeMember"/> that owns this <see cref="Member"/>.
	/// </summary>
	public TypeMember Type { get; }

	/// <param name="instance">Pass null if the field is static.</param>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public object? GetValue(object? instance)
		=> this._GetInvoke.Value?.Invoke(instance);

	/// <param name="instance">Pass null if the field is static.</param>
	/// <param name="value">The value to set the property to.</param>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public void SetValue(object? instance, object? value)
		=> this._SetInvoke.Value?.Invoke(instance, value);

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
