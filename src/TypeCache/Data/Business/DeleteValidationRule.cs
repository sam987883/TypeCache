// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Schema;
using TypeCache.Extensions;

namespace TypeCache.Data.Business
{
	internal class DeleteValidationRule : IValidationRule<(ISqlApi SqlApi, DeleteRequest Delete)>
	{
		public async ValueTask ValidateAsync((ISqlApi SqlApi, DeleteRequest Delete) request, CancellationToken cancellationToken)
			=> await Task.Run(() => request.SqlApi.GetObjectSchema(request.Delete.From).Type.Assert($"{nameof(DeleteRequest)}.{nameof(DeleteRequest.From)}", ObjectType.Table));
	}
}
