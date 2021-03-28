// Copyright (c) 2021 Samuel Abraham

using System;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Data.Business
{
	internal class MergeValidationRule : IValidationRule<ISqlApi, BatchRequest>
	{
		public async ValueTask<ValidationResponse> ApplyAsync(ISqlApi sqlApi, BatchRequest request, CancellationToken cancellationToken)
		{
			return await Task.Run(() =>
			{
				try
				{
					var schema = sqlApi.GetObjectSchema(request.Table);
					schema.Type.Assert($"{nameof(BatchRequest)}.{nameof(request.Table)}", ObjectType.Table);

					if (!request.Delete && !request.Update.Any() && !request.Insert.Any())
						throw new ArgumentException($"[{nameof(BatchRequest)}] must have either {nameof(request.Delete)} selected, {nameof(request.Insert)} columns or {nameof(request.Update)} columns.", nameof(request));

					if (request.Delete || request.Update.Any())
					{
						if (!request.Input.Columns.Any())
							throw new ArgumentException("Batch DELETE/UPDATE requires input columns.", $"{nameof(BatchRequest)}.{nameof(request.Input)}.{nameof(request.Input.Columns)}");

						var primaryKeys = schema.Columns.If(column => column!.PrimaryKey).To(column => column!.Name).ToList();
						if (!primaryKeys.Any())
							throw new ArgumentException($"Table {schema.Name} must have primary key(s) defined to use batch DELETE/UPDATE.", $"{nameof(BatchRequest)}.{nameof(request.Input)}.{nameof(request.Input.Columns)}");

						if (!request.Input.Columns.Has(primaryKeys, StringComparer.OrdinalIgnoreCase))
							throw new ArgumentException("Input columns must contain all primary keys to use batch DELETE/UPDATE.", $"{nameof(BatchRequest)}.{nameof(request.Input)}.{nameof(request.Input.Columns)}");
					}

					if (request.Insert.Any())
					{
						var invalidColumnCsv = request.Insert.Without(schema.Columns.To(column => column.Name)).ToCsv(column => $"[{column}]");
						if (!invalidColumnCsv.IsBlank())
							throw new ArgumentException($"Columns do not exist: {invalidColumnCsv}", $"{nameof(BatchRequest)}.{nameof(request.Insert)}");

						if (!request.Input.Columns.Any())
							throw new ArgumentException("Input columns are required for batch INSERT.", $"{nameof(BatchRequest)}.{nameof(request.Input)}.{nameof(request.Input.Columns)}");

						invalidColumnCsv = request.Insert.Without(request.Input.Columns, StringComparer.OrdinalIgnoreCase).ToCsv(_ => _.EscapeIdentifier());
						if (!invalidColumnCsv.IsBlank())
							throw new ArgumentException($"Column selections are not available from the input: {invalidColumnCsv}", $"{nameof(BatchRequest)}.{nameof(request.Insert)}");

						var writableColumns = schema.Columns.If(column => !column!.ReadOnly).To(column => column!.Name);
						invalidColumnCsv = request.Insert.Without(writableColumns).ToCsv(column => $"[{column}]");
						if (!invalidColumnCsv.IsBlank())
							throw new ArgumentException($"Column selections for table {schema.Name} contain non-writable columns: {invalidColumnCsv}", $"{nameof(BatchRequest)}.{nameof(request.Insert)}");
					}

					if (request.Update.Any())
					{
						var invalidColumnCsv = request.Update.Without(schema.Columns.To(column => column.Name)).ToCsv(column => $"[{column}]");
						if (!invalidColumnCsv.IsBlank())
							throw new ArgumentException($"Columns do not exist: {invalidColumnCsv}", $"{nameof(BatchRequest)}.{nameof(request.Update)}");

						invalidColumnCsv = request.Update.Without(request.Input.Columns, StringComparer.OrdinalIgnoreCase).ToCsv(_ => _.EscapeIdentifier());
						if (!invalidColumnCsv.IsBlank())
							throw new ArgumentException($"Column selections are not available from the input: {invalidColumnCsv}", $"{nameof(BatchRequest)}.{nameof(request.Update)}");

						var writableColumns = schema.Columns.If(column => !column!.ReadOnly).To(column => column!.Name);
						invalidColumnCsv = request.Update.Without(writableColumns).ToCsv(column => $"[{column}]");
						if (!invalidColumnCsv.IsBlank())
							throw new ArgumentException($"Column selections for table {schema.Name} contain non-writable columns: {invalidColumnCsv}", $"{nameof(BatchRequest)}.{nameof(request.Update)}");
					}

					if (!request.Input.Rows.Any())
						throw new ArgumentException("Input rows are required.", $"{nameof(BatchRequest)}.{nameof(request.Input)}.{nameof(request.Input.Rows)}");

					var invalidRowCount = request.Input.Rows.If(row => row!.Length != request.Input.Columns.Length).Count();
					if (invalidRowCount > 0)
						throw new ArgumentException($"{invalidRowCount} input rows have a different number of values than the {request.Input.Columns.Length} columns.", $"{nameof(BatchRequest)}.{nameof(request.Input)}.{nameof(request.Input.Rows)}");

					if (request.Output.Any())
					{
						var aliases = request.Output.To(_ => _.As).IfNotBlank();
						var uniqueAliases = aliases.ToHashSet(StringComparer.OrdinalIgnoreCase);
						if (aliases.Count() != uniqueAliases.Count)
							throw new ArgumentException($"Duplicate aliases found.", $"{nameof(BatchRequest)}.{nameof(request.Output)}");
					}

					return ValidationResponse.Success;
				}
				catch (Exception exception)
				{
					return new ValidationResponse(exception);
				}
			});
		}
	}
}
