// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;

namespace TypeCache.GraphQL.Extensions;

public static class AttributeExtensions
{
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string? GraphQLDeprecationReason(this IEnumerable<Attribute> @this)
		=> @this.OfType<GraphQLDeprecationReasonAttribute>().FirstOrDefault()?.DeprecationReason?.NullIfBlank();

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string? GraphQLDescription(this IEnumerable<Attribute> @this)
		=> @this.OfType<GraphQLDescriptionAttribute>().FirstOrDefault()?.Description?.NullIfBlank();

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool GraphQLIgnore(this IEnumerable<Attribute> @this)
		=> @this.Any<GraphQLIgnoreAttribute>();

	public static string? GraphQLInputName(this IEnumerable<Attribute> @this)
	{
		var name = @this.OfType<GraphQLInputNameAttribute>().FirstOrDefault()?.Name?.NullIfBlank();
		if (name.IsNotBlank())
			return name;

		name = @this.GraphQLName();
		if (name.IsNotBlank())
			return Invariant($"{@this.GraphQLName()}Input");

		return null;
	}

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string? GraphQLName(this IEnumerable<Attribute> @this)
		=> @this.OfType<GraphQLNameAttribute>().FirstOrDefault()?.Name?.NullIfBlank();

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Type? GraphQLType(this IEnumerable<Attribute> @this)
		=> @this.OfType<GraphQLTypeAttribute>().FirstOrDefault()?.Type;
}
