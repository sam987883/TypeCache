// Copyright (c) 2021 Samuel Abraham

using System;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Requests;

namespace TypeCache.Data.Business
{
	internal class SelectValidationRule : IValidationRule<SelectRequest>
	{
		private readonly ISqlApi _SqlApi;

		public SelectValidationRule(ISqlApi sqlApi)
		{
			this._SqlApi = sqlApi;
		}

		public async ValueTask ValidateAsync(SelectRequest request, CancellationToken cancellationToken)
		{
			request.Schema = this._SqlApi.GetObjectSchema(request.DataSource, request.From);
			if (request.OrderBy.Any() && !request.Select.Keys.Has(request.OrderBy.To(_ => _.Item1)))
				throw new ArgumentException($"All {nameof(SelectRequest.OrderBy)} Keys must also be in {nameof(SelectRequest.Select)} Keys.", nameof(SelectRequest));

			await ValueTask.CompletedTask;
		}
	}
}
