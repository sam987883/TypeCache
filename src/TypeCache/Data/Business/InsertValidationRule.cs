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
	internal class InsertValidationRule : IValidationRule<InsertRequest>
	{
		private readonly ISqlApi _SqlApi;

		public InsertValidationRule(ISqlApi sqlApi)
		{
			this._SqlApi = sqlApi;
		}

		public async ValueTask ValidateAsync(InsertRequest request, CancellationToken cancellationToken)
		{
			var schema = this._SqlApi.GetObjectSchema(request.DataSource, request.Into);
			schema.Type.Assert($"{nameof(InsertRequest)}.{nameof(request.Into)}", ObjectType.Table);

			if (!request.Insert.Any())
				throw new ArgumentException($"Columns are required for Insert.", $"{nameof(InsertRequest)}.{nameof(InsertRequest.Insert)}");

			var columns = schema.Columns.To(column => column.Name).ToArray();
			var invalidColumnCsv = request.Insert.Without(columns).ToCSV(column => $"[{column}]");
			if (!invalidColumnCsv.IsBlank())
				throw new ArgumentException($"Columns were not found on table [{nameof(InsertRequest.Into)}]: {invalidColumnCsv}", $"{nameof(InsertRequest)}.{nameof(InsertRequest.Insert)}");

			if (!request.Select.Any())
				throw new ArgumentException($"Columns are required for Select.", $"{nameof(InsertRequest)}.{nameof(InsertRequest.Select)}");

			if (request.Insert.Count() != request.Select.Count())
				throw new ArgumentException($"Must have same number of columns.", $"{nameof(InsertRequest)}.{nameof(InsertRequest.Insert)}, {nameof(InsertRequest)}.{nameof(InsertRequest.Select)}");

			await ValueTask.CompletedTask;
		}
	}
}
