// Copyright (c) 2020 Samuel Abraham

namespace Sam987883.Reflection
{
	public interface IPropertyMember<in T> : IMember
		where T : class
	{
		object? this[T instance] { get; set; }

		IMethodMember<T>? GetMethod { get; }

		IMethodMember<T>? SetMethod { get; }
	}
}
