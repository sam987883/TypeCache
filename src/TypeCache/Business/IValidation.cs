// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Business
{
	public interface IValidation
	{
		bool IsError { get; }

		string[] Messages { get; }
	}
}
