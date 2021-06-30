// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Extensions;

namespace TypeCache.Data.Business
{
	internal class SelectRule : IRule<(ISqlApi SqlApi, SelectRequest Select), RowSet>, IRule<SelectRequest, string>
	{
		async ValueTask<RowSet> IRule<(ISqlApi SqlApi, SelectRequest Select), RowSet>.ApplyAsync((ISqlApi SqlApi, SelectRequest Select) request, CancellationToken cancellationToken)
		{
			request.Select.From = request.SqlApi.GetObjectSchema(request.Select.From).Name;
			return await request.SqlApi.SelectAsync(request.Select, cancellationToken);
		}

		async ValueTask<string> IRule<SelectRequest, string>.ApplyAsync(SelectRequest request, CancellationToken cancellationToken)
			=> await ValueTask.FromResult(request.ToSQL());
	}
}
