// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;

namespace TypeCache.Common
{
	public class CustomEnumerator<T> : IEnumerator<T>
	{
		private readonly IEnumerator<T> _Enumerator;
		private readonly Func<object, T> _Current;
		private readonly Action<object>? _Dispose;
		private readonly Func<object, bool> _MoveNext;
		private readonly Action<object>? _Reset;

		public CustomEnumerator(IEnumerator<T> enumerator)
		{
			enumerator.AssertNotNull(nameof(IEnumerator<T>));

			var typeCache = Reflection.Caches.TypeCache.BackDoor;
			typeCache.AssertNotNull("TypeCache must be registered by calling extension: RegisterTypeCache(IServiceCollection).");

			this._Enumerator = enumerator;

			var methodCache = typeCache.GetMethodCache(enumerator.GetType());
			var propertyCache = typeCache.GetPropertyCache(enumerator.GetType());

			this._Current = (Func<object, T>)propertyCache.Properties[nameof(IEnumerator<T>.Current)].GetMethod.Method;
			this._Dispose = methodCache.Methods.Get(nameof(IDisposable.Dispose))?.First()?.Method as Action<object>;
			this._MoveNext = (Func<object, bool>)methodCache.Methods.Get(nameof(IEnumerator<T>.MoveNext)).First().Method;
			this._Reset = methodCache.Methods.Get(nameof(IEnumerator<T>.Reset))?.First()?.Method as Action<object>;
		}

		public T Current => this._Current(this._Enumerator);

		object IEnumerator.Current => this._Current(this._Enumerator);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Dispose()
			=> this._Dispose?.Invoke(this._Enumerator);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
			=> this._MoveNext(this._Enumerator);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Reset()
			=> this._Reset?.Invoke(this._Enumerator);
	}
}
