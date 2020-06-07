// Copyright (c) 2020 Samuel Abraham

namespace sam987883.Dependencies
{
	public interface INamedService<T>
	{
		string Name { get; }

		T Service { get; }
	}
}
