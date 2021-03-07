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
	internal class ExecuteSqlRuleHandler : IRuleHandler<ISqlApi, SqlRequest, RowSet[]>
	{
		private readonly IServiceProvider _ServiceProvider;

		public ExecuteSqlRuleHandler(IServiceProvider provider)
			=> this._ServiceProvider = provider;

		async ValueTask<Response<RowSet[]>> IRuleHandler<ISqlApi, SqlRequest, RowSet[]>.HandleAsync(ISqlApi sqlApi, SqlRequest request, CancellationToken cancellationToken)
		{
			var validationResponses = (await this.ApplyValidationRules(this._ServiceProvider, sqlApi, request, cancellationToken))
				.Union(await this.ApplyValidationRules(this._ServiceProvider, request, cancellationToken))
				.ToArray();
			if (!validationResponses.Any(_ => _.IsError))
				return new Response<RowSet[]>(validationResponses);

			try
			{
				var rule = this._ServiceProvider.GetRequiredService<IRule<ISqlApi, SqlRequest, RowSet[]>>();
				var response = new Response<RowSet[]>(await rule.ApplyAsync(sqlApi, request, cancellationToken));
				return response;
			}
			catch (Exception exception)
			{
				return new Response<RowSet[]>(exception);
			}
		}
	}
}
