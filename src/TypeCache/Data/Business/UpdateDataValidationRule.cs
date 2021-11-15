// Copyright (c) 2021 Samuel Abraham

using System;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Requests;
using TypeCache.Data.Schema;
using TypeCache.Extensions;

namespace TypeCache.Data.Business
{
	internal class UpdateDataValidationRule : IValidationRule<UpdateDataRequest>
	{
		private readonly ISqlApi _SqlApi;

		public UpdateDataValidationRule(ISqlApi sqlApi)
		{
			this._SqlApi = sqlApi;
		}

		public async ValueTask ValidateAsync(UpdateDataRequest request, CancellationToken cancellationToken)
		{
			var schema = this._SqlApi.GetObjectSchema(request.DataSource, request.Table);
			schema.Type.Assert($"{nameof(UpdateDataRequest)}.{nameof(UpdateDataRequest.Table)}", ObjectType.Table);
			request.Table = schema.Name;

			var invalidColumnCsv = request.Input.Columns.Without(schema.Columns.If(column => !column.Identity && !column.ReadOnly).To(column => column.Name)).ToCSV(column => $"[{column}]");
			if (!invalidColumnCsv.IsBlank())
				throw new ArgumentException($"{nameof(request.Input)}.{nameof(request.Input.Columns)} contains non-writable columns: {invalidColumnCsv}.", $"{nameof(UpdateDataRequest)}.{nameof(UpdateDataRequest.Input)}");

			if (!request.On.Any())
				request.On = schema.Columns.If(column => column.PrimaryKey).To(column => column.Name).ToArray();

			await ValueTask.CompletedTask;
		}
	}
}
