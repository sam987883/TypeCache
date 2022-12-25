// Copyright (c) 2021 Samuel Abraham

using System.Collections.Immutable;
using System.Reflection;
using TypeCache.Extensions;

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

	/// <summary>
	/// <c>=&gt; (<see langword="this"/>.Getter?.Handle, <see langword="this"/>.Setter?.Handle).Equals((<paramref name="other"/>?.Getter?.Handle, <paramref name="other"/>?.Setter?.Handle));</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public bool Equals([NotNullWhen(true)] PropertyMember? other)
		=> (this.Getter?.Handle, this.Setter?.Handle).Equals((other?.Getter?.Handle, other?.Setter?.Handle));

	/// <summary>
	/// <c>=&gt; <see langword="this"/>.Equals(<paramref name="item"/> <see langword="as"/> <see cref="PropertyMember"/>);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public override bool Equals([NotNullWhen(true)] object? item)
		=> this.Equals(item as PropertyMember);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public override int GetHashCode()
		=> (this.Getter?.Handle, this.Setter?.Handle).GetHashCode();
}
