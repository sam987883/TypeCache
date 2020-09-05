// Copyright (c) 2020 Samuel Abraham

namespace Sam987883.Reflection
{
	public interface IIndexerMember<in T> : IMember
		where T : class
	{
		object? this[T instance, params object[] indexParameters] { get; set; }

		IMethodMember<T>? GetMethod { get; }

		IMethodMember<T>? SetMethod { get; }
	}
}
