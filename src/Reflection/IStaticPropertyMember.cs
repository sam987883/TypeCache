// Copyright (c) 2020 Samuel Abraham

namespace Sam987883.Reflection
{
	public interface IStaticPropertyMember : IMember
	{
		IStaticMethodMember? GetMethod { get; }

		IStaticMethodMember? SetMethod { get; }

		object? Value { get; set; }
	}
}
