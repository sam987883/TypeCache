// Copyright (c) 2021 Samuel Abraham

using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;
using TypeCache.Extensions;

namespace TypeCache.Data.Business
{
	internal class InsertValidationRule : IValidationRule<DbConnection, InsertRequest>
	{
		public async ValueTask<ValidationResponse> ApplyAsync(DbConnection dbConnection, InsertRequest request, CancellationToken cancellationToken)
		{
			try
			{
				var insertSchema = await dbConnection.GetObjectSchema(request.Into);
				request.Into = insertSchema.Name;
				var fromSchema = await dbConnection.GetObjectSchema(request.From);
				request.From = fromSchema.Name;

				var validator = new SchemaValidator(insertSchema);
				validator.Validate(request);

				return default;
			}
			catch (ArgumentException exception)
			{
				return new ValidationResponse
				{
					IsError = true,
					Messages = new[] { exception.Source, exception.ParamName, exception.Message, exception.StackTrace }
				};
			}
			catch (Exception exception)
			{
				return new ValidationResponse
				{
					IsError = true,
					Messages = new[] { exception.Message, exception.StackTrace }
				};
			}
		}
	}
}
