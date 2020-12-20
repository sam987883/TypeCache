// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace TypeCache.Common
{
	public sealed class CustomEnumerable<T>
    {
        private readonly IEnumerable<T> _Enumerable;

        public CustomEnumerable(IEnumerable<T> enumerable)
        {
            this._Enumerable = enumerable;
        }

        public T? First()
        {
            var enumerator = this.GetEnumerator();
            return enumerator.MoveNext() ? enumerator.Current : default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CustomEnumerator<T> GetEnumerator()
            => new CustomEnumerator<T>(this._Enumerable.GetEnumerator());

        public T? GetItem(Index index)
        {
            var i = index.Value;
            var enumerator = this.GetEnumerator();
            while (i > 0 && enumerator.MoveNext())
                --i;
            return i == 0 ? enumerator.Current : default;
        }
    }
}
