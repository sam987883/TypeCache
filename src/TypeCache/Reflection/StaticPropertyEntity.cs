// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using TypeCache.Extensions;

namespace TypeCache.Reflection;

[DebuggerDisplay("Static property: {Name}")]
public sealed class StaticPropertyEntity : Property
{
	public StaticPropertyEntity(PropertyInfo propertyInfo)
		: base(propertyInfo)
	{
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

	public StaticMethodEntity? SetMethod { get; }

	public object? Value
	{
		get
		{
			this.GetMethod.ThrowIfNull();

			return this.GetMethod.Invoke();
		}
		set
		{
			this.SetMethod.ThrowIfNull();

			this.SetMethod.Invoke(value.ToValueTuple());
		}
	}
}
