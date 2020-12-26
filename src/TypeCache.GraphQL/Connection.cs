// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Extensions;

namespace TypeCache.GraphQL
{
	public struct Connection<T, C>
		where T : class
	{
		public Connection(T[] items, Func<T, C> getCursor, int totalCount, bool hasNextPage, bool hasPreviousPage)
		{
			this.Edges = items.To(item => new Edge<T, C>(item, getCursor)).ToArrayOf(items.Length);
			this.Items = items;
			this.PageInfo = new PageInfo<C>
			{
				StartCursor = getCursor(items.First()),
				EndCursor = items.Any() ? getCursor(items[items.Length - 1]) : default,
				HasNextPage = hasNextPage,
				HasPreviousPage = hasPreviousPage
			};
			this.TotalCount = totalCount;
		}

		public Edge<T, C>[] Edges { get; set; }

		public T[] Items { get; set; }

		public PageInfo<C> PageInfo { get; set; }

		public int TotalCount { get; set; }
	}
}
