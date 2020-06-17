// Copyright (c) 2020 Samuel Abraham

using System.Collections.Immutable;

namespace sam987883.Reflection.Mappers
{
	public interface IFieldMapper<in FROM, in TO>
	{
		void Map(FROM from, TO to);

		IImmutableList<MapperSetting> Settings { get; }
	}

	public interface IPropertyMapper<in FROM, in TO>
	{
		void Map(FROM from, TO to);

		IImmutableList<MapperSetting> Settings { get; }
	}
}
