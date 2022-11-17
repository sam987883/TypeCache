// Copyright (c) 2021 Samuel Abraham

using System.Data;
using System.Threading.Tasks;
using GraphQL;
using TypeCache.Collections.Extensions;
using TypeCache.Data;
using TypeCache.GraphQL.SqlApi;

namespace TypeCache.GraphQL.Resolvers;

public sealed class DatabaseSchemaFieldResolver : FieldResolver<DataRow[]>
{
	protected override async ValueTask<DataRow[]?> ResolveAsync(IResolveFieldContext context)
	{
		var collection = context.FieldDefinition.GetMetadata<SchemaCollection>(nameof(SchemaCollection));
		var dataSource = context.FieldDefinition.GetMetadata<IDataSource>(nameof(IDataSource));

		string database = context.GetArgument<string>(nameof(database));
		string where = context.GetArgument<string>(nameof(where));
		OrderBy[] orderBy = context.GetArgument<OrderBy[]>(nameof(orderBy));

		var table = await dataSource.GetDatabaseSchemaAsync(collection, database, context.CancellationToken);
		return table?.Select(where, orderBy.Map(_ => _.ToString()).Join(", "));
	}
}
