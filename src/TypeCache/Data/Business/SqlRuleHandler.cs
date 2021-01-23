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
	internal class SqlRuleHandler<T> : IRuleHandler<DbConnection, T, string>
	{
		private readonly IServiceProvider _ServiceProvider;

		public SqlRuleHandler(IServiceProvider provider)
			=> this._ServiceProvider = provider;

		public async ValueTask<Response<string>> HandleAsync(DbConnection dbConnection, T request, CancellationToken cancellationToken)
		{
			try
			{
				await dbConnection.OpenAsync();
				var validationResponses = (await this.ApplyValidationRules(this._ServiceProvider, dbConnection, request, cancellationToken))
					.Union(await this.ApplyValidationRules(this._ServiceProvider, request, cancellationToken))
					.ToArray();
				if (validationResponses.All(_ => !_.IsError))
				{
					var rule = this._ServiceProvider.GetRequiredService<IRule<DbConnection, T, string>>();
					var response = new Response<string>(await rule.ApplyAsync(dbConnection, request, cancellationToken));
					return response;
				}
				return new Response<string>(validationResponses);
			}
			catch (Exception exception)
			{
				return new Response<string>(exception);
			}
			finally
			{
				await dbConnection.CloseAsync();
				await dbConnection.DisposeAsync();
			}
		}
	}
}
