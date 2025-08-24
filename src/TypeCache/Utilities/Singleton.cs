// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;
using TypeCache.Reflection;

namespace TypeCache.Utilities;

public static class Singleton<T>
	where T : class, new()
{
	public static T Instance { get; } = Type<T>.Create()!;
}
