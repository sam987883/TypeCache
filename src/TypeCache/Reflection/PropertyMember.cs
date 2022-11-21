// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection;

[DebuggerDisplay("{Type}.{Name,nq}", Name = "{Name}")]
public sealed class PropertyMember	: IMember, IEquatable<PropertyMember>
{
	internal PropertyMember(PropertyInfo propertyInfo, TypeMember type)
	{
		this.Attributes = propertyInfo.GetCustomAttributes<Attribute>()?.ToImmutableArray() ?? ImmutableArray<Attribute>.Empty;
		this.Getter = propertyInfo.GetMethod is not null ? new MethodMember(propertyInfo.GetMethod, type) : null;
		this.Indexer = propertyInfo.GetIndexParameters().Any();
		var accessors = propertyInfo.GetAccessors(true);
		this.Internal = accessors.Any(_ => _.IsAssembly);
		this.Name = propertyInfo.Name();
		this.PropertyTypeHandle = propertyInfo.PropertyType.TypeHandle;
		this.Public = accessors.Any(_ => _.IsPublic);
		this.Setter = propertyInfo.SetMethod is not null ? new MethodMember(propertyInfo.SetMethod, type) : null;
		this.Type = type;
	}

	/// <inheritdoc/>
	public IReadOnlyList<Attribute> Attributes { get; }

	public MethodMember? Getter { get; }

	public bool Indexer { get; }

	/// <inheritdoc/>
	public bool Internal { get; }

	/// <inheritdoc/>
	public string Name { get; }

	public TypeMember PropertyType => this.PropertyTypeHandle.GetTypeMember();

	public RuntimeTypeHandle PropertyTypeHandle { get; }

	/// <inheritdoc/>
	public bool Public { get; }

	public MethodMember? Setter { get; }

	/// <summary>
	/// The <see cref="TypeMember"/> that owns this property.
	/// </summary>
	public TypeMember Type { get; }

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    if"/> (<see langword="this"/>.Getter <see langword="is null"/>)<br/>
	/// <see langword="        return null"/>;<br/>
	/// <br/>
	/// <see langword="    if"/> (!<see langword="this"/>.Getter.Static)<br/>
	/// <see langword="        "/><paramref name="instance"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="    return"/> <paramref name="instance"/> <see langword="switch"/><br/>
	/// <see langword="    "/>{<br/>
	/// <see langword="        not null when"/> <paramref name="indexers"/>?.Any() <see langword="is true"/> =&gt; <see langword="this"/>.Getter.Invoke(<paramref name="indexers"/>.Prepend(<paramref name="instance"/>).ToArray()),<br/>
	/// <see langword="        not null"/> =&gt; <see langword="this"/>.Getter.Invoke(<paramref name="instance"/>),<br/>
	/// <see langword="        "/>_ <see langword="when"/> <paramref name="indexers"/>?.Any() <see langword="is true"/> =&gt; <see langword="this"/>.Getter.Invoke(<paramref name="indexers"/>),<br/>
	/// <see langword="        "/>_ =&gt; <see langword="this"/>.Getter.Invoke()<br/>
	/// <see langword="    "/>};<br/>
	/// }
	/// </code>
	/// </summary>
	/// <remarks>FirstOrDefault item in <paramref name="arguments"/> must be the instance of the type that the methode belongs to, unless the method is <c><see langword="static"/></c>.</remarks>
	/// <param name="instance">Pass null if the property getter is static.</param>
	/// <param name="indexers">Ignore if property is not an indexer.</param>
	/// <exception cref="ArgumentNullException"/>
	public object? GetValue(object instance, params object?[]? indexers)
	{
		if (this.Getter is null)
			return null;

		if (!this.Getter.Static)
			instance.AssertNotNull();

		return instance switch
		{
			not null when indexers?.Any() is true => this.Getter.Invoke(indexers.Prepend(instance).ToArray()),
			not null => this.Getter.Invoke(instance),
			_ when indexers?.Any() is true => this.Getter.Invoke(indexers),
			_ => this.Getter.Invoke()
		};
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    if"/> (<see langword="this"/>.Setter <see langword="is null"/>)<br/>
	/// <see langword="        return null"/>;<br/>
	/// <br/>
	/// <see langword="    if"/> (!<see langword="this"/>.Setter.Static)<br/>
	/// <see langword="        "/><paramref name="instance"/>.AssertNotNull();<br/>
	/// <br/>
	/// <see langword="    var"/> arguments = <paramref name="instance"/> <see langword="switch"/><br/>
	/// <see langword="    "/>{<br/>
	/// <see langword="        not null when"/> <paramref name="indexers"/>?.Any() <see langword="is true"/> =&gt; <paramref name="indexers"/>.Prepend(<paramref name="instance"/>).Append(<paramref name="value"/>).ToArray(),<br/>
	/// <see langword="        not null"/> =&gt; <see langword="new"/>[] { <paramref name="instance"/>, <paramref name="value"/> },<br/>
	/// <see langword="        "/>_ <see langword="when"/> <paramref name="indexers"/>?.Any() <see langword="is true"/> =&gt; <paramref name="indexers"/>).Append(<paramref name="value"/>).ToArray(),<br/>
	/// <see langword="        "/>_ =&gt; <see langword="new"/>[] { <paramref name="value"/> }<br/>
	/// <see langword="    "/>};<br/>
	/// <see langword="    this"/>.Setter.Invoke(arguments)<br/>
	/// }
	/// </code>
	/// </summary>
	/// <remarks>FirstOrDefault item in <paramref name="arguments"/> must be the instance of the type that the methode belongs to, unless the method is <c><see langword="static"/></c>.</remarks>
	/// <param name="instance">Pass null if the property getter is static.</param>
	/// <param name="indexers">Ignore if property is not an indexer.</param>
	/// <exception cref="ArgumentNullException"/>
	public void SetValue(object instance, object? value, params object?[]? indexers)
	{
		if (this.Setter is null)
			return;

		if (!this.Setter.Static)
			instance.AssertNotNull();

		var arguments = instance switch
		{
			not null when indexers?.Any() is true => indexers.Prepend(instance).Append(value).ToArray(),
			not null => new[] { instance, value },
			_ when indexers?.Any() is true => indexers.Append(value).ToArray(),
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
