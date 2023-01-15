// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using System.Reflection;
using TypeCache.Attributes;

namespace TypeCache.Extensions;

partial class ReflectionExtensions
{
	/// <remarks>
	/// <c>@<paramref name="this"/>.GetCustomAttribute&lt;<typeparamref name="T"/>&gt;(<paramref name="inherit"/>) <see langword="is not null"/>;</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool HasCustomAttribute<T>(this ParameterInfo @this, bool inherit = true)
		where T : Attribute
		=> @this.GetCustomAttribute<T>(inherit) is not null;

	/// <remarks>
	/// <c>@<paramref name="this"/>.GetCustomAttribute(<paramref name="attributeType"/>, <paramref name="inherit"/>) <see langword="is not null"/>;</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool HasCustomAttribute(this ParameterInfo @this, Type attributeType, bool inherit = true)
		=> @this.GetCustomAttribute(attributeType, inherit) is not null;

	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.GetCustomAttribute&lt;<see cref="NameAttribute"/>&gt;()?.Name<br/>
	/// <see langword="    "/>?? @<paramref name="this"/>.Name.Left(@<paramref name="this"/>.Name.IndexOf('`'))<br/>
	/// <see langword="    "/>?? Invariant($"Parameter{@<paramref name="this"/>.Position}");</c>
	/// </summary>
	[DebuggerHidden]
	public static string Name(this ParameterInfo @this)
		=> @this.GetCustomAttribute<NameAttribute>()?.Name
			?? @this.Name?.Left(@this.Name.IndexOf(GENERIC_TICKMARK))
			?? Invariant($"Parameter{@this.Position}");

	/// <inheritdoc cref="Expression.Parameter(Type, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Parameter(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ParameterExpression ToParameterExpression(this ParameterInfo @this)
		=> Expression.Parameter(@this.ParameterType, @this.Name);
}
