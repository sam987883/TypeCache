// Copyright (c) 2021 Samuel Abraham

using System.Data;
using System.Data.Common;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace TypeCache.Data.Extensions;

public static class DbCommandExtensions
{
	public static int AddInputParameter(this IDbCommand @this, string name, object? value)
	{
		var parameter = @this.CreateParameter();
		parameter.Direction = ParameterDirection.Input;
		parameter.ParameterName = name;
		parameter.Value = value ?? DBNull.Value;
		return @this.Parameters.Add(parameter);
	}

	public static int AddInputOutputParameter(this IDbCommand @this, string name, object? value, DbType dbType)
	{
		var parameter = @this.CreateParameter();
		parameter.Direction = ParameterDirection.InputOutput;
		parameter.ParameterName = name;
		parameter.Value = value ?? DBNull.Value;
		parameter.DbType = dbType;
		return @this.Parameters.Add(parameter);
	}

	public static int AddOutputParameter(this IDbCommand @this, string name, DbType dbType)
	{
		var parameter = @this.CreateParameter();
		parameter.Direction = ParameterDirection.Output;
		parameter.ParameterName = name;
		parameter.DbType = dbType;
		return @this.Parameters.Add(parameter);
	}

	public static async Task<DbTransaction> BeginTransactionAsync(this DbCommand @this, IsolationLevel isolationLevel, CancellationToken token = default)
		=> @this.Transaction = isolationLevel == IsolationLevel.Unspecified
			? await @this.Connection!.BeginTransactionAsync(token)
			: await @this.Connection!.BeginTransactionAsync(isolationLevel, token);

	public static void CopyOutputParameters(this DbCommand @this, SqlCommand sqlCommand)
	{
		var parameters = @this.Parameters
			.OfType<DbParameter>()
			.Where(parameter => parameter.Direction is ParameterDirection.InputOutput || parameter.Direction is ParameterDirection.Output);

		foreach (var parameter in parameters)
			sqlCommand.Parameters[parameter.ParameterName] = parameter.Value;
	}

	public static async Task<int> ExecuteReaderAsync(this DbCommand @this, Func<DbDataReader, CancellationToken, ValueTask> readData, CancellationToken token = default)
	{
		await using var reader = await @this.ExecuteReaderAsync(token);
		await readData(reader, token);
		await reader.CloseAsync();
		return reader.RecordsAffected;
	}

	public static async Task<T> ExecuteReaderAsync<T>(this DbCommand @this, Func<DbDataReader, CancellationToken, ValueTask<T>> readData, CancellationToken token = default)
	{
		await using var reader = await @this.ExecuteReaderAsync(CommandBehavior.CloseConnection, token);
		var data = await readData(reader, token);
		await reader.CloseAsync();
		return data;
	}

	public static async Task<DataTable> GetDataTableAsync(this DbCommand @this, CancellationToken token = default)
	{
		await using var reader = await @this.ExecuteReaderAsync(CommandBehavior.SingleResult, token);
		return reader.ReadDataTable();
	}

	public static async Task<JsonArray> GetResultsAsJsonAsync(this DbCommand @this, JsonNodeOptions? jsonOptions = null, CancellationToken token = default)
	{
		await using var reader = await @this.ExecuteReaderAsync(CommandBehavior.SingleResult, token);
		return await reader.ReadResultsAsJsonAsync(jsonOptions, token);
	}

	public static async Task<JsonObject> GetResultSetAsJsonAsync(this DbCommand @this, JsonNodeOptions? jsonOptions = null, CancellationToken token = default)
	{
		await using var reader = await @this.ExecuteReaderAsync(CommandBehavior.SingleResult, token);
		return await reader.ReadResultSetAsJsonAsync(jsonOptions, token);
	}

	public static async Task<IList<T>> GetModelsAsync<T>(this DbCommand @this, int listInitialCapacity, CancellationToken token = default)
		where T : notnull, new()
	{
		var rows = new List<T>(listInitialCapacity);
		await using var reader = await @this.ExecuteReaderAsync(token);
		await reader.ReadModelsAsync<T>(rows, token);
		await reader.CloseAsync();
		@this.AddInputParameter(nameof(reader.RecordsAffected), reader.RecordsAffected);
		return rows;
	}

	public static async Task<IList<object>> GetModelsAsync(this DbCommand @this, Type modelType, int listInitialCapacity, CancellationToken token = default)
	{
		var rows = new List<object>(listInitialCapacity);
		await using var reader = await @this.ExecuteReaderAsync(token);
		await reader.ReadModelsAsync(modelType, rows, token);
		await reader.CloseAsync();
		@this.AddInputParameter(nameof(reader.RecordsAffected), reader.RecordsAffected);
		return rows;
	}

	public static async Task<string?> GetStringAsync(this DbCommand @this, CancellationToken token = default)
		=> await @this.ExecuteScalarAsync(token) switch
		{
			DBNull or null => null,
			string value => value,
			object value => value.ToString()
		};

	public static async Task<T?> GetValueAsync<T>(this DbCommand @this, CancellationToken token = default)
		where T : unmanaged
		=> await @this.ExecuteScalarAsync(token) switch
		{
			T value => value,
			_ => null
		};

	public static async Task WriteResultsAsJsonAsync(this DbCommand @this, Utf8JsonWriter writer, JsonSerializerOptions? jsonOptions = null, CancellationToken token = default)
	{
		await using var reader = await @this.ExecuteReaderAsync(CommandBehavior.SingleResult, token);
		await reader.WriteResultsAsJsonAsync(writer, jsonOptions, token);
	}

	public static async Task WriteResultSetAsJsonAsync(this DbCommand @this, Utf8JsonWriter writer, JsonSerializerOptions? jsonOptions = null, CancellationToken token = default)
	{
		await using var reader = await @this.ExecuteReaderAsync(CommandBehavior.SingleResult, token);
		await reader.WriteResultSetAsJsonAsync(writer, jsonOptions, token);
	}
}
