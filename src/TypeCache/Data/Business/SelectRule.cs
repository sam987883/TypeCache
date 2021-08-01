// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Extensions;

namespace TypeCache.Data.Business
{
	internal class SelectRule : IRule<SelectRequest, RowSet>, IRule<SelectRequest, string>
	{
		private readonly ISqlApi _SqlApi;

		public SelectRule(ISqlApi sqlApi)
		{
			this._SqlApi = sqlApi;
		}

		async ValueTask<RowSet> IRule<SelectRequest, RowSet>.ApplyAsync(SelectRequest request, CancellationToken cancellationToken)
		{
			request.From = this._SqlApi.GetObjectSchema(request.DataSource, request.From).Name;
			return await this._SqlApi.SelectAsync(request, cancellationToken);
		}

		async ValueTask<string> IRule<SelectRequest, string>.ApplyAsync(SelectRequest request, CancellationToken cancellationToken)
			=> await ValueTask.FromResult(request.ToSQL());
	}
}
