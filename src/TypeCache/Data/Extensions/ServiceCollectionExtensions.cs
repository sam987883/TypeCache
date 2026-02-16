// Copyright (c) 2021 Samuel Abraham

using System.Data;
using System.Data.Common;
using System.Text.Json.Nodes;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Data.Mediation;
using TypeCache.Mediation;

namespace TypeCache.Data.Extensions;

public static class ServiceCollectionExtensions
{
	extension(IServiceCollection @this)
	{
		/// <summary>
		/// Registers keyed singleton: <c><see cref="IDataSource"/></c> where the key is the data source name.
		/// </summary>
		public IServiceCollection AddDataSource(string name, DbProviderFactory dbProviderFactory, string connectionString, string[] databases)
			=> @this.AddKeyedSingleton<IDataSource>(name, new DataSource(name, dbProviderFactory, connectionString, databases));

		/// <summary>
		/// Registers the following Rules:
		/// <list type="bullet">
		/// <item><c>IRule&lt;<see cref="SqlCommand"/>, ValueTask&lt;<see cref="DataSet"/>&gt;&gt;</c></item>
		/// <item><c>IRule&lt;<see cref="SqlCommand"/>, ValueTask&lt;<see cref="DataTable"/>&gt;&gt;</c></item>
		/// <item><c>IRule&lt;<see cref="SqlCommand"/>, <see cref="Task"/>&gt;</c></item>
		/// <item><c>IRule&lt;<see cref="SqlCommand"/>, ValueTask&lt;<see cref="JsonArray"/>&gt;&gt;</c></item>
		/// <item><c>IRule&lt;<see cref="SqlCommand"/>, ValueTask&lt;<see cref="IList{T}"/>&gt;&gt;</c></item>
		/// <item><c>IRule&lt;<see cref="SqlCommand"/>, ValueTask&lt;<see langword="object"/>&gt;&gt;</c></item>
		/// </list>
		/// </summary>
		public IServiceCollection AddSqlCommandRules()
			=> @this.AddSqlDataSetRule()
				.AddSqlDataTableRule()
				.AddSqlExecuteRule()
				.AddSqlResultJsonRule()
				.AddSqlScalarRule();

		/// <summary>
		/// Registers: <c>IRule&lt;<see cref="SqlCommand"/>, ValueTask&lt;<see cref="DataSet"/>&gt;&gt;</c>
		/// </summary>
		public IServiceCollection AddSqlDataSetRule()
			=> @this.AddRule<SqlDataSetRule>();

		/// <summary>
		/// Registers: <c>IRule&lt;<see cref="SqlCommand"/>, ValueTask&lt;<see cref="DataTable"/>&gt;&gt;</c>
		/// </summary>
		public IServiceCollection AddSqlDataTableRule()
			=> @this.AddRule<SqlDataTableRule>();

		/// <summary>
		/// Registers: <c>IRule&lt;<see cref="SqlCommand"/>, <see cref="Task"/>&gt;</c>
		/// </summary>
		public IServiceCollection AddSqlExecuteRule()
			=> @this.AddRule<SqlExecuteRule>();

		/// <summary>
		/// Registers: <c>IRule&lt;<see cref="SqlCommand"/>, ValueTask&lt;<see cref="JsonArray"/>&gt;&gt;</c>
		/// </summary>
		public IServiceCollection AddSqlResultJsonRule()
			=> @this.AddRule<SqlResultJsonRule>();

		/// <summary>
		/// Registers: <c>IRule&lt;<see cref="SqlCommand"/>, ValueTask&lt;<see langword="object"/>&gt;&gt;</c>
		/// </summary>
		public IServiceCollection AddSqlScalarRule()
			=> @this.AddRule<SqlScalarRule>();
	}
}
