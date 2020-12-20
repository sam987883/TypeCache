// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Reflection
{
	public interface IIndexerMember : IMember
	{
		object? this[object instance, params object[] indexParameters] { get; set; }

		IMethodMember? GetMethod { get; }

		IMethodMember? SetMethod { get; }
	}
}
