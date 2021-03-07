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
	internal class SqlRuleHandler<T> : IRuleHandler<T, string>
	{
		private readonly IServiceProvider _ServiceProvider;

		public SqlRuleHandler(IServiceProvider provider)
			=> this._ServiceProvider = provider;

		public async ValueTask<Response<string>> HandleAsync(T request, CancellationToken cancellationToken)
		{
			try
			{
				var validationResponses = (await this.ApplyValidationRules(this._ServiceProvider, request, cancellationToken)).ToArray();
				if (validationResponses.All(_ => !_.IsError))
				{
					var rule = this._ServiceProvider.GetRequiredService<IRule<T, string>>();
					var response = new Response<string>(await rule.ApplyAsync(request, cancellationToken));
					return response;
				}
				return new Response<string>(validationResponses);
			}
			catch (Exception exception)
			{
				return new Response<string>(exception);
			}
		}
	}
}
