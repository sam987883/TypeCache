// Copyright (c) 2020 Samuel Abraham

namespace Sam987883.Dependencies
{
	public interface INamedService<T>
	{
		string Name { get; }

		T Service { get; }
	}
}
