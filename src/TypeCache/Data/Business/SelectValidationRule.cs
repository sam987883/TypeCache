// Copyright (c) 2021 Samuel Abraham

using System;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Collections.Extensions;

namespace TypeCache.Data.Business
{
	internal class SelectValidationRule : IValidationRule<ISqlApi, SelectRequest>
	{
		public async ValueTask<ValidationResponse> ApplyAsync(ISqlApi sqlApi, SelectRequest request, CancellationToken cancellationToken)
		{
			return await Task.Run(() =>
			{
				try
				{
					var schema = sqlApi.GetObjectSchema(request.From);

					if (request.Select.Any())
					{
						var aliases = request.Select.To(_ => _.As).IfNotBlank();
						var uniqueAliases = aliases.ToHashSet(StringComparer.OrdinalIgnoreCase);
						if (aliases.Count() != uniqueAliases.Count)
							throw new ArgumentException($"Duplicate aliases found.", $"{nameof(SelectRequest)}.{nameof(request.Select)}");
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
