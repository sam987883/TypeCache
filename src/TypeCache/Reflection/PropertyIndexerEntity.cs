// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;

namespace TypeCache.Reflection;

[DebuggerDisplay("Property indexer: {name}")]
public sealed class PropertyIndexerEntity : Property
{
	public PropertyIndexerEntity(PropertyInfo propertyInfo)
		: base(propertyInfo)
	{
		this.IndexParameters = propertyInfo.GetIndexParameters()
			.OrderBy(_ => _.Position)
			.Select(_ => new ParameterEntity(_))
			.ToArray();

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

	public IReadOnlyList<ParameterEntity> IndexParameters { get; }

	public MethodEntity? SetMethod { get; }

	/// <param name="instance">The object instance to get the property value from.</param>
	/// <param name="arguments">Property indexer arguments.</param>
	public object? GetValue(object instance, object?[] arguments)
	{
		this.GetMethod.ThrowIfNull();

		return this.GetMethod.Invoke(instance, arguments);
	}

	/// <param name="instance">The object instance to get the property value from.</param>
	/// <param name="arguments">Property indexer arguments.</param>
	public object? GetValue(object instance, ITuple arguments)
	{
		this.GetMethod.ThrowIfNull();
		(this.GetMethod is MethodEntity).ThrowIfFalse();

		return this.GetMethod.Invoke(instance, arguments);
	}

	/// <param name="instance">The object instance to set the property value on.</param>
	/// <param name="arguments">Property indexer arguments.</param>
	/// <param name="value">The property value to set.</param>
	public void SetValue(object instance, object?[] arguments, object? value)
	{
		this.SetMethod.ThrowIfNull();

		this.SetMethod.Invoke(instance, [.. arguments, value]);
	}

	/// <param name="instance">The object instance to set the property value on.</param>
	/// <param name="arguments">Property indexer arguments.</param>
	/// <param name="value">The property value to set.</param>
	public void SetValue(object instance, ITuple arguments, object? value)
	{
		this.SetMethod.ThrowIfNull();

		this.SetMethod.Invoke(instance, [.. arguments.ToArray(), value]);
	}
}
