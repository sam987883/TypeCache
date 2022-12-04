// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Collections;

public delegate void ActionRef<T>(ref T item);
public delegate void ActionRef<T, I>(ref T item, ref I index) where I : unmanaged;
public delegate ref T FunctionRef<T>();
