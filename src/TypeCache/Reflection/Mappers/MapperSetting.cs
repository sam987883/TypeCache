// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Reflection.Mappers
{
	public readonly struct MapperSetting
	{
		public string From { get; init; }

		public bool IgnoreNullValue { get; init; }

		public string To { get; init; }
	}
}
