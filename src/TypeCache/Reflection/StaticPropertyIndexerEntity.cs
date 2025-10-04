// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;

namespace TypeCache.Reflection;

[DebuggerDisplay("Property indexer: {name}")]
public sealed class StaticPropertyIndexerEntity : Property
{
	public StaticPropertyIndexerEntity(PropertyInfo propertyInfo)
		: base(propertyInfo)
	{
		this.IndexParameters = propertyInfo.GetIndexParameters()
			.OrderBy(_ => _.Position)
			.Select(_ => new ParameterEntity(_))
			.ToArray();

		if (propertyInfo.GetMethod is not null)
		{
			propertyInfo.GetMethod.IsStatic.ThrowIfFalse();
			this.GetMethod = new StaticMethodEntity(propertyInfo.GetMethod);
		}

		if (propertyInfo.SetMethod is not null)
		{
			propertyInfo.SetMethod.IsStatic.ThrowIfFalse();
			this.SetMethod = new StaticMethodEntity(propertyInfo.SetMethod);
		}
	}

	public StaticMethodEntity? GetMethod { get; }

	public IReadOnlyList<ParameterEntity> IndexParameters { get; }

	public StaticMethodEntity? SetMethod { get; }

	/// <param name="arguments">Property indexer arguments.</param>
	public object? GetStaticValue(object?[] arguments)
	{
		this.GetMethod.ThrowIfNull();

		return this.GetMethod.Invoke(arguments);
	}

	/// <param name="arguments">Property indexer arguments.</param>
	public object? GetStaticValue(ITuple arguments)
	{
		this.GetMethod.ThrowIfNull();

		return this.GetMethod.Invoke(arguments);
	}

	/// <param name="arguments">Property indexer arguments.</param>
	/// <param name="value">The property value to set.</param>
	public void SetStaticValue(object?[] arguments, object? value)
	{
		this.SetMethod.ThrowIfNull();

		this.SetMethod.Invoke([.. arguments, value]);
	}

	/// <param name="arguments">Property indexer arguments.</param>
	/// <param name="value">The property value to set.</param>
	public void SetStaticValue(ITuple arguments, object? value)
	{
		this.SetMethod.ThrowIfNull();

		this.SetMethod.Invoke([.. arguments.ToArray(), value]);
	}
}
