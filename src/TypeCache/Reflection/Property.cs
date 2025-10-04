// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using TypeCache.Collections;
using TypeCache.Extensions;

namespace TypeCache.Reflection;

[DebuggerDisplay("Property: {Name}")]
public class Property
{
	private readonly RuntimeTypeHandle _PropertyTypeHandle;
	private readonly RuntimeTypeHandle _TypeHandle;

	public Property(PropertyInfo propertyInfo)
	{
		propertyInfo.ThrowIfNull();
		propertyInfo.DeclaringType.ThrowIfNull();

		this._PropertyTypeHandle = propertyInfo.PropertyType.TypeHandle;
		this._TypeHandle = propertyInfo.DeclaringType.TypeHandle;

		this.Attributes = new ReadOnlyCollection<Attribute>(propertyInfo.GetCustomAttributes());
		this.CanRead = propertyInfo.CanRead;
		this.CanWrite = propertyInfo.CanWrite;
		this.Name = propertyInfo.Name;
	}

	public IReadOnlyCollection<Attribute> Attributes { get; }

	public bool CanRead { get; }

	public bool CanWrite { get; }

	public string Name { get; }

	public Type PropertyType => this._PropertyTypeHandle.ToType();

	public Type Type => this._TypeHandle.ToType();
}
