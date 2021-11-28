// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Mappers
{
	public interface IFieldToCsvMapper<in T>
		where T : notnull
	{
		string[] Map(params T[] rows);
	}
}
