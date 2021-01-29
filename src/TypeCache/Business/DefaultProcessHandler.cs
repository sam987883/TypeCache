// Copyright (c) 2021 Samuel Abraham

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Business.Extensions;

namespace TypeCache.Business
{
	public class DefaultProcessHandler<T> : IProcessHandler<T>
	{
		private readonly IServiceProvider _ServiceProvider;

		public DefaultProcessHandler(IServiceProvider serviceProvider)
			=> this._ServiceProvider = serviceProvider;

		public async ValueTask HandleAsync(T request, CancellationToken cancellationToken)
		{
			await this.ApplyValidationRules(this._ServiceProvider, request, cancellationToken);
			var process = this._ServiceProvider.GetRequiredService<IProcess<T>>();
			await process.RunAsync(request, cancellationToken);
		}
	}

	public class DefaultProcessHandler<M, T> : IProcessHandler<M, T>
	{
		private readonly IServiceProvider _ServiceProvider;

		public DefaultProcessHandler(IServiceProvider provider)
			=> this._ServiceProvider = provider;

		public async ValueTask HandleAsync(M metadata, T request, CancellationToken cancellationToken)
		{
			await this.ApplyValidationRules(this._ServiceProvider, request, cancellationToken);
			await this.ApplyValidationRules(this._ServiceProvider, metadata, request, cancellationToken);
			var process = this._ServiceProvider.GetRequiredService<IProcess<M, T>>();
			await process.RunAsync(metadata, request, cancellationToken);
		}
	}
}
