// Copyright (c) 2021 Samuel Abraham

using System;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Extensions;
using TypeCache.Data.Schema;
using TypeCache.Extensions;

namespace TypeCache.Data.Business
{
	internal class MergeValidationRule : IValidationRule<(ISqlApi SqlApi, BatchRequest Batch)>
	{
		public async ValueTask ValidateAsync((ISqlApi SqlApi, BatchRequest Batch) request, CancellationToken cancellationToken)
		{
			var batch = request.Batch;

			var schema = request.SqlApi.GetObjectSchema(batch.Table);
			schema.Type.Assert($"{nameof(BatchRequest)}.{nameof(batch.Table)}", ObjectType.Table);

			if (!batch.Delete && !batch.Update.Any() && !batch.Insert.Any())
				throw new ArgumentException($"[{nameof(BatchRequest)}] must have either {nameof(batch.Delete)} selected, {nameof(batch.Insert)} columns or {nameof(batch.Update)} columns.", nameof(batch));

			if (batch.Delete || batch.Update.Any())
			{
				if (!batch.Input.Columns.Any())
					throw new ArgumentException("Batch DELETE/UPDATE requires input columns.", $"{nameof(BatchRequest)}.{nameof(batch.Input)}.{nameof(batch.Input.Columns)}");

				var primaryKeys = schema.Columns.If(column => column!.PrimaryKey).To(column => column!.Name).ToList();
				if (!primaryKeys.Any())
					throw new ArgumentException($"Table {schema.Name} must have primary key(s) defined to use batch DELETE/UPDATE.", $"{nameof(BatchRequest)}.{nameof(batch.Input)}.{nameof(batch.Input.Columns)}");

				if (!batch.Input.Columns.Has(primaryKeys, StringComparer.OrdinalIgnoreCase))
					throw new ArgumentException("Input columns must contain all primary keys to use batch DELETE/UPDATE.", $"{nameof(BatchRequest)}.{nameof(batch.Input)}.{nameof(batch.Input.Columns)}");
			}

			if (batch.Insert.Any())
			{
				var invalidColumnCsv = batch.Insert.Without(schema.Columns.To(column => column.Name)).ToCSV(column => $"[{column}]");
				if (!invalidColumnCsv.IsBlank())
					throw new ArgumentException($"Columns do not exist: {invalidColumnCsv}", $"{nameof(BatchRequest)}.{nameof(batch.Insert)}");

				if (!batch.Input.Columns.Any())
					throw new ArgumentException("Input columns are required for batch INSERT.", $"{nameof(BatchRequest)}.{nameof(batch.Input)}.{nameof(batch.Input.Columns)}");

				invalidColumnCsv = batch.Insert.Without(batch.Input.Columns, StringComparer.OrdinalIgnoreCase).ToCSV(_ => _.EscapeIdentifier());
				if (!invalidColumnCsv.IsBlank())
					throw new ArgumentException($"Column selections are not available from the input: {invalidColumnCsv}", $"{nameof(BatchRequest)}.{nameof(batch.Insert)}");

				var writableColumns = schema.Columns.If(column => !column!.ReadOnly).To(column => column!.Name);
				invalidColumnCsv = batch.Insert.Without(writableColumns).ToCSV(column => $"[{column}]");
				if (!invalidColumnCsv.IsBlank())
					throw new ArgumentException($"Column selections for table {schema.Name} contain non-writable columns: {invalidColumnCsv}", $"{nameof(BatchRequest)}.{nameof(batch.Insert)}");
			}

			if (batch.Update.Any())
			{
				var invalidColumnCsv = batch.Update.Without(schema.Columns.To(column => column.Name)).ToCSV(column => $"[{column}]");
				if (!invalidColumnCsv.IsBlank())
					throw new ArgumentException($"Columns do not exist: {invalidColumnCsv}", $"{nameof(BatchRequest)}.{nameof(batch.Update)}");

				invalidColumnCsv = batch.Update.Without(batch.Input.Columns, StringComparer.OrdinalIgnoreCase).ToCSV(_ => _.EscapeIdentifier());
				if (!invalidColumnCsv.IsBlank())
					throw new ArgumentException($"Column selections are not available from the input: {invalidColumnCsv}", $"{nameof(BatchRequest)}.{nameof(batch.Update)}");

				var writableColumns = schema.Columns.If(column => !column!.ReadOnly).To(column => column!.Name);
				invalidColumnCsv = batch.Update.Without(writableColumns).ToCSV(column => $"[{column}]");
				if (!invalidColumnCsv.IsBlank())
					throw new ArgumentException($"Column selections for table {schema.Name} contain non-writable columns: {invalidColumnCsv}", $"{nameof(BatchRequest)}.{nameof(batch.Update)}");
			}

			if (!batch.Input.Rows.Any())
				throw new ArgumentException("Input rows are required.", $"{nameof(BatchRequest)}.{nameof(batch.Input)}.{nameof(batch.Input.Rows)}");

			var invalidRowCount = batch.Input.Rows.If(row => row!.Length != batch.Input.Columns.Length).Count();
			if (invalidRowCount > 0)
				throw new ArgumentException($"{invalidRowCount} input rows have a different number of values than the {batch.Input.Columns.Length} columns.", $"{nameof(BatchRequest)}.{nameof(batch.Input)}.{nameof(batch.Input.Rows)}");

			await ValueTask.CompletedTask;
		}
	}
}
