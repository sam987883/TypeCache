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
	internal class CallRuleHandler : IRuleHandler<DbConnection, StoredProcedureRequest, StoredProcedureResponse>
	{
		private readonly IServiceProvider _ServiceProvider;

		public CallRuleHandler(IServiceProvider provider)
			=> this._ServiceProvider = provider;

		public async ValueTask<Response<StoredProcedureResponse>> HandleAsync(DbConnection dbConnection, StoredProcedureRequest request, CancellationToken cancellationToken)
		{
			try
			{
				await dbConnection.OpenAsync(cancellationToken);
				var validationResponses = (await this.ApplyValidationRules(this._ServiceProvider, dbConnection, request, cancellationToken))
					.Union(await this.ApplyValidationRules(this._ServiceProvider, request, cancellationToken))
					.ToArray();
				if (validationResponses.All(_ => !_.IsError))
				{
					var rule = this._ServiceProvider.GetRequiredService<IRule<DbConnection, StoredProcedureRequest, StoredProcedureResponse>>();
					var response = new Response<StoredProcedureResponse>(await rule.ApplyAsync(dbConnection, request, cancellationToken));
					return response;
				}
				return new Response<StoredProcedureResponse>(validationResponses);
			}
			catch (Exception exception)
			{
				return new Response<StoredProcedureResponse>(exception);
			}
			finally
			{
				await dbConnection.CloseAsync();
				await dbConnection.DisposeAsync();
			}
		}
	}
}
