// Copyright (c) 2020 Samuel Abraham

using Sam987883.Reflection.Mappers;
using System.Collections.Immutable;

namespace Sam987883.Reflection
{
	public interface IFieldMapper<in FROM, in TO>
	{
		void Map(FROM from, TO to);

		IImmutableList<MapperSetting> Settings { get; }
	}
}
