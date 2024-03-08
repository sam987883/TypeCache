// Copyright (c) 2021 Samuel Abraham

using System.Text;
using TypeCache.Utilities;

namespace TypeCache.Extensions;

public static class StringBuilderExtensions
{
	/// <summary>
	/// <c>=&gt; <paramref name="condition"/> ? @<paramref name="this"/>.Append(<paramref name="value"/>) : @<paramref name="this"/>;</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static StringBuilder AppendIf(this StringBuilder @this, bool condition, string value)
		=> condition ? @this.Append(value) : @this;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Append(<paramref name="condition"/> ? <paramref name="trueValue"/> : <paramref name="falseValue"/>);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static StringBuilder AppendIf(this StringBuilder @this, bool condition, string trueValue, string falseValue)
		=> @this.Append(condition ? trueValue : falseValue);

	/// <summary>
	/// <c>=&gt; <paramref name="condition"/> ? @<paramref name="this"/>.Append(<paramref name="value"/>) : @<paramref name="this"/>;</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static StringBuilder AppendIf(this StringBuilder @this, bool condition, ReadOnlySpan<char> value)
		=> condition ? @this.Append(value) : @this;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.Append(<paramref name="condition"/> ? <paramref name="trueValue"/> : <paramref name="falseValue"/>);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static StringBuilder AppendIf(this StringBuilder @this, bool condition, ReadOnlySpan<char> trueValue, ReadOnlySpan<char> falseValue)
		=> condition ? @this.Append(trueValue) : @this.Append(falseValue);

	/// <summary>
	/// <c>=&gt; <paramref name="condition"/> ? action(@<paramref name="this"/>) : @<paramref name="this"/>;</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static StringBuilder AppendIf(this StringBuilder @this, bool condition, Func<StringBuilder, StringBuilder> action)
		=> condition ? action(@this) : @this;

	[DebuggerHidden]
	public static StringBuilder AppendIf<T>(this StringBuilder @this, bool condition, T value)
		=> value switch
		{
			_ when !condition => @this,
			ReadOnlyMemory<char> item => @this.Append(item),
			char item => @this.Append(item),
			bool item => @this.Append(item),
			sbyte item => @this.Append(item),
			byte item => @this.Append(item),
			short item => @this.Append(item),
			ushort item => @this.Append(item),
			int item => @this.Append(item),
			uint item => @this.Append(item),
			long item => @this.Append(item),
			ulong item => @this.Append(item),
			float item => @this.Append(item),
			double item => @this.Append(item),
			decimal item => @this.Append(item),
			Enum item => @this.Append(item.ToString("F")),
			char[] item => @this.Append(item),
			string item => @this.Append(item),
			StringBuilder item => @this.Append(item),
			_ => @this.Append(value)
		};

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.AppendIf(<paramref name="condition"/>, <paramref name="trueValue"/>).AppendIf(!<paramref name="condition"/>,  <paramref name="falseValue"/>);</c>
	/// </summary>
	[DebuggerHidden]
	public static StringBuilder AppendIf<T>(this StringBuilder @this, bool condition, T trueValue, T falseValue)
		=> @this.AppendIf(condition, trueValue).AppendIf(!condition, falseValue);

	/// <summary>
	/// <c>=&gt; <paramref name="condition"/> ? @<paramref name="this"/>.AppendLine() : @<paramref name="this"/>;</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static StringBuilder AppendLineIf(this StringBuilder @this, bool condition)
		=> condition ? @this.AppendLine() : @this;

	/// <summary>
	/// <c>=&gt; <paramref name="condition"/> ? @<paramref name="this"/>.AppendLine(<paramref name="value"/>) : @<paramref name="this"/>;</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static StringBuilder AppendLineIf(this StringBuilder @this, bool condition, string value)
		=> condition ? @this.AppendLine(value) : @this;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.AppendLine(<paramref name="condition"/> ? <paramref name="trueValue"/> : <paramref name="falseValue"/>);</c>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static StringBuilder AppendLineIf(this StringBuilder @this, bool condition, string trueValue, string falseValue)
		=> @this.AppendLine(condition ? trueValue : falseValue);

	/// <inheritdoc cref="StringWriter.StringWriter(StringBuilder)"/>
	/// <remarks>
	/// <c>=&gt; <see langword="new"/> <see cref="StringWriter"/>(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static StringWriter ToStringWriter(this StringBuilder @this)
		=> new StringWriter(@this);

	/// <inheritdoc cref="StringWriter.StringWriter(StringBuilder, IFormatProvider?)"/>
	/// <remarks>
	/// <c>=&gt; <see langword="new"/> <see cref="StringWriter"/>(@<paramref name="this"/>, <paramref name="formatProvider"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static StringWriter ToStringWriter(this StringBuilder @this, IFormatProvider? formatProvider)
		=> new StringWriter(@this, formatProvider);

	/// <remarks>
	/// <c>=&gt; <see langword="new"/> <see cref="CustomStringWriter"/>(@<paramref name="this"/>, <paramref name="encoding"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static StringWriter ToStringWriter(this StringBuilder @this, Encoding encoding)
		=> new CustomStringWriter(@this, encoding);
}
