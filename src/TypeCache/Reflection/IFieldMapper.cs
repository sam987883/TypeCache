// Copyright (c) 2021 Samuel Abraham

using TypeCache.Reflection.Mappers;
using System.Collections.Immutable;

namespace TypeCache.Reflection
{
	public interface IFieldMapper<in FROM, in TO>
	{
		void Map(FROM from, TO to);

		IImmutableList<MapperSetting> Settings { get; }
	}
}
