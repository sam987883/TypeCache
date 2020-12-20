// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;

namespace TypeCache.Reflection
{
	public interface IMethodMember : IMember
	{
		Func<object, object?[]?, object?> Invoke { get; }

		bool IsCallableWith(params object?[]? arguments);

		bool IsTask { get; }

		bool IsValueTask { get; }

		bool IsVoid { get; }

		Delegate Method { get; }

		IImmutableList<IParameter> Parameters { get; }

		IImmutableList<Attribute> ReturnAttributes { get; }
	}
}
