// Copyright (c) 2020 Samuel Abraham

namespace Sam987883.Reflection
{
	public interface IFieldAccessorFactory<T>
		where T : class
	{
		IFieldAccessor Create(T instance);
	}
}
