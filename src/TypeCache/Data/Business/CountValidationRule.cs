// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Data.Requests;

namespace TypeCache.Data.Business
{
	internal class CountValidationRule : IValidationRule<CountRequest>
	{
		private readonly ISqlApi _SqlApi;

		public CountValidationRule(ISqlApi sqlApi)
		{
			this._SqlApi = sqlApi;
		}

		public async ValueTask ValidateAsync(CountRequest request, CancellationToken cancellationToken)
		{
			var schema = this._SqlApi.GetObjectSchema(request.DataSource, request.From);

			await ValueTask.CompletedTask;
		}
	}
}
