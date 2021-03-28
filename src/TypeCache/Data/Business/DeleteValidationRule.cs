// Copyright (c) 2021 Samuel Abraham

using System;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Data.Business
{
	internal class DeleteValidationRule : IValidationRule<ISqlApi, DeleteRequest>
	{
		public async ValueTask<ValidationResponse> ApplyAsync(ISqlApi sqlApi, DeleteRequest request, CancellationToken cancellationToken)
		{
			return await Task.Run(() =>
			{
				try
				{
					var schema = sqlApi.GetObjectSchema(request.From);
					schema.Type.Assert($"{nameof(DeleteRequest)}.{nameof(request.From)}", ObjectType.Table);

					if (request.Output.Any())
					{
						var aliases = request.Output.To(_ => _.As).IfNotBlank();
						var uniqueAliases = aliases.ToHashSet(StringComparer.OrdinalIgnoreCase);
						if (aliases.Count() != uniqueAliases.Count)
							throw new ArgumentException($"Duplicate aliases found.", $"{nameof(DeleteRequest)}.{nameof(request.Output)}");
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
