// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data;
using TypeCache.Extensions;

namespace TypeCache.Web.Business
{
	internal class DataRuleHandler<T> : IRuleHandler<DbConnection, T, RowSet>
	{
		private readonly IServiceProvider _ServiceProvider;

		public DataRuleHandler(IServiceProvider provider)
			=> this._ServiceProvider = provider;

		public async ValueTask<Response<RowSet>> HandleAsync(DbConnection connection, T request, CancellationToken cancellationToken = default)
		{
			var validationRules = this._ServiceProvider.GetServices<IValidationRule<T>>();
			if (validationRules.Any())
			{
				var results = await validationRules.To(async validationRule => await validationRule.ApplyAsync(request, cancellationToken)).AllAsync();
				if (!results.All(true))
					return new Response<RowSet>(validationRules);
			}

			try
			{
				await connection.OpenAsync();
				var rule = this._ServiceProvider.GetRequiredService<IRule<DbConnection, T, RowSet>>();
				var response = new Response<RowSet>(await rule.ApplyAsync(connection, request, cancellationToken));
				await connection.CloseAsync();
				return response;
			}
			catch (Exception exception)
			{
				return new Response<RowSet>(exception);
			}
			finally
			{
				await connection.DisposeAsync();
			}
		}
	}
}
