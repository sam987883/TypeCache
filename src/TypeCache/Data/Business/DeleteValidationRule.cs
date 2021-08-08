// Copyright (c) 2021 Samuel Abraham

using System;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Requests;
using TypeCache.Data.Schema;
using TypeCache.Extensions;

namespace TypeCache.Data.Business
{
	internal class DeleteValidationRule : IValidationRule<DeleteRequest>
	{
		private readonly ISqlApi _SqlApi;

		public DeleteValidationRule(ISqlApi sqlApi)
		{
			this._SqlApi = sqlApi;
		}

		public async ValueTask ValidateAsync(DeleteRequest request, CancellationToken cancellationToken)
		{
			var schema = this._SqlApi.GetObjectSchema(request.DataSource, request.From);
			schema.Type.Assert($"{nameof(DeleteRequest)}.{nameof(DeleteRequest.From)}", ObjectType.Table);

			var invalidColumnCsv = request.Output.Keys.Without(schema.Columns.To(column => column.Name)).ToCSV(column => $"[{column}]");
			if (!invalidColumnCsv.IsBlank())
				throw new ArgumentException($"{schema.Name} does not contain columns: {invalidColumnCsv}", $"{nameof(DeleteRequest)}.{nameof(DeleteRequest.Output)}");

			await ValueTask.CompletedTask;
		}
	}
}
