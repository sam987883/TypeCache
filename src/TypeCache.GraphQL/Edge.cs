// Copyright (c) 2021 Samuel Abraham

using System;

namespace TypeCache.GraphQL
{
	public struct Edge<T, C>
		where T : class
	{
		public Edge(T item, Func<T, C> getCursor)
		{
			this.Cursor = getCursor(item);
			this.Node = item;
		}

		public C Cursor { get; set; }

		public T Node { get; set; }
	}
}
