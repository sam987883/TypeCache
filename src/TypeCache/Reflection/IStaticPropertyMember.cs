// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Reflection
{
	public interface IStaticPropertyMember : IMember
	{
		IStaticMethodMember? GetMethod { get; }

		IStaticMethodMember? SetMethod { get; }

		object? Value { get; set; }
	}
}
