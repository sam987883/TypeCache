// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Extensions;

public static class BooleanExtensions
{
	extension(bool @this)
	{
		[DebuggerHidden]
		public bool Else(Action doIfFalse)
		{
			if (!@this)
				doIfFalse();

			return @this;
		}

		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public T If<T>(T trueValue, T falseValue)
			=> @this ? trueValue : falseValue;

		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public T If<T>(Func<T> trueValue, T falseValue)
			=> @this ? trueValue() : falseValue;

		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public T If<T>(Func<T> trueValue, Func<T> falseValue)
			=> @this ? trueValue() : falseValue();

		[DebuggerHidden]
		public bool Then(Action doIfTrue)
		{
			if (@this)
				doIfTrue();

			return @this;
		}

		/// <remarks>
		/// <c>=&gt; @this ? [(<see cref="byte"/>)1] : [(<see cref="byte"/>)0];</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public byte[] ToBytes()
			=> @this ? [(byte)1] : [(byte)0];
	}
}
