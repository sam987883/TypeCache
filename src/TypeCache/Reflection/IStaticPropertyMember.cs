// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Reflection
{
	public interface IStaticPropertyMember : IMember
	{
		IStaticMethodMember? Getter { get; }

		IStaticMethodMember? Setter { get; }

		object? Value { get; set; }
	}
}
