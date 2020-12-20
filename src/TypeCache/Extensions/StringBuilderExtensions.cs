// Copyright(c) 2020 Samuel Abraham

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using TypeCache.Common;

namespace TypeCache.Extensions
{
	public static class StringBuilderExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static StringWriter ToStringWriter(this StringBuilder @this)
			=> new StringWriter(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static StringWriter ToStringWriter(this StringBuilder @this, IFormatProvider? formatProvider)
			=> new StringWriter(@this, formatProvider);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static StringWriter ToStringWriter(this StringBuilder @this, Encoding encoding)
			=> new CustomStringWriter(@this, encoding);
	}
}
