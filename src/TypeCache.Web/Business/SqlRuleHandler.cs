// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Extensions;

namespace TypeCache.Web.Business
{
	internal class SqlRuleHandler<T> : IRuleHandler<DbConnection, T, string>
	{
		private readonly IServiceProvider _ServiceProvider;

		public SqlRuleHandler(IServiceProvider provider)
			=> this._ServiceProvider = provider;

		public async ValueTask<Response<string>> HandleAsync(DbConnection connection, T request, CancellationToken cancellationToken)
		{
			var validationRules = this._ServiceProvider.GetServices<IValidationRule<T>>();
			if (validationRules.Any())
			{
				var results = await validationRules.To(async validationRule => await validationRule.ApplyAsync(request, cancellationToken)).AllAsync();
				if (!results.All(true))
					return new Response<string>(validationRules);
			}

			try
			{
				await connection.OpenAsync();
				var rule = this._ServiceProvider.GetRequiredService<IRule<DbConnection, T, string>>();
				var response = new Response<string>(await rule.ApplyAsync(connection, request, cancellationToken));
				await connection.CloseAsync();
				return response;
			}
			catch (Exception exception)
			{
				return new Response<string>(exception);
			}
			finally
			{
				await connection.DisposeAsync();
			}
		}
	}
}
