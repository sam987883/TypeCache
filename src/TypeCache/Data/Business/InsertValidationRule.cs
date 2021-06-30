// Copyright (c) 2021 Samuel Abraham

using System;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Schema;
using TypeCache.Extensions;

namespace TypeCache.Data.Business
{
	internal class InsertValidationRule : IValidationRule<(ISqlApi SqlApi, InsertRequest Insert)>
	{
		public async ValueTask ValidateAsync((ISqlApi SqlApi, InsertRequest Insert) request, CancellationToken cancellationToken)
		{
			var insert = request.Insert;

			var schema = request.SqlApi.GetObjectSchema(insert.Into);
			schema.Type.Assert($"{nameof(InsertRequest)}.{nameof(insert.Into)}", ObjectType.Table);

			if (!insert.Insert.Any())
				throw new ArgumentException($"Columns are required for Insert.", $"{nameof(InsertRequest)}.{nameof(InsertRequest.Insert)}");

			var invalidColumnCsv = insert.Insert.Without(schema.Columns.To(column => column.Name)).ToCsv(column => $"[{column}]");
			if (!invalidColumnCsv.IsBlank())
				throw new ArgumentException($"Columns were not found on table [{nameof(InsertRequest.Into)}]: {invalidColumnCsv}", $"{nameof(InsertRequest)}.{nameof(InsertRequest.Insert)}");

			if (!insert.Select.Any())
				throw new ArgumentException($"Columns are required for Select.", $"{nameof(InsertRequest)}.{nameof(InsertRequest.Select)}");

			if (insert.Insert.Count() != insert.Select.Count())
				throw new ArgumentException($"Must have same number of columns.", $"{nameof(InsertRequest)}.{nameof(InsertRequest.Insert)}, {nameof(InsertRequest)}.{nameof(InsertRequest.Select)}");

			await ValueTask.CompletedTask;
		}
	}
}
