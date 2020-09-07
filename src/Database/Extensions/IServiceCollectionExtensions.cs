// Copyright(c) 2020 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using Sam987883.Reflection;
using System.Data.Common;

namespace Sam987883.Database.Extensions
{
	public static class IServiceCollectionExtensions
	{
		/// <summary>
		/// Makes a call to:
		/// <code>DbProviderFactories.RegisterFactory(databaseProvider, dbProviderFactory);</code>
		/// </summary>
		/// <param name="databaseProvider">ie. <c>"Microsoft.Data.SqlClient"</c></param>
		/// <param name="dbProviderFactory">ie. <c>SqlClientFactory.Instance</c></param>
		public static IServiceCollection RegisterDatabaseProviderFactory(this IServiceCollection @this, string databaseProvider, DbProviderFactory dbProviderFactory)
		{
			DbProviderFactories.RegisterFactory(databaseProvider, dbProviderFactory);
			return @this;
		}

		/// <summary>
		/// Registers Singletons:
		/// <list type="bullet">
		/// <item><see cref="ISchemaFactory"/></item>
		/// <item><see cref="ISchemaStore"/></item>
		/// </list>
		/// <i>Requires call to: <see cref="Reflection.Extensions.IServiceCollectionExtensions.RegisterTypeCache"/></i>
		/// </summary>
		public static IServiceCollection RegisterDatabaseSchema(this IServiceCollection @this) => @this
			.AddSingleton<ISchemaFactory>(provider => new SchemaFactory(provider.GetRequiredService<ITypeCache>()))
			.AddSingleton<ISchemaStore>(provider => new SchemaStore(provider.GetRequiredService<ISchemaFactory>()));
	}
}
