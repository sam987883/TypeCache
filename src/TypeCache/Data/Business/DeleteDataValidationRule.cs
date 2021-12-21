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

internal class DeleteDataValidationRule : IValidationRule<DeleteDataRequest>
{
	private readonly ISqlApi _SqlApi;

	public DeleteDataValidationRule(ISqlApi sqlApi)
	{
		this._SqlApi = sqlApi;
	}

	public async ValueTask ValidateAsync(DeleteDataRequest request, CancellationToken cancellationToken)
	{
		var schema = this._SqlApi.GetObjectSchema(request.DataSource, request.From);
		schema.Type.Assert(ObjectType.Table);

		if (!request.Input.Columns.Has(schema.Columns.If(column => column.PrimaryKey).To(column => column.Name)))
			throw new ArgumentException($"{nameof(request.Input)}.{nameof(request.Input.Columns)} must contain all Primary Key column(s).", $"{nameof(DeleteDataRequest)}.{nameof(DeleteDataRequest.Input)}");

		await ValueTask.CompletedTask;
	}
}
