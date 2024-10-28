using System.Numerics;
using static System.Globalization.CultureInfo;

namespace TypeCache.Extensions;

public static partial class NumericExtensions
{
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.ToString("D", <paramref name="provider"/> ?? <see cref="InvariantCulture"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string ToText(this Guid @this, IFormatProvider? provider = null)
		=> @this.ToString("D", provider ?? InvariantCulture);
}
