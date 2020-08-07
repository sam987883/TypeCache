// Copyright (c) 2020 Samuel Abraham

using System.Runtime.CompilerServices;

namespace sam987883.Reflection.Accessors
{
	internal sealed class PropertyAccessorFactory<T> : IPropertyAccessorFactory<T>
		where T : class
	{
		private readonly IPropertyCache<T> _PropertyCache;

		public PropertyAccessorFactory(IPropertyCache<T> propertyCache)
		{
			this._PropertyCache = propertyCache;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IPropertyAccessor Create(T instance) =>
			new PropertyAccessor<T>(this._PropertyCache, instance);
	}
}
