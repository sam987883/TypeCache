// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;

namespace TypeCache.Business
{
    public interface IRuleHandler<in T, R>
    {
        Task<R[]> HandleAsync(T request, CancellationToken cancellationToken);
    }

    public interface IRuleHandler<in M, in T, R>
    {
        Task<R[]> HandleAsync(M metadata, T request, CancellationToken cancellationToken);
    }
}
