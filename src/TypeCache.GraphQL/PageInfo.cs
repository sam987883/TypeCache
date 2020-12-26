// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.GraphQL
{
	public struct PageInfo<C>
	{
		public C EndCursor { get; set; }

		public bool HasNextPage { get; set; }

		public bool HasPreviousPage { get; set; }

		public C StartCursor { get; set; }
	}
}
