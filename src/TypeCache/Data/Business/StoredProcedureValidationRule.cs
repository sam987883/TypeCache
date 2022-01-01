// Copyright (c) 2021 Samuel Abraham

using System;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Requests;
using TypeCache.Data.Schema;
using TypeCache.Extensions;

namespace TypeCache.Data.Business;

internal class StoredProcedureValidationRule : IValidationRule<StoredProcedureRequest>
{
	private readonly ISqlApi _SqlApi;

	public StoredProcedureValidationRule(ISqlApi sqlApi)
	{
		this._SqlApi = sqlApi;
	}

	public async ValueTask ValidateAsync(StoredProcedureRequest request, CancellationToken cancellationToken)
	{
		var schema = this._SqlApi.GetObjectSchema(request.DataSource, request.Procedure);
		schema.Type.Assert(ObjectType.StoredProcedure);

		var invalidParameterCsv = request.Parameters.Keys.Without(schema.Parameters.If(parameter => !parameter!.Return).To(parameter => parameter!.Name)).ToCSV();
		if (invalidParameterCsv.IsNotBlank())
			throw new ArgumentException($"{schema.Name} does not have the following parameters: {invalidParameterCsv}.", $"{nameof(StoredProcedureRequest)}.{nameof(StoredProcedureRequest.Parameters)}");

		await ValueTask.CompletedTask;
	}
}
