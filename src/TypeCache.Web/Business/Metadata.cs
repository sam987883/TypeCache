// Copyright (c) 2021 Samuel Abraham

using Microsoft.AspNetCore.Http;
using System.Data.Common;

namespace TypeCache.Web.Business
{
	public readonly struct Metadata
	{
		public DbConnection DbConnection { get; init; }

		public HttpContext HttpContext { get; init; }
	}
}
