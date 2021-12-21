// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Extensions;
using TypeCache.Data.Requests;

namespace TypeCache.Data.Business;

internal class CountRule : IRule<CountRequest, long>, IRule<CountRequest, string>
{
	private readonly ISqlApi _SqlApi;

	public CountRule(ISqlApi sqlApi)
	{
		this._SqlApi = sqlApi;
	}

	async ValueTask<long> IRule<CountRequest, long>.ApplyAsync(CountRequest request, CancellationToken cancellationToken)
	{
		request.From = this._SqlApi.GetObjectSchema(request.DataSource, request.From).Name;
		return await this._SqlApi.CountAsync(request, cancellationToken);
	}

	async ValueTask<string> IRule<CountRequest, string>.ApplyAsync(CountRequest request, CancellationToken cancellationToken)
		=> await ValueTask.FromResult(request.ToSQL());
}
