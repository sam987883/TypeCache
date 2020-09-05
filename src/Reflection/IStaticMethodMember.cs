// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections.Immutable;

namespace Sam987883.Reflection
{
	public interface IStaticMethodMember : IMember
	{
		Delegate Method { get; }

		IImmutableList<IParameter> Parameters { get; }

		bool Void { get; }

		object? Invoke(params object?[]? parameters);

		bool IsCallableWith(params object?[]? arguments);
	}
}
