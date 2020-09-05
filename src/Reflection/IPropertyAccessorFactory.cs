// Copyright (c) 2020 Samuel Abraham

namespace Sam987883.Reflection
{
	public interface IPropertyAccessorFactory<T>
		where T : class
	{
		IPropertyAccessor Create(T instance);
	}
}
