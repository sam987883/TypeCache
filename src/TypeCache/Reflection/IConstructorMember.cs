// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;

namespace TypeCache.Reflection
{
	public interface IConstructorMember : IMember
	{
		Delegate Method { get; }

		IImmutableList<IParameter> Parameters { get; }

		Func<object?[]?, object> Invoke { get; }

		bool IsCallableWith(params object?[]? arguments);
	}
}
