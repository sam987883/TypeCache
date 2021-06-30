// Copyright (c) 2021 Samuel Abraham

using System;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Collections.Extensions;

namespace TypeCache.Data.Business
{
	internal class SelectValidationRule : IValidationRule<(ISqlApi SqlApi, SelectRequest Select)>
	{
		public async ValueTask ValidateAsync((ISqlApi SqlApi, SelectRequest Select) request, CancellationToken cancellationToken)
		{
			var schema = request.SqlApi.GetObjectSchema(request.Select.From);
			if (request.Select.OrderBy.Any() && !request.Select.Select.Keys.Has(request.Select.OrderBy.Keys))
				throw new ArgumentException($"All {nameof(SelectRequest.OrderBy)} Keys must also be in {nameof(SelectRequest.Select)} Keys.", nameof(SelectRequest));

			await ValueTask.CompletedTask;
		}
	}
}
