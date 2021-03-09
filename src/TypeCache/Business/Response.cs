// Copyright (c) 2021 Samuel Abraham

using System;

namespace TypeCache.Business
{
	public record Response<T>(T? Result, Exception? Exception)
	{
		public static Response<T> Empty => new Response<T>(default, null);

		public Response(T value) : this(value, null)
		{
		}

		public Response(Exception exception) : this(default, exception)
		{
		}
	}
}
