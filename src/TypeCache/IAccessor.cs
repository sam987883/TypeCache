// Copyright (c) 2021 Samuel Abraham

using System.Diagnostics.CodeAnalysis;

namespace TypeCache;

public interface IAccessor<T>
{
	T this[string name] { get; }

	bool Has([NotNullWhen(true)] string name);
}
