﻿// Copyright (c) 2021 Samuel Abraham

using System.Data;
using System.Threading.Tasks;
using GraphQL;
using TypeCache.Data;
using TypeCache.Extensions;

namespace TypeCache.GraphQL.Resolvers;

public sealed class DatabaseSchemaFieldResolver : FieldResolver<DataRow[]>
{
	protected override async ValueTask<DataRow[]?> ResolveAsync(IResolveFieldContext context)
	{
		var collection = context.FieldDefinition.GetMetadata<SchemaCollection>(nameof(SchemaCollection));
		var dataSource = context.FieldDefinition.GetMetadata<IDataSource>(nameof(IDataSource));

		string database = context.GetArgument<string>(nameof(database));
		string where = context.GetArgument<string>(nameof(where));
		string[] orderBy = context.GetArgument<string[]>(nameof(orderBy));

		var table = await dataSource.GetDatabaseSchemaAsync(collection, database, context.CancellationToken);
		return table?.Select(where, orderBy?.ToCSV());
	}
}