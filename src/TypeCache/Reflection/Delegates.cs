// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Reflection
{
	public delegate object? GetValue(object instance);
	public delegate object? InvokeType(object instance, params object?[]? arguments);
	public delegate void SetValue(object instance, object? value);
	public delegate object? StaticGetValue();
	public delegate object? StaticInvokeType(params object?[]? arguments);
	public delegate void StaticSetValue(object? value);
}
