// Copyright(c) 2020 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using Sam987883.Database.Models;
using Sam987883.Reflection;

namespace Sam987883.Database.Extensions
{
	public static class IServiceCollectionExtensions
	{
		public static IServiceCollection RegisterDatabaseSchema(this IServiceCollection @this) => @this
			.AddSingleton<ISchemaFactory>(provider => new SchemaFactory(provider.GetRequiredService<IRowSetConverter<ObjectSchema>>()
				, provider.GetRequiredService<IRowSetConverter<ColumnSchema>>()
				, provider.GetRequiredService<IRowSetConverter<ParameterSchema>>()))
			.AddSingleton<ISchemaStore>(provider => new SchemaStore(provider.GetRequiredService<ISchemaFactory>()));
	}
}
