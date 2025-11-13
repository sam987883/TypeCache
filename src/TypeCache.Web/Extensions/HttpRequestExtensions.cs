// Copyright (c) 2021 Samuel Abraham

using Microsoft.AspNetCore.Http;

namespace TypeCache.Web.Extensions;

public static class HttpRequestExtensions
{
	extension(HttpRequest @this)
	{
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string? GetQueryString(string key)
			=> @this.Query[key];

		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public string[] GetQueryValues(string key)
			=> @this.GetQueryString(key)?.Split(',') ?? [];
	}
}
