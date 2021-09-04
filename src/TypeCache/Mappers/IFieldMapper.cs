﻿// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Mappers
{
	public interface IFieldMapper<in FROM, in TO>
		where FROM : notnull
		where TO : notnull
	{
		void Map(FROM from, TO to);
	}
}
