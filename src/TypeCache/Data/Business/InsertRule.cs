// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Extensions;

namespace TypeCache.Data.Business
{
	internal class InsertRule : IRule<ISqlApi, InsertRequest, RowSet>, IRule<InsertRequest, string>
	{
		async ValueTask<RowSet> IRule<ISqlApi, InsertRequest, RowSet>.ApplyAsync(ISqlApi sqlApi, InsertRequest request, CancellationToken cancellationToken)
		{
			request.From = sqlApi.GetObjectSchema(request.From).Name;
			request.Into = sqlApi.GetObjectSchema(request.Into).Name;
			return await sqlApi.InsertAsync(request, cancellationToken);
		}

		async ValueTask<string> IRule<InsertRequest, string>.ApplyAsync(InsertRequest request, CancellationToken cancellationToken)
			=> await ValueTask.FromResult(request.ToSql());
	}
}
