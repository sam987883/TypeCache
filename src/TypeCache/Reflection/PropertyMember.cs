// Copyright (c) 2021 Samuel Abraham

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection;

[DebuggerDisplay("{Type}.{Name,nq}", Name = "{Name}")]
public class PropertyMember	: Member, IEquatable<PropertyMember>
{
	internal PropertyMember(PropertyInfo propertyInfo, TypeMember type) : base(propertyInfo)
	{
		this.PropertyTypeHandle = propertyInfo.PropertyType.TypeHandle;
		this.Indexer = propertyInfo.GetIndexParameters().Any();
		this.Type = type;
		this.Getter = propertyInfo.GetMethod is not null ? new MethodMember(propertyInfo.GetMethod, type) : null;
		this.Setter = propertyInfo.SetMethod is not null ? new MethodMember(propertyInfo.SetMethod, type) : null;
	}

	public bool Indexer { get; }

	public TypeMember PropertyType => this.PropertyTypeHandle.GetTypeMember();

	public RuntimeTypeHandle PropertyTypeHandle { get; }

	public MethodMember? Getter { get; }

	public MethodMember? Setter { get; }

	/// <summary>
	/// The <see cref="TypeMember"/> that owns this <see cref="Member"/>.
	/// </summary>
	public TypeMember Type { get; }

	/// <remarks>First item in <paramref name="arguments"/> must be the instance of the type that the methode belongs to, unless the method is <c><see langword="static"/></c>.</remarks>
	/// <param name="instance">Pass null if the property getter is static.</param>
	/// <param name="indexers">Ignore if property is not an indexer.</param>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public object? GetValue(object instance, params object?[]? indexers)
	{
		if (this.Getter is null)
			return null;

		return instance switch
		{
			not null when indexers.Any() => this.Getter.Invoke(new[] { instance }.Append(indexers).ToArray()),
			not null => this.Getter.Invoke(instance),
			_ when indexers.Any() => this.Getter.Invoke(indexers),
			_ => this.Getter.Invoke()
		};
	}

	/// <remarks>First item in <paramref name="arguments"/> must be the instance of the type that the methode belongs to, unless the method is <c><see langword="static"/></c>.</remarks>
	/// <param name="value">The value to set the property to.</param>
	/// <param name="indexers">Ignore if property is not an indexer.</param>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public void SetValue(object instance, object? value, params object?[]? indexers)
	{
		if (this.Setter is null)
			return;

		var arguments = instance switch
		{
			not null when indexers.Any() => new[] { instance }.Append(indexers).Append(value).ToArray(),
			not null => new[] { instance, value },
			_ when indexers.Any() => indexers.Append(value).ToArray(),
			_ => new[] { value }
		};
		this.Setter.Invoke(arguments);
	}

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public bool Equals([NotNullWhen(true)] PropertyMember? other)
		=> (this.Getter?.Handle, this.Setter?.Handle).Equals((other?.Getter?.Handle, other?.Setter?.Handle));

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override bool Equals([NotNullWhen(true)] object? item)
		=> this.Equals(item as PropertyMember);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override int GetHashCode()
		=> (this.Getter?.Handle, this.Setter?.Handle).GetHashCode();
}
