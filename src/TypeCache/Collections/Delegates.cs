// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Collections
{
	public delegate T ActionRef<T>(ref T item);
	public delegate T ActionRef<T, I>(ref T item, ref I index) where I : unmanaged;
}
