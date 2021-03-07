// Copyright (c) 2021 Samuel Abraham

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Business;
using TypeCache.Business.Extensions;
using TypeCache.Collections.Extensions;

namespace TypeCache.Data.Business
{
	internal class DataRuleHandler<T> : IRuleHandler<ISqlApi, T, RowSet>
	{
		private readonly IServiceProvider _ServiceProvider;

		public DataRuleHandler(IServiceProvider provider)
			=> this._ServiceProvider = provider;

		public async ValueTask<Response<RowSet>> HandleAsync(ISqlApi sqlApi, T request, CancellationToken cancellationToken)
		{
			try
			{
				var validationResponses = (await this.ApplyValidationRules(this._ServiceProvider, sqlApi, request, cancellationToken))
					.Union(await this.ApplyValidationRules(this._ServiceProvider, request, cancellationToken))
					.ToArray();
				if (validationResponses.All(_ => !_.IsError))
				{
					var rule = this._ServiceProvider.GetRequiredService<IRule<ISqlApi, T, RowSet>>();
					var response = new Response<RowSet>(await rule.ApplyAsync(sqlApi, request, cancellationToken));
					return response;
				}
				return new Response<RowSet>(validationResponses);
			}
			catch (Exception exception)
			{
				return new Response<RowSet>(exception);
			}
		}
	}
}
