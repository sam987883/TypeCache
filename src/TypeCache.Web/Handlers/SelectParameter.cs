using Microsoft.AspNetCore.Http;
using TypeCache.Data;
using TypeCache.Extensions;

namespace TypeCache.Web.Handlers;

public sealed class SelectParameter : SelectQuery
{
	public static ValueTask<SelectParameter?> BindAsync(HttpContext context)
		=> ValueTask.FromResult(new SelectParameter
		{
			Distinct = context.Request.Query.TryGetValue("distinct", out var distinct),
			DistinctOn = distinct.ToString(),
			Fetch = context.Request.Query.TryGetValue("fetch", out var fetch) ? uint.Parse(fetch.ToString()) : default,
			From = context.Request.Query.TryGetValue("from", out var from) ? from : context.Request.RouteValues["table"]!.ToString(),
			GroupBy = context.Request.Query.TryGetValue("groupBy", out var groupBy) ? groupBy.ToString() : null,
			Having = context.Request.Query.TryGetValue("having", out var having) ? having.ToString() : null,
			Offset = context.Request.Query.TryGetValue("offset", out var offset) ? uint.Parse(offset.ToString()) : default,
			OrderBy = context.Request.Query.TryGetValue("orderBy", out var orderBy) ? orderBy.ToString().SplitEx(',') : null,
			Select = context.Request.Query.TryGetValue("select", out var select) ? select.ToString().SplitEx(',') : null,
			TableHints = context.Request.Query.TryGetValue("hints", out var hints) ? hints.ToString() : null,
			Top = context.Request.Query.TryGetValue("top", out var top) ? uint.Parse(top.ToString().TrimEnd('%')) : null,
			TopPercent = top.ToString().EndsWith('%') is true,
			Where = context.Request.Query.TryGetValue("where", out var where) ? where.ToString() : null
		})!;
}
