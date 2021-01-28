// Copyright (c) 2021 Samuel Abraham

using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Extensions;

namespace TypeCache.Data.Business
{
	internal class MergeValidationRule : IValidationRule<DbConnection, BatchRequest>
	{
		public async ValueTask<ValidationResponse> ApplyAsync(DbConnection dbConnection, BatchRequest request, CancellationToken cancellationToken)
		{
			return await Task.Run(() =>
			{
				try
				{
					var schema = dbConnection.GetObjectSchema(request.Table);
					request.Table = schema.Name;
					request.On = schema.Columns.If(column => column!.PrimaryKey).To(column => column!.Name).ToArray();

					var validator = new SchemaValidator(schema);
					validator.Validate(request);

					return default;
				}
				catch (ArgumentException exception)
				{
					return new ValidationResponse
					{
						IsError = true,
						Messages = new[] { exception.Source!, exception.ParamName!, exception.Message, exception.StackTrace! }
					};
				}
				catch (Exception exception)
				{
					return new ValidationResponse
					{
						IsError = true,
						Messages = new[] { exception.Message, exception.StackTrace! }
					};
				}
			});
		}
	}
}
