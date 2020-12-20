// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;

namespace TypeCache.Reflection
{
	public interface IStaticMethodMember : IMember
	{
		object? Invoke(params object?[]? parameters);

		bool IsCallableWith(params object?[]? arguments);

		bool IsTask { get; }

		bool IsValueTask { get; }

		bool IsVoid { get; }

		Delegate Method { get; }

		IImmutableList<IParameter> Parameters { get; }

		IImmutableList<Attribute> ReturnAttributes { get; }
	}
}
