// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Extensions;

namespace TypeCache.Business
{
	public struct Response<T>
	{
		public static Response<T> Empty
			=> new Response<T>
			{
				HasError = false,
				Messages = Array.Empty<string>(),
				Result = default
			};

		public Response(T result)
		{
			this.HasError = false;
			this.Messages = Array.Empty<string>();
			this.Result = result;
		}

		public Response(Exception ex)
		{
			this.HasError = true;
			this.Messages = new[] { ex.Message, ex.StackTrace! };
			this.Result = default;
		}

		public Response(ValidationResponse[] validationResponses)
		{
			this.HasError = validationResponses.Any(_ => _.IsError);
			this.Messages = validationResponses.ToMany(_ => _.Messages).ToArray();
			this.Result = default;
		}

		public bool HasError { get; init; }

		public string[] Messages { get; init; }

		public T? Result { get; init; }
	}
}
