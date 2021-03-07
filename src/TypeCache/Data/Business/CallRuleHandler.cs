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
	internal class CallRuleHandler : IRuleHandler<ISqlApi, StoredProcedureRequest, StoredProcedureResponse>
	{
		private readonly IServiceProvider _ServiceProvider;

		public CallRuleHandler(IServiceProvider provider)
			=> this._ServiceProvider = provider;

		public async ValueTask<Response<StoredProcedureResponse>> HandleAsync(ISqlApi sqlApi, StoredProcedureRequest request, CancellationToken cancellationToken)
		{
			try
			{
				var validationResponses = (await this.ApplyValidationRules(this._ServiceProvider, sqlApi, request, cancellationToken))
					.Union(await this.ApplyValidationRules(this._ServiceProvider, request, cancellationToken))
					.ToArray();
				if (validationResponses.All(_ => !_.IsError))
				{
					var rule = this._ServiceProvider.GetRequiredService<IRule<ISqlApi, StoredProcedureRequest, StoredProcedureResponse>>();
					var response = new Response<StoredProcedureResponse>(await rule.ApplyAsync(sqlApi, request, cancellationToken));
					return response;
				}
				return new Response<StoredProcedureResponse>(validationResponses);
			}
			catch (Exception exception)
			{
				return new Response<StoredProcedureResponse>(exception);
			}
		}
	}
}
