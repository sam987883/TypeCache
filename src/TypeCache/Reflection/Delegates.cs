// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Reflection;

public delegate object CreateType(params object?[]? arguments);

/// <param name="instance">Pass null for static calls.</param>
public delegate object? GetValue(object? instance);

/// <param name="instance">Pass null for static calls.</param>
public delegate object? InvokeType(object? instance, params object?[]? arguments);

/// <param name="instance">Pass null for static calls.</param>
public delegate void SetValue(object? instance, object? value);
