// Copyright (c) 2021 Samuel Abraham

using System;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;

namespace TypeCache.Data.Business
{
	internal class UpdateValidationRule : IValidationRule<ISqlApi, UpdateRequest>
	{
		public async ValueTask<ValidationResponse> ApplyAsync(ISqlApi sqlApi, UpdateRequest request, CancellationToken cancellationToken)
		{
			return await Task.Run(() =>
			{
				try
				{
					var schema = sqlApi.GetObjectSchema(request.Table);
					request.Table = schema.Name;

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
