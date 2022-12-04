// Copyright (c) 2021 Samuel Abraham

using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using TypeCache.Collections;
using static System.Runtime.CompilerServices.MethodImplOptions;

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
