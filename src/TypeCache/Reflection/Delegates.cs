// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Reflection
{
	public delegate object? InvokeType(object instance, params object?[]? arguments);
	public delegate object? StaticInvokeType(params object?[]? arguments);
}
