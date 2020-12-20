// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;

namespace TypeCache.Business
{
    public interface IMediator
    {
        Task RunAsync<T>(T request, CancellationToken cancellationToken = default);

        Task RunAsync<M, T>(M metadata, T request, CancellationToken cancellationToken = default);

        Task<R[]> SendAsync<T, R>(T request, CancellationToken cancellationToken = default);

        Task<R[]> SendAsync<M, T, R>(M metadata, T request, CancellationToken cancellationToken = default);
    }
}
