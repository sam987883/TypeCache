// Copyright (c) 2021 Samuel Abraham

using TypeCache.Collections;

namespace TypeCache.GraphQL.Extensions;

public class ConnectionSelections
{
	public bool TotalCount { get; set; }
	public bool HasNextPage { get; set; }
	public bool HasPreviousPage { get; set; }
	public bool Cursor { get; set; }
	public bool StartCursor { get; set; }
	public bool EndCursor { get; set; }
	public string[] EdgeNodeFields { get; set; } = Array<string>.Empty;
	public string[] ItemFields { get; set; } = Array<string>.Empty;
}
