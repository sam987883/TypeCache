// Copyright (c) 2021 Samuel Abraham

using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Business;
using TypeCache.Business.Extensions;
using TypeCache.Collections.Extensions;

namespace TypeCache.Data.Business
{
	internal class ExecuteSqlRuleHandler : IRuleHandler<DbConnection, SqlRequest, RowSet[]>
	{
		private readonly IServiceProvider _ServiceProvider;

		public ExecuteSqlRuleHandler(IServiceProvider provider)
			=> this._ServiceProvider = provider;

		async ValueTask<Response<RowSet[]>> IRuleHandler<DbConnection, SqlRequest, RowSet[]>.HandleAsync(DbConnection dbConnection, SqlRequest request, CancellationToken cancellationToken)
		{
			var validationResponses = (await this.ApplyValidationRules(this._ServiceProvider, dbConnection, request, cancellationToken))
				.Union(await this.ApplyValidationRules(this._ServiceProvider, request, cancellationToken))
				.ToArray();
			if (!validationResponses.Any(_ => _.IsError))
				return new Response<RowSet[]>(validationResponses);

			try
			{
				await dbConnection.OpenAsync(cancellationToken);
				var rule = this._ServiceProvider.GetRequiredService<IRule<DbConnection, SqlRequest, RowSet[]>>();
				var response = new Response<RowSet[]>(await rule.ApplyAsync(dbConnection, request, cancellationToken));
				await dbConnection.CloseAsync();
				return response;
			}
			catch (Exception exception)
			{
				return new Response<RowSet[]>(exception);
			}
			finally
			{
				await dbConnection.DisposeAsync();
			}
		}
	}
}
