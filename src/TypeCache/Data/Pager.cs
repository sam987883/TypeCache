// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Data
{
	public struct Pager
	{
		public Pager(uint first, uint after)
		{
			this.First = first;
			this.After = after;
		}

		public uint First { get; init; }

		public uint After { get; init; }
	}
}
