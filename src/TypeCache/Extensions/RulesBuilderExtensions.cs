// Copyright (c) 2021 Samuel Abraham

using System.Data;
using System.Text.Json.Nodes;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Data.Mediation;
using TypeCache.Mediation;

namespace TypeCache.Extensions;

public static class RulesBuilderExtensions
{
	/// <summary>
	/// <c>=&gt; @<paramref name="this"/>.AddSingleton&lt;IRule&lt;<see cref="SqlDataSetRequest"/>, <see cref="DataSet"/>&gt;, <see cref="SqlDataSetRule"/>&gt;()<br/>
	/// <see langword="    "/>.AddSingleton&lt;IRule&lt;<see cref="SqlDataTableRequest"/>, <see cref="DataTable"/>&gt;, <see cref="SqlDataTableRule"/>&gt;()<br/>
	/// <see langword="    "/>.AddSingleton&lt;IRule&lt;<see cref="SqlExecuteRequest"/>, <see cref="SqlExecuteRule"/>&gt;()<br/>
	/// <see langword="    "/>.AddSingleton&lt;IRule&lt;<see cref="SqlJsonArrayRequest"/>, <see cref="JsonArray"/>&gt;, <see cref="SqlJsonArrayRule"/>&gt;()<br/>
	/// <see langword="    "/>.AddSingleton&lt;IRule&lt;<see cref="SqlModelsRequest"/>, IList&lt;<see cref="object"/>&gt;&gt;, <see cref="SqlModelsRule"/>&gt;()<br/>
	/// <see langword="    "/>.AddSingleton&lt;IRule&lt;<see cref="SqlScalarRequest"/>, <see cref="object"/>&gt;, <see cref="SqlScalarRule"/>&gt;()<br/>
	/// </c>
	/// <i><b>Requires calls to:</b></i>
	/// <code>
	/// <see cref="ServiceCollectionExtensions.AddMediation(IServiceCollection, Action{RulesBuilder}?)"/><br/>
	/// </code>
	/// </summary>
	public static RulesBuilder AddSqlCommandRules(this RulesBuilder @this)
		=> @this.AddRule<SqlDataSetRequest, DataSet>(new SqlDataSetRule())
			.AddRule<SqlDataSetRequest, DataSet>(new SqlDataSetRule())
			.AddRule<SqlDataTableRequest, DataTable>(new SqlDataTableRule())
			.AddRule<SqlExecuteRequest>(new SqlExecuteRule())
			.AddRule<SqlJsonArrayRequest, JsonArray>(new SqlJsonArrayRule())
			.AddRule<SqlModelsRequest, IList<object>>(new SqlModelsRule())
			.AddRule<SqlScalarRequest, object?>(new SqlScalarRule());
}
