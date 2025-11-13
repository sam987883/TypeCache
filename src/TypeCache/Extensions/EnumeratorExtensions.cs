// Copyright (c) 2021 Samuel Abraham

using System.Collections;

namespace TypeCache.Extensions;

public static class EnumeratorExtensions
{
	extension<T>(IEnumerator<T> @this)
	{
		public bool IfNext([NotNullWhen(true)] out T? item)
		{
			if (@this.MoveNext())
			{
				item = @this.Current!;
				return true;
			}

			item = default;
			return false;
		}

		/// <summary>
		/// <c>=&gt; @this.MoveNext() ? @this.Current : <see langword="default"/>;</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public T? Next()
			=> @this.MoveNext() ? @this.Current : default;

		public IEnumerable<T> Rest()
		{
			while (@this.MoveNext())
				yield return @this.Current;
		}
	}

	extension(IEnumerator @this)
	{
		public int Count()
		{
			var count = 0;
			while (@this.MoveNext())
				++count;
			return count;
		}

		public bool IfNext([NotNullWhen(true)] out object? item)
		{
			if (@this.MoveNext())
			{
				item = @this.Current;
				return true;
			}

			item = null;
			return false;
		}

		public bool Move(int count)
		{
			while (count > 0 && @this.MoveNext())
				--count;

			return count == 0;
		}
	}

	extension<E>(ref E @this) where E : struct, IEnumerator
	{
		public int Count()
		{
			var count = 0;
			while (@this.MoveNext())
				++count;

			return count;
		}

		public bool IfNext([NotNullWhen(true)] out object? item)
		{
			if (@this.MoveNext())
			{
				item = @this.Current;
				return true;
			}

			item = null;
			return false;
		}

		public bool Move(int count)
		{
			while (count > 0 && @this.MoveNext())
				--count;

			return count == 0;
		}
	}

	extension<E, T>(ref E @this) where E : struct, IEnumerator<T>
	{
		public bool IfNext([NotNullWhen(true)] out T? item)
		{
			if (@this.MoveNext())
			{
				item = @this.Current!;
				return true;
			}

			item = default;
			return false;
		}

		/// <summary>
		/// <c>=&gt; @this.MoveNext() ? @this.Current : <see langword="default"/>;</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public T? Next()
			=> @this.MoveNext() ? @this.Current : default;
	}
}
