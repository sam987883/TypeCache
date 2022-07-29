// Copyright (c) 2021 Samuel Abraham

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using TypeCache.IO;
using static TypeCache.Default;

namespace TypeCache.Extensions;

public static class StringBuilderExtensions
{
	/// <inheritdoc cref="StringWriter.StringWriter(StringBuilder)"/>
	/// <remarks>
	/// <c>=&gt; <see langword="new"/> <see cref="StringWriter"/>(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static StringWriter ToStringWriter(this StringBuilder @this)
		=> new StringWriter(@this);

	/// <inheritdoc cref="StringWriter.StringWriter(StringBuilder, IFormatProvider?)"/>
	/// <remarks>
	/// <c>=&gt; <see langword="new"/> <see cref="StringWriter"/>(@<paramref name="this"/>, <paramref name="formatProvider"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static StringWriter ToStringWriter(this StringBuilder @this, IFormatProvider? formatProvider)
		=> new StringWriter(@this, formatProvider);

	/// <remarks>
	/// <c>=&gt; <see langword="new"/> <see cref="CustomStringWriter"/>(@<paramref name="this"/>, <paramref name="encoding"/>);</c>
	/// </remarks>
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static StringWriter ToStringWriter(this StringBuilder @this, Encoding encoding)
		=> new CustomStringWriter(@this, encoding);
}
