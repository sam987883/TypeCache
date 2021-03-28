// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Extensions;

namespace TypeCache.Data.Business
{
	internal class SelectRule : IRule<ISqlApi, SelectRequest, RowSet>, IRule<SelectRequest, string>
	{
		async ValueTask<RowSet> IRule<ISqlApi, SelectRequest, RowSet>.ApplyAsync(ISqlApi sqlApi, SelectRequest request, CancellationToken cancellationToken)
		{
			request.From = sqlApi.GetObjectSchema(request.From).Name;
			return await sqlApi.SelectAsync(request, cancellationToken);
		}

		async ValueTask<string> IRule<SelectRequest, string>.ApplyAsync(SelectRequest request, CancellationToken cancellationToken)
			=> await ValueTask.FromResult(request.ToSql());
	}
}
