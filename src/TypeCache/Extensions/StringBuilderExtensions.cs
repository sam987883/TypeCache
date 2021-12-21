// Copyright (c) 2021 Samuel Abraham

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using TypeCache.IO;
using static TypeCache.Default;

namespace TypeCache.Extensions;

public static class StringBuilderExtensions
{
	/// <summary>
	/// <c>=&gt; <see langword="new"/> <see cref="StringWriter"/>(@<paramref name="this"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static StringWriter ToStringWriter(this StringBuilder @this)
		=> new StringWriter(@this);

	/// <summary>
	/// <c>=&gt; <see langword="new"/> <see cref="StringWriter"/>(@<paramref name="this"/>, <paramref name="formatProvider"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static StringWriter ToStringWriter(this StringBuilder @this, IFormatProvider? formatProvider)
		=> new StringWriter(@this, formatProvider);

	/// <summary>
	/// <c>=&gt; <see langword="new"/> <see cref="CustomStringWriter"/>(@<paramref name="this"/>, <paramref name="encoding"/>);</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public static StringWriter ToStringWriter(this StringBuilder @this, Encoding encoding)
		=> new CustomStringWriter(@this, encoding);
}
