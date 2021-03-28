// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Extensions;

namespace TypeCache.Data.Business
{
	internal class UpdateRule : IRule<ISqlApi, UpdateRequest, RowSet>, IRule<UpdateRequest, string>
	{
		async ValueTask<RowSet> IRule<ISqlApi, UpdateRequest, RowSet>.ApplyAsync(ISqlApi sqlApi, UpdateRequest request, CancellationToken cancellationToken)
		{
			request.Table = sqlApi.GetObjectSchema(request.Table).Name;
			return await sqlApi.UpdateAsync(request, cancellationToken);
		}

		async ValueTask<string> IRule<UpdateRequest, string>.ApplyAsync(UpdateRequest request, CancellationToken cancellationToken)
			=> await ValueTask.FromResult(request.ToSql());
	}
}
