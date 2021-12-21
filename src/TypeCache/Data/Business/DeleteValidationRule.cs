// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Requests;
using TypeCache.Data.Schema;
using TypeCache.Extensions;

namespace TypeCache.Data.Business;

internal class DeleteValidationRule : IValidationRule<DeleteRequest>
{
	private readonly ISqlApi _SqlApi;

	public DeleteValidationRule(ISqlApi sqlApi)
	{
		this._SqlApi = sqlApi;
	}

	public async ValueTask ValidateAsync(DeleteRequest request, CancellationToken cancellationToken)
	{
		var schema = this._SqlApi.GetObjectSchema(request.DataSource, request.From);
		schema.Type.Assert(ObjectType.Table);

		await ValueTask.CompletedTask;
	}
}
