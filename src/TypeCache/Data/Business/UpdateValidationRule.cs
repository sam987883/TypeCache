// Copyright (c) 2021 Samuel Abraham

using System;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Data.Business
{
	internal class UpdateValidationRule : IValidationRule<ISqlApi, UpdateRequest>
	{
		public async ValueTask<ValidationResponse> ApplyAsync(ISqlApi sqlApi, UpdateRequest request, CancellationToken cancellationToken)
		{
			return await Task.Run(() =>
			{
				try
				{
					var schema = sqlApi.GetObjectSchema(request.Table);
					schema.Type.Assert($"{nameof(UpdateRequest)}.{nameof(request.Table)}", ObjectType.Table);

					if (request.Output.Any())
					{
						var aliases = request.Output.To(_ => _.As).IfNotBlank();
						var uniqueAliases = aliases.ToHashSet(StringComparer.OrdinalIgnoreCase);
						if (aliases.Count() != uniqueAliases.Count)
							throw new ArgumentException($"Duplicate aliases found.", $"{nameof(UpdateRequest)}.{nameof(request.Output)}");
					}

					if (!request.Set.Any())
						throw new ArgumentException($"Columns are required.", $"{nameof(UpdateRequest)}.{nameof(request.Set)}");

					var invalidColumnCsv = request.Set.To(set => set.Column).Without(schema.Columns.To(column => column.Name)).ToCsv(column => $"[{column}]");
					if (!invalidColumnCsv.IsBlank())
						throw new ArgumentException($"{schema.Name} does not contain columns: {invalidColumnCsv}", $"{nameof(UpdateRequest)}.{nameof(request.Set)}");

					var writableColumns = schema.Columns.If(column => !column!.ReadOnly).To(column => column!.Name);
					invalidColumnCsv = request.Set.To(set => set.Column).Without(writableColumns).ToCsv(column => $"[{column}]");
					if (!invalidColumnCsv.IsBlank())
						throw new ArgumentException($"{schema.Name} columns are read-only: {invalidColumnCsv}", $"{nameof(UpdateRequest)}.{nameof(request.Set)}");

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
