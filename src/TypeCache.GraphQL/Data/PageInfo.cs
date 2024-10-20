// Copyright (c) 2021 Samuel Abraham

using TypeCache.GraphQL.Attributes;

namespace TypeCache.GraphQL.Data;

[GraphQLDescription("Information about pagination in a connection.")]
public readonly record struct PageInfo(
	[GraphQLDescription("The first cursor value of the current page.")] long StartCursor
	, [GraphQLDescription("The last cursor value of the current page.")] long EndCursor
	, [GraphQLDescription("Whether a page exists before the current page.")] bool HasPreviousPage
	, [GraphQLDescription("Whether a page exists after the current page.")] bool HasNextPage)
{
	public PageInfo(long offset, long fetch, long totalCount)
		: this(offset + 1L, offset + fetch, offset > 0, (offset + fetch) < totalCount)
	{
	}
}
