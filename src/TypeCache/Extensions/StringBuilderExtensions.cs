// Copyright (c) 2021 Samuel Abraham

using System.Runtime.CompilerServices;
using System.Text;
using TypeCache.Utilities;

namespace TypeCache.Extensions;

public static class StringBuilderExtensions
{
	extension(StringBuilder @this)
	{
		/// <summary>
		/// <c>=&gt; <paramref name="condition"/> ? @this.Append(<paramref name="value"/>) : @this;</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public StringBuilder AppendIf(bool condition, ReadOnlySpan<char> value)
			=> condition ? @this.Append(value) : @this;

		/// <summary>
		/// <c>=&gt; @this.Append(<paramref name="condition"/> ? <paramref name="trueValue"/> : <paramref name="falseValue"/>);</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public StringBuilder AppendIf(bool condition, ReadOnlySpan<char> trueValue, ReadOnlySpan<char> falseValue)
			=> condition ? @this.Append(trueValue) : @this.Append(falseValue);

		/// <summary>
		/// <c>=&gt; <paramref name="condition"/> ? action(@this) : @this;</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public StringBuilder AppendIf(bool condition, Func<StringBuilder, StringBuilder> action)
			=> condition ? action(@this) : @this;

		[DebuggerHidden]
		public StringBuilder AppendIf<T>(bool condition, T value)
			where T : notnull
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
				Enum item => @this.Append(item.Name),
				char[] item => @this.Append(item),
				string item => @this.Append(item),
				StringBuilder item => @this.Append(item),
				_ => @this.Append(value)
			};

		/// <summary>
		/// <c>=&gt; @this.AppendIf(<see langword="true"/>, <paramref name="condition"/> ? <paramref name="trueValue"/> : <paramref name="falseValue"/>);</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public StringBuilder AppendIf<T>(bool condition, T trueValue, T falseValue)
			where T : notnull
			=> @this.AppendIf(true, condition ? trueValue : falseValue);

		/// <summary>
		/// <c>=&gt; <paramref name="condition"/> ? @this.AppendLine() : @this;</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public StringBuilder AppendLineIf(bool condition)
			=> condition ? @this.AppendLine() : @this;

		/// <summary>
		/// <c>=&gt; <paramref name="condition"/> ? @this.AppendLine(<paramref name="value"/>) : @this;</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public StringBuilder AppendLineIf(bool condition, string value)
			=> condition ? @this.AppendLine(value) : @this;

		/// <summary>
		/// <c>=&gt; @this.AppendLine(<paramref name="condition"/> ? <paramref name="trueValue"/> : <paramref name="falseValue"/>);</c>
		/// </summary>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public StringBuilder AppendLineIf(bool condition, string trueValue, string falseValue)
			=> @this.AppendLine(condition ? trueValue : falseValue);

		/// <inheritdoc cref="StringWriter.StringWriter(StringBuilder)"/>
		/// <remarks>
		/// <c>=&gt; <see langword="new"/> <see cref="StringWriter"/>(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public StringWriter ToStringWriter()
			=> new StringWriter(@this);

		/// <inheritdoc cref="StringWriter.StringWriter(StringBuilder, IFormatProvider?)"/>
		/// <remarks>
		/// <c>=&gt; <see langword="new"/> <see cref="StringWriter"/>(@this, <paramref name="formatProvider"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public StringWriter ToStringWriter(IFormatProvider? formatProvider)
			=> new StringWriter(@this, formatProvider);

		/// <remarks>
		/// <c>=&gt; <see langword="new"/> <see cref="CustomStringWriter"/>(@this, <paramref name="encoding"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public StringWriter ToStringWriter(Encoding encoding)
			=> new CustomStringWriter(@this, encoding);
	}
}
