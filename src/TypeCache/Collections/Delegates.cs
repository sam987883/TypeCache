// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Collections;

public delegate void ActionRef<T>(ref T item);
public delegate void ActionIndexRef<T>(ref T item, int index);
public delegate ref T FunctionRef<T>();
