// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Business
{
    public interface IValidationRule<in T> : IRule<T, bool>
    {
        string Message { get; }
    }
}
