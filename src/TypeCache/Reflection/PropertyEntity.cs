// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using TypeCache.Extensions;

namespace TypeCache.Reflection;

[DebuggerDisplay("Property: {Name}")]
public sealed class PropertyEntity : Property
{
	public PropertyEntity(PropertyInfo propertyInfo)
		: base(propertyInfo)
	{
		if (propertyInfo.GetMethod is not null)
		{
			propertyInfo.GetMethod.IsStatic.ThrowIfTrue();
			this.GetMethod = new MethodEntity(propertyInfo.GetMethod);
		}

		if (propertyInfo.SetMethod is not null)
		{
			propertyInfo.SetMethod.IsStatic.ThrowIfTrue();
			this.SetMethod = new MethodEntity(propertyInfo.SetMethod);
		}
	}

	public MethodEntity? GetMethod { get; }

	public MethodEntity? SetMethod { get; }

	/// <param name="instance">The object instance to get the property value from.</param>
	public object? GetValue(object instance)
	{
		this.GetMethod.ThrowIfNull();

		return this.GetMethod.Invoke(instance);
	}

	/// <param name="instance">The object instance to set the property value on.</param>
	/// <param name="value">The property value to set.</param>
	public void SetValue(object instance, object? value)
	{
		this.SetMethod.ThrowIfNull();

		this.SetMethod.Invoke(instance, value.ToValueTuple());
	}
}
