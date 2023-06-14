namespace TypeCache.Extensions;

public static class BooleanExtensions
{
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool Else(this bool @this, Action doIfFalse)
		=> (!@this).Then(doIfFalse);

	[DebuggerHidden]
	public static bool Then(this bool @this, Action doIfTrue)
	{
		if (@this)
			doIfTrue();

		return @this;
	}
}
