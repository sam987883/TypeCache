// Copyright (c) 2021 Samuel Abraham

using System;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Data.Business
{
	internal class InsertValidationRule : IValidationRule<ISqlApi, InsertRequest>
	{
		public async ValueTask<ValidationResponse> ApplyAsync(ISqlApi sqlApi, InsertRequest request, CancellationToken cancellationToken)
		{
			return await Task.Run(() =>
			{
				try
				{
					var schema = sqlApi.GetObjectSchema(request.Into);
					schema.Type.Assert($"{nameof(InsertRequest)}.{nameof(request.Into)}", ObjectType.Table);

					if (!request.Insert.Any())
						throw new ArgumentException($"Columns are required for Insert.", $"{nameof(InsertRequest)}.{nameof(request.Insert)}");

					var invalidColumnCsv = request.Insert.Without(schema.Columns.To(column => column.Name)).ToCsv(column => $"[{column}]");
					if (!invalidColumnCsv.IsBlank())
						throw new ArgumentException($"Columns were not found on table [{request.Into}]: {invalidColumnCsv}", $"{nameof(InsertRequest)}.{nameof(request.Insert)}");

					if (!request.Select.Any())
						throw new ArgumentException($"Columns are required for Select.", $"{nameof(InsertRequest)}.{nameof(request.Select)}");

					if (request.Insert.Count() != request.Select.Count())
						throw new ArgumentException($"Must have same number of columns.", $"{nameof(InsertRequest)}.{nameof(request.Insert)}, {nameof(InsertRequest)}.{nameof(request.Select)}");

					var aliases = request.Select.To(_ => _.As).IfNotBlank();
					var uniqueAliases = aliases.ToHashSet(StringComparer.OrdinalIgnoreCase);
					if (aliases.Count() != uniqueAliases.Count)
						throw new ArgumentException($"Duplicate aliases found.", $"{nameof(InsertRequest)}.{nameof(request.Select)}");

					if (request.Output.Any())
					{
						aliases = request.Output.To(_ => _.As).IfNotBlank();
						uniqueAliases = aliases.ToHashSet(StringComparer.OrdinalIgnoreCase);
						if (aliases.Count() != uniqueAliases.Count)
							throw new ArgumentException($"Duplicate aliases found.", $"{nameof(InsertRequest)}.{nameof(request.Output)}");
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
