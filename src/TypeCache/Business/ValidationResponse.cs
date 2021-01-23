// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Business
{
	public readonly struct ValidationResponse
	{
		public bool IsError { get; init; }

		public string[] Messages { get; init; }
	}
}
