// Copyright (c) 2021 Samuel Abraham

using System;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Data.Business
{
	internal class StoredProcedureValidationRule : IValidationRule<ISqlApi, StoredProcedureRequest>
	{
		public async ValueTask<ValidationResponse> ApplyAsync(ISqlApi sqlApi, StoredProcedureRequest request, CancellationToken cancellationToken)
		{
			return await Task.Run(() =>
			{
				try
				{
					var schema = sqlApi.GetObjectSchema(request.Procedure);
					schema.Type.Assert(nameof(StoredProcedureRequest), ObjectType.StoredProcedure);

					var invalidParameterCsv = request.Parameters.To(_ => _.Name).Without(schema.Parameters.If(parameter => !parameter!.Return).To(parameter => parameter!.Name)).ToCsv();
					if (!invalidParameterCsv.IsBlank())
						throw new ArgumentException($"{schema.Name} does not have the following parameters: {invalidParameterCsv}.", $"{nameof(StoredProcedureRequest)}.{nameof(request.Parameters)}");

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
