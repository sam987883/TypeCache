// Copyright (c) 2021 Samuel Abraham

using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using TypeCache.Collections;
using static TypeCache.Default;

namespace TypeCache.Web.Extensions;

public static class HttpRequestExtensions
{
	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static string GetQueryString(this HttpRequest @this, string key)
		=> @this.Query[key];

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static string[] GetQueryValues(this HttpRequest @this, string key)
		=> ((string)@this.Query[key])?.Split(',') ?? Array<string>.Empty;
}
