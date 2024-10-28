// Copyright (c) 2021 Samuel Abraham

using System.Runtime.CompilerServices;

namespace TypeCache.Extensions;

public static class BooleanExtensions
{
	[DebuggerHidden]
	public static bool Else(this bool @this, Action doIfFalse)
	{
		if (!@this)
			doIfFalse();

		return @this;
	}

	[DebuggerHidden]
	public static bool Then(this bool @this, Action doIfTrue)
	{
		if (@this)
			doIfTrue();

		return @this;
	}
}
