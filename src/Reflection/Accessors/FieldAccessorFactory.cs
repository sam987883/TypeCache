// Copyright (c) 2020 Samuel Abraham

using System.Runtime.CompilerServices;

namespace Sam987883.Reflection.Accessors
{
	internal sealed class FieldAccessorFactory<T> : IFieldAccessorFactory<T>
		where T : class
	{
		private readonly IFieldCache<T> _FieldCache;

		public FieldAccessorFactory(IFieldCache<T> fieldCache)
		{
			this._FieldCache = fieldCache;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IFieldAccessor Create(T instance) =>
			new FieldAccessor<T>(this._FieldCache, instance);
	}
}
