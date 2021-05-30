// Copyright (c) 2021 Samuel Abraham

using System.Threading.Tasks;

namespace TypeCache.Collections
{
	public delegate T ActionRef<T>(ref T item);
	public delegate T ActionRef<T, I>(ref T item, ref I index) where I : unmanaged;
	public delegate ref T FunctionRef<T>();
	public delegate ValueTask<bool> PredicateAsync<in T>(T value);
}
