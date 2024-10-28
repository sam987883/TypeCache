// Copyright (c) 2021 Samuel Abraham

using System;
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
	{
		var name = @this.Name.Replace("@", string.Empty);
		var index = name.IndexOf(GENERIC_TICKMARK);
		return index > -1 ? name.Left(index) : name;
	}
}
