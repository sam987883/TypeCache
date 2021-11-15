// Copyright (c) 2021 Samuel Abraham

using System;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Requests;
using TypeCache.Data.Schema;
using TypeCache.Extensions;

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
			var schema = this._SqlApi.GetObjectSchema(request.DataSource, request.From);
			if (schema.Type != ObjectType.Table && schema.Type != ObjectType.View && schema.Type != ObjectType.Function)
				throw new ArgumentOutOfRangeException(nameof(SelectRequest.From), $"Cannot SELECT from a {schema.Type.Name()}.");

			request.From = schema.Name;

			await ValueTask.CompletedTask;
		}
	}
}
