// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Utilities;

public interface IAccessor<T>
	where T : class, IName
{
	T? this[string name] { get; }

	bool Has([NotNullWhen(true)] string name);
}
