// Copyright (c) 2021 Samuel Abraham

using System.Reflection;

namespace TypeCache.Extensions;

partial class ReflectionExtensions
{
	[DebuggerHidden]
	public static MethodInfo ToMethodInfo(this Delegate @this)
		=> @this.GetType().GetMethod(nameof(Action.Invoke), INSTANCE_BINDING_FLAGS)!;
}
