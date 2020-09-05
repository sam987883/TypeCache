// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections.Immutable;

namespace Sam987883.Reflection
{
	public interface IMethodMember<in T> : IMember
		where T : class
	{
		Delegate Method { get; }

		IImmutableList<IParameter> Parameters { get; }

		bool Void { get; }

		object? Invoke(T instance, params object?[]? parameters);

		bool IsCallableWith(params object?[]? arguments);
	}
}
