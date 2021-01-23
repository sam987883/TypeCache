// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Business
{
	public interface IValidationRule<in T> : IRule<T, ValidationResponse>
	{
	}

	public interface IValidationRule<in M, in T> : IRule<M, T, ValidationResponse>
	{
	}
}
