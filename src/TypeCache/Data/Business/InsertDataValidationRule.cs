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
	internal class InsertDataValidationRule : IValidationRule<InsertDataRequest>
	{
		private readonly ISqlApi _SqlApi;

		public InsertDataValidationRule(ISqlApi sqlApi)
		{
			this._SqlApi = sqlApi;
		}

		public async ValueTask ValidateAsync(InsertDataRequest request, CancellationToken cancellationToken)
		{
			var schema = this._SqlApi.GetObjectSchema(request.DataSource, request.Into);
			schema.Type.Assert($"{nameof(InsertDataRequest)}.{nameof(request.Into)}", ObjectType.Table);

			var invalidColumnCsv = request.Input.Columns.Without(schema.Columns.If(column => !column.Identity && !column.ReadOnly).To(column => column.Name)).ToCSV(column => $"[{column}]");
			if (!invalidColumnCsv.IsBlank())
				throw new ArgumentException($"{nameof(request.Input)}.{nameof(request.Input.Columns)} contains non-writable columns: {invalidColumnCsv}.", $"{nameof(InsertDataRequest)}.{nameof(InsertDataRequest.Input)}");

			await ValueTask.CompletedTask;
		}
	}
}
