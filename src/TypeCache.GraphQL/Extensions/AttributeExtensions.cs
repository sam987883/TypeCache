// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;

namespace TypeCache.GraphQL.Extensions;

public static class AttributeExtensions
{
	extension(IEnumerable<Attribute> @this)
	{
		[DebuggerHidden]
		public string? GraphQLDeprecationReason => @this.OfType<GraphQLDeprecationReasonAttribute>().FirstOrDefault()?.DeprecationReason?.IfBlank(null);

		[DebuggerHidden]
		public string? GraphQLDescription => @this.OfType<GraphQLDescriptionAttribute>().FirstOrDefault()?.Description?.IfBlank(null);

		[DebuggerHidden]
		public bool GraphQLIgnore => @this.Any<GraphQLIgnoreAttribute>();

		[DebuggerHidden]
		public string? GraphQLInputName => @this.OfType<GraphQLInputNameAttribute>().FirstOrDefault()?.Name?.IfBlank(null);

		[DebuggerHidden]
		public bool GraphQLMutation => @this.Any<GraphQLMutationAttribute>();

		[DebuggerHidden]
		public string? GraphQLName => @this.OfType<GraphQLNameAttribute>().FirstOrDefault()?.Name?.IfBlank(null);

		[DebuggerHidden]
		public bool GraphQLQuery => @this.Any<GraphQLQueryAttribute>();

		[DebuggerHidden]
		public bool GraphQLSubscription => @this.Any<GraphQLSubscriptionAttribute>();

		[DebuggerHidden]
		public Type? GraphQLType => @this.OfType<GraphQLTypeAttribute>().FirstOrDefault()?.Type;
	}
}
