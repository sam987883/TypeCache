// Copyright (c) 2021 Samuel Abraham

using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Business;
using TypeCache.Extensions;

namespace TypeCache.Data.Business
{
	internal class DataRuleHandler<T> : IRuleHandler<DbConnection, T, RowSet>
	{
		private readonly IServiceProvider _ServiceProvider;

		public DataRuleHandler(IServiceProvider provider)
			=> this._ServiceProvider = provider;

		public async ValueTask<Response<RowSet>> HandleAsync(DbConnection dbConnection, T request, CancellationToken cancellationToken)
		{
			try
			{
				await dbConnection.OpenAsync(cancellationToken);
				var validationResponses = (await this.ApplyValidationRules(this._ServiceProvider, dbConnection, request, cancellationToken))
					.Union(await this.ApplyValidationRules(this._ServiceProvider, request, cancellationToken))
					.ToArray();
				if (validationResponses.All(_ => !_.IsError))
				{
					var rule = this._ServiceProvider.GetRequiredService<IRule<DbConnection, T, RowSet>>();
					var response = new Response<RowSet>(await rule.ApplyAsync(dbConnection, request, cancellationToken));
					return response;
				}
				return new Response<RowSet>(validationResponses);
			}
			catch (Exception exception)
			{
				return new Response<RowSet>(exception);
			}
			finally
			{
				await dbConnection.CloseAsync();
				await dbConnection.DisposeAsync();
			}
		}
	}
}
