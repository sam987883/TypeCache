// Copyright (c) 2021 Samuel Abraham

using TypeCache.GraphQL.Attributes;

namespace TypeCache.GraphQL.Data;

[GraphQLDescription("Information about pagination in a connection.")]
public readonly record struct PageInfo(
	[GraphQLDescription("The first cursor value of the current page.")] int StartCursor
	, [GraphQLDescription("The last cursor value of the current page.")] int EndCursor
	, [GraphQLDescription("Whether a page exists before the current page.")] bool HasPreviousPage
	, [GraphQLDescription("Whether a page exists after the current page.")] bool HasNextPage)
{
	public PageInfo(uint offset, uint fetch, int totalCount)
		: this((int)offset + 1, (int)(offset + fetch), offset > 0, (offset + fetch) < totalCount)
	{
	}
}
