// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;

namespace TypeCache.Utilities;

public static class Singleton<T>
	where T : class, new()
{
	public static T Instance { get; } = (T)typeof(T).Create()!;
}
