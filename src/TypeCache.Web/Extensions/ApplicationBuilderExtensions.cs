﻿// Copyright (c) 2021 Samuel Abraham

using Microsoft.AspNetCore.Builder;
using TypeCache.Web.Middleware;

namespace TypeCache.Web.Extensions;

public static class ApplicationBuilderExtensions
{
	/// <summary>
	/// Registers <see cref="SqlApiErrorHandlerMiddleware"/>.
	/// </summary>
	/// <remarks>Must be called before any other UseSqlApi middelware registration.</remarks>
	public static IApplicationBuilder UseSqlApi(this IApplicationBuilder @this)
		=> @this.UseMiddleware<SqlApiErrorHandlerMiddleware>();
}
