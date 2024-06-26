﻿// Copyright (c) 2021 Samuel Abraham

using System.Reflection;

namespace TypeCache.Extensions;

public partial class ReflectionExtensions
{
	/// <remarks>
	/// <c>@<paramref name="this"/>.GetCustomAttribute&lt;<typeparamref name="T"/>&gt;(<paramref name="inherit"/>) <see langword="is not null"/>;</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool HasCustomAttribute<T>(this MemberInfo @this, bool inherit = true)
		where T : Attribute
		=> @this.GetCustomAttribute<T>(inherit) is not null;

	/// <remarks>
	/// <c>@<paramref name="this"/>.GetCustomAttribute(<paramref name="attributeType"/>, <paramref name="inherit"/>) <see langword="is not null"/>;</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool HasCustomAttribute(this MemberInfo @this, Type attributeType, bool inherit = true)
		=> @this.GetCustomAttribute(attributeType, inherit) is not null;

	[DebuggerHidden]
	public static string Name(this MemberInfo @this)
		=> @this.Name.IndexOf(GENERIC_TICKMARK) switch
		{
			var index when index >= 0 => @this.Name.Left(index),
			_ => @this.Name
		};
}
