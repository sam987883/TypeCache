// Copyright (c) 2021 Samuel Abraham

using Microsoft.AspNetCore.Http;
using TypeCache.Utilities;

namespace TypeCache.Web.Extensions;

public static class HttpRequestExtensions
{
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string? GetQueryString(this HttpRequest @this, string key)
		=> @this.Query[key];

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string[] GetQueryValues(this HttpRequest @this, string key)
		=> @this.GetQueryString(key)?.Split(',') ?? Array<string>.Empty;
}
