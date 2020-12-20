// Copyright (c) 2021 Samuel Abraham

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace TypeCache.Common
{
    public class LazyLoad<T>
    {
        private readonly object _ThreadLock = new object();
        private Func<T> _GetValue;
        [AllowNull] private T _Value;

        public LazyLoad([NotNull] Func<T> getValue)
        {
            this._GetValue = initialGetValue;
            this._Value = default;
            this.Loaded = false;

            T initialGetValue()
            {
                lock (this._ThreadLock)
                {
                    if (!this.Loaded)
                    {
                        this._Value = getValue();
                        this._GetValue = () => this._Value;
                        this.Loaded = true;
                    }
                }
                return this._Value;
            }
        }

        public bool Loaded { get; private set; }

        public T Value
            => this._GetValue();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object? value)
            => Equals(this._GetValue(), value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
            => this._GetValue()?.GetHashCode() ?? 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
            => this._GetValue()?.ToString() ?? string.Empty;
    }
}
