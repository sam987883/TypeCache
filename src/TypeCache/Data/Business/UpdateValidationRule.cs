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

internal class UpdateValidationRule : IValidationRule<UpdateRequest>
{
	private readonly ISqlApi _SqlApi;

	public UpdateValidationRule(ISqlApi sqlApi)
	{
		this._SqlApi = sqlApi;
	}

	public async ValueTask ValidateAsync(UpdateRequest request, CancellationToken cancellationToken)
	{
		var schema = this._SqlApi.GetObjectSchema(request.DataSource, request.Table);
		schema.Type.Assert(ObjectType.Table);

		if (!request.Set.Any())
			throw new ArgumentException($"Columns are required.", $"{nameof(UpdateRequest)}.{nameof(UpdateRequest.Set)}");

		await ValueTask.CompletedTask;
	}
}
