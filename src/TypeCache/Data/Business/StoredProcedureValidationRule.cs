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
	internal class StoredProcedureValidationRule : IValidationRule<(ISqlApi SqlApi, StoredProcedureRequest Procedure)>
	{
		public async ValueTask ValidateAsync((ISqlApi SqlApi, StoredProcedureRequest Procedure) request, CancellationToken cancellationToken)
		{
			var schema = request.SqlApi.GetObjectSchema(request.Procedure.Procedure);
			schema.Type.Assert(nameof(StoredProcedureRequest), ObjectType.StoredProcedure);

			var invalidParameterCsv = request.Procedure.Parameters.Keys.Without(schema.Parameters.If(parameter => !parameter!.Return).To(parameter => parameter!.Name)).ToCsv();
			if (!invalidParameterCsv.IsBlank())
				throw new ArgumentException($"{schema.Name} does not have the following parameters: {invalidParameterCsv}.", $"{nameof(StoredProcedureRequest)}.{nameof(StoredProcedureRequest.Parameters)}");

			await ValueTask.CompletedTask;
		}
	}
}
