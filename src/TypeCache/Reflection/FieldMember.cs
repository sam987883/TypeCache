// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection;

[DebuggerDisplay("{Type}.{Name,nq}", Name = "{Name}")]
public sealed class FieldMember : IMember, IEquatable<FieldMember>
{
	private readonly Lazy<Func<object?, object?>> _GetInvoke;
	private readonly Lazy<Delegate> _GetMethod;
	private readonly Lazy<Action<object?, object?>?> _SetInvoke;
	private readonly Lazy<Delegate?> _SetMethod;

	internal FieldMember(FieldInfo fieldInfo, TypeMember type)
	{
		this.Attributes = fieldInfo.GetCustomAttributes<Attribute>()?.ToImmutableArray() ?? ImmutableArray<Attribute>.Empty;
		this.FieldTypeHandle = fieldInfo.FieldType.TypeHandle;
		this.FieldHandle = fieldInfo.FieldHandle;
		this.Internal = fieldInfo.IsAssembly;
		this.Name = fieldInfo.Name();
		this.Public = fieldInfo.IsPublic;
		this.Static = fieldInfo.IsStatic;
		this.Type = type;

		this._GetInvoke = Lazy.Create(() => this.FieldHandle.ToFieldInfo(this.Type.TypeHandle).FieldGetInvoke().Compile());
		this._GetMethod = Lazy.Create(() => this.FieldHandle.ToFieldInfo(this.Type.TypeHandle).FieldGetMethod().Compile());

		if (!fieldInfo.IsInitOnly && !fieldInfo.IsLiteral)
		{
			this._SetInvoke = Lazy.Create(() => this.FieldHandle.ToFieldInfo(this.Type.TypeHandle).FieldSetInvoke().Compile())!;
			this._SetMethod = Lazy.Create(() => this.FieldHandle.ToFieldInfo(this.Type.TypeHandle).FieldSetMethod().Compile())!;
		}
		else
		{
			this._SetInvoke = Lazy.Null<Action<object?, object?>>();
			this._SetMethod = Lazy.Null<Delegate>();
		}
	}

	/// <inheritdoc/>
	public IReadOnlyList<Attribute> Attributes { get; }

	/// <inheritdoc cref="FieldInfo.FieldHandle"/>
	public RuntimeFieldHandle FieldHandle { get; }

	public TypeMember FieldType => this.FieldTypeHandle.GetTypeMember();

	public RuntimeTypeHandle FieldTypeHandle { get; }

	public Delegate? GetMethod => this._GetMethod.Value;

	public Func<object, object?>? GetValue => this._GetInvoke.Value;

	/// <inheritdoc cref="FieldInfo.IsAssembly"/>
	public bool Internal { get; }

	/// <inheritdoc/>
	public string Name { get; }

	/// <inheritdoc cref="FieldInfo.IsPublic"/>
	public bool Public { get; }

	public Delegate? SetMethod => this._SetMethod.Value;

	public Action<object, object?>? SetValue => this._SetInvoke.Value;

	/// <inheritdoc cref="FieldInfo.IsStatic"/>
	public bool Static { get; }

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    get"/> =&gt; <see langword="this"/>._GetInvoke.Value(<see langword="null"/>);<br/>
	/// <see langword="    set"/> =&gt; <see langword="this"/>._SetInvoke.Value?.Invoke(<see langword="null"/>, <see langword="value"/>);<br/>
	/// }
	/// </code>
	/// </summary>
	/// <remarks>Gets or sets the value of the field if it is <c><see langword="static"/></c>.</remarks>
	public object? StaticValue
	{
		get => this._GetInvoke.Value(null);
		set => this._SetInvoke.Value?.Invoke(null, value);
	}

	/// <summary>
	/// The <see cref="TypeMember"/> that owns this field.
	/// </summary>
	public TypeMember Type { get; }

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public bool Equals([NotNullWhen(true)] FieldMember? other)
		=> this.FieldHandle == other?.FieldHandle;

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override bool Equals([NotNullWhen(true)] object? item)
		=> this.Equals(item as FieldMember);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override int GetHashCode()
		=> this.FieldHandle.GetHashCode();

	/// <summary>
	/// <c>=&gt; <paramref name="member"/>.FieldHandle.ToFieldInfo(<paramref name="member"/>.Type.Handle)!;</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static implicit operator FieldInfo(FieldMember member)
		=> member.FieldHandle.ToFieldInfo(member.Type.TypeHandle);
}
