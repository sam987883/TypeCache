// Copyright (c) 2021 Samuel Abraham

using System;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;

namespace TypeCache.Data.Business
{
	internal class InsertValidationRule : IValidationRule<ISqlApi, InsertRequest>
	{
		public async ValueTask<ValidationResponse> ApplyAsync(ISqlApi sqlApi, InsertRequest request, CancellationToken cancellationToken)
		{
			return await Task.Run(() =>
			{
				try
				{
					var insertSchema = sqlApi.GetObjectSchema(request.Into);
					request.Into = insertSchema.Name;
					var fromSchema = sqlApi.GetObjectSchema(request.From);
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
