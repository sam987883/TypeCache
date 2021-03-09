﻿// Copyright (c) 2021 Samuel Abraham

using System;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Business;

namespace TypeCache.Data.Business
{
	internal class DeleteValidationRule : IValidationRule<ISqlApi, DeleteRequest>
	{
		public async ValueTask<ValidationResponse> ApplyAsync(ISqlApi sqlApi, DeleteRequest request, CancellationToken cancellationToken)
		{
			return await Task.Run(() =>
			{
				try
				{
					var schema = sqlApi.GetObjectSchema(request.From);
					request.From = schema.Name;

					var validator = new SchemaValidator(schema);
					validator.Validate(request);

					return ValidationResponse.Success;
				}
				catch (Exception exception)
				{
					return new ValidationResponse(exception);
				}
			});
		}
	}
}
