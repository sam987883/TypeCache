// Copyright (c) 2021 Samuel Abraham

using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection;

[DebuggerDisplay("{Type}.{Name,nq}", Name = "{Name}")]
public class PropertyMember	: Member, IEquatable<PropertyMember>
{
	internal PropertyMember(PropertyInfo propertyInfo, TypeMember type) : base(propertyInfo)
	{
		this.PropertyType = propertyInfo.PropertyType.GetTypeMember();
		this.Indexer = propertyInfo.GetIndexParameters().Any();
		this.Getter = propertyInfo.GetMethod is not null ? new MethodMember(propertyInfo.GetMethod, type) : null;
		this.Setter = propertyInfo.SetMethod is not null ? new MethodMember(propertyInfo.SetMethod, type) : null;
	}

	public bool Indexer { get; }

	public TypeMember PropertyType { get; }

	public MethodMember? Getter { get; }

	public MethodMember? Setter { get; }

	/// <param name="instance">Pass null if the property getter is static.</param>
	/// <param name="indexers">Ignore if property is not an indexer.</param>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public object? GetValue(object? instance, params object?[]? indexers)
		=> this.Getter?.Invoke(instance, indexers);

	/// <param name="instance">Pass null if the property setter is static.</param>
	/// <param name="value">The value to set the property to.</param>
	/// <param name="indexers">Ignore if property is not an indexer.</param>
	public void SetValue(object? instance, object? value, params object?[]? indexers)
		=> this.Setter?.Invoke(instance, indexers.Append(value).ToArray());

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public bool Equals(PropertyMember? other)
		=> this.Getter?.Handle == other?.Getter?.Handle && this.Setter?.Handle == other?.Setter?.Handle;
}
