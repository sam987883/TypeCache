// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Extensions;

namespace TypeCache.Data.Business
{
	internal class UpdateRule : IRule<(ISqlApi SqlApi, UpdateRequest Update), RowSet>, IRule<UpdateRequest, string>
	{
		async ValueTask<RowSet> IRule<(ISqlApi SqlApi, UpdateRequest Update), RowSet>.ApplyAsync((ISqlApi SqlApi, UpdateRequest Update) request, CancellationToken cancellationToken)
		{
			request.Update.Table = request.SqlApi.GetObjectSchema(request.Update.Table).Name;
			return await request.SqlApi.UpdateAsync(request.Update, cancellationToken);
		}

		async ValueTask<string> IRule<UpdateRequest, string>.ApplyAsync(UpdateRequest request, CancellationToken cancellationToken)
			=> await ValueTask.FromResult(request.ToSQL());
	}
}
