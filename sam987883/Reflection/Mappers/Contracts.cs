// Copyright (c) 2020 Samuel Abraham

namespace sam987883.Reflection.Mappers
{
	public interface IFieldMapper<in FROM, in TO>
	{
		void Map(FROM from, TO to);
	}

	public interface IPropertyMapper<in FROM, in TO>
	{
		void Map(FROM from, TO to);
	}
}
