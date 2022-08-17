// Copyright (c) 2021 Samuel Abraham

using System;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Domain;
using TypeCache.Data.Extensions;
using TypeCache.Data.Schema;
using TypeCache.Extensions;
using static TypeCache.Default;

namespace TypeCache.Data;

public class DataSource : IEquatable<DataSource>
{
	private readonly DbProviderFactory _DbProviderFactory;

	internal DataSource(string name, string databaseProvider, string connectionString)
	{
		this.Name = name;
		this.DatabaseProvider = databaseProvider;
		this.ConnectionString = connectionString;
		this._DbProviderFactory = DbProviderFactories.GetFactory(databaseProvider);
		this.ObjectSchemas = new(name => this._GetObjectSchema(name).GetAwaiter().GetResult(), comparer: STRING_COMPARISON.ToStringComparer());
	}

	public string ConnectionString { get; }

	public string DatabaseProvider { get; }

	public string Name { get; }

	public LazyDictionary<string, ObjectSchema> ObjectSchemas { get; }

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public DbConnection CreateDbConnection()
		=> this._DbProviderFactory.CreateConnection(this.ConnectionString);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public bool Equals(DataSource? other)
		=> this.Name.Is(other?.Name);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override bool Equals([NotNullWhen(true)] object? item)
		=> item is var dataSource && this.Equals(dataSource);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override int GetHashCode()
		=> this.Name.GetHashCode(StringComparison.OrdinalIgnoreCase);

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override string ToString()
		=> this.Name;

	private async Task<ObjectSchema> _GetObjectSchema(string name)
	{
		await using var connection = this.CreateDbConnection();
		await connection.OpenAsync();

		var objectId = await connection.GetObjectId(name);
		objectId.AssertNotNull();

		var command = new SqlCommand
		{
			DataSource = connection.DataSource,
			ReadData = async (reader, token) =>
			{
				var objectSchemaModel = (await reader.ReadRowsAsync<ObjectSchemaModel>(1, token)).First();
				if (objectSchemaModel is null)
					return null!;

				if (await reader.NextResultAsync(token) && await reader.ReadAsync(token))
				{
					var count = await reader.GetFieldValueAsync<int>(0, token);
					if (await reader.NextResultAsync(token))
						objectSchemaModel!.Columns = await reader.ReadRowsAsync<ColumnSchemaModel>(count, token);
				}

				if (await reader.NextResultAsync(token) && await reader.ReadAsync(token))
				{
					var count = await reader.GetFieldValueAsync<int>(0, token);
					if (await reader.NextResultAsync(token))
						objectSchemaModel!.Parameters = await reader.ReadRowsAsync<ParameterSchemaModel>(count, token);
				}

				return objectSchemaModel!;
			},
			SQL = @$"
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SET NOCOUNT ON;

{ObjectSchema.SQL}
{ColumnSchema.SQL}
{ParameterSchema.SQL}"
		};
		command.InputParameters.Add(nameof(objectId), objectId.Value);

		var objectSchemaModel = (ObjectSchemaModel?)await connection.ExecuteAsync(command, command.ReadData);
		await connection.CloseAsync();

		objectSchemaModel.AssertNotNull();
		return new ObjectSchema(connection.DataSource, objectSchemaModel);
	}
}
