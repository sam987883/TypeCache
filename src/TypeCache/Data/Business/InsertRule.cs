// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Extensions;

namespace TypeCache.Data.Business
{
	internal class InsertRule : IRule<(ISqlApi SqlApi, InsertRequest Insert), RowSet>, IRule<InsertRequest, string>
	{
		async ValueTask<RowSet> IRule<(ISqlApi SqlApi, InsertRequest Insert), RowSet>.ApplyAsync((ISqlApi SqlApi, InsertRequest Insert) request, CancellationToken cancellationToken)
		{
			request.Insert.From = request.SqlApi.GetObjectSchema(request.Insert.From).Name;
			request.Insert.Into = request.SqlApi.GetObjectSchema(request.Insert.Into).Name;
			return await request.SqlApi.InsertAsync(request.Insert, cancellationToken);
		}

		async ValueTask<string> IRule<InsertRequest, string>.ApplyAsync(InsertRequest request, CancellationToken cancellationToken)
			=> await ValueTask.FromResult(request.ToSQL());
	}
}
