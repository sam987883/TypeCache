// Copyright (c) 2021 Samuel Abraham

using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Domain;

namespace TypeCache.Data.Extensions;

public static class DbCommandExtensions
{
	/// <summary>
	/// <code>
	/// <see langword="var"/> parameter = @<paramref name="this"/>.CreateParameter();<br/>
	/// parameter.Direction = <see cref="ParameterDirection.Input"/>;<br/>
	/// parameter.ParameterName = <paramref name="name"/>;<br/>
	/// parameter.Value = <paramref name="value"/> ?? <see cref="DBNull.Value"/>;<br/>
	/// <see langword="return"/> @<paramref name="this"/>.Parameters.Add(parameter);
	/// </code>
	/// </summary>
	public static int AddInputParameter(this IDbCommand @this, string name, object? value)
	{
		var parameter = @this.CreateParameter();
		parameter.Direction = ParameterDirection.Input;
		parameter.ParameterName = name;
		parameter.Value = value ?? DBNull.Value;
		return @this.Parameters.Add(parameter);
	}

	/// <summary>
	/// <code>
	/// <see langword="var"/> parameter = @<paramref name="this"/>.CreateParameter();<br/>
	/// parameter.Direction = <see cref="ParameterDirection.InputOutput"/>;<br/>
	/// parameter.ParameterName = <paramref name="name"/>;<br/>
	/// parameter.Value = <paramref name="value"/> ?? <see cref="DBNull.Value"/>;<br/>
	/// parameter.DbType = <paramref name="dbType"/>;<br/>
	/// <see langword="return"/> @<paramref name="this"/>.Parameters.Add(parameter);
	/// </code>
	/// </summary>
	public static int AddInputOutputParameter(this IDbCommand @this, string name, object? value, DbType dbType)
	{
		var parameter = @this.CreateParameter();
		parameter.Direction = ParameterDirection.InputOutput;
		parameter.ParameterName = name;
		parameter.Value = value ?? DBNull.Value;
		parameter.DbType = dbType;
		return @this.Parameters.Add(parameter);
	}

	/// <summary>
	/// <code>
	/// <see langword="var"/> parameter = @<paramref name="this"/>.CreateParameter();<br/>
	/// parameter.Direction = <see cref="ParameterDirection.Output"/>;<br/>
	/// parameter.ParameterName = <paramref name="name"/>;<br/>
	/// parameter.DbType = <paramref name="dbType"/>;<br/>
	/// <see langword="return"/> @<paramref name="this"/>.Parameters.Add(parameter);
	/// </code>
	/// </summary>
	public static int AddOutputParameter(this IDbCommand @this, string name, DbType dbType)
	{
		var parameter = @this.CreateParameter();
		parameter.Direction = ParameterDirection.Output;
		parameter.ParameterName = name;
		parameter.DbType = dbType;
		return @this.Parameters.Add(parameter);
	}

	/// <summary>
	/// <code>
	/// =&gt; @<paramref name="this"/>.Transaction = <paramref name="isolationLevel"/> == <see cref="IsolationLevel.Unspecified"/><br/>
	/// <see langword="    "/>? <see langword="await"/> @<paramref name="this"/>.Connection.BeginTransactionAsync(<paramref name="token"/>)<br/>
	/// <see langword="    "/>: <see langword="await"/> @<paramref name="this"/>.Connection.BeginTransactionAsync(<paramref name="isolationLevel"/>, <paramref name="token"/>);
	/// </code>
	/// </summary>
	public static async Task<DbTransaction> BeginTransactionAsync(this DbCommand @this, IsolationLevel isolationLevel, CancellationToken token = default)
		=> @this.Transaction = isolationLevel == IsolationLevel.Unspecified
			? await @this.Connection!.BeginTransactionAsync(token)
			: await @this.Connection!.BeginTransactionAsync(isolationLevel, token);

	/// <summary>
	/// <code>
	/// <see langword="await using var"/> reader = <see langword="await"/> @<paramref name="this"/>.ExecuteReaderAsync(<paramref name="token"/>);<br/>
	/// <see langword="await"/> <paramref name="readData"/>(reader, <paramref name="token"/>)<br/>
	/// <see langword="await"/> reader.CloseAsync();<br/>
	/// <see langword="return"/> reader.RecordsAffected;
	/// </code>
	/// </summary>
	public static async Task<int> ExecuteReaderAsync(this DbCommand @this, Func<DbDataReader, CancellationToken, ValueTask> readData, CancellationToken token = default)
	{
		await using var reader = await @this.ExecuteReaderAsync(token);
		await readData(reader, token);
		await reader.CloseAsync();
		return reader.RecordsAffected;
	}

	/// <summary>
	/// <code>
	/// <see langword="await using var"/> reader = <see langword="await"/> @<paramref name="this"/>.ExecuteReaderAsync(<paramref name="token"/>);<br/>
	/// <see langword="await"/> <paramref name="readData"/>(reader, <paramref name="token"/>)<br/>
	/// <see langword="await"/> reader.CloseAsync();<br/>
	/// <see langword="return"/> reader.RecordsAffected;
	/// </code>
	/// </summary>
	public static async Task<T> ExecuteReaderAsync<T>(this DbCommand @this, Func<DbDataReader, CancellationToken, ValueTask<T>> readData, CancellationToken token = default)
	{
		await using var reader = await @this.ExecuteReaderAsync(CommandBehavior.CloseConnection, token);
		var data = await readData(reader, token);
		await reader.CloseAsync();
		return data;
	}

	/// <summary>
	/// <code>
	/// <see langword="await using var"/> reader = <see langword="await"/> @<paramref name="this"/>.ExecuteReaderAsync(<paramref name="token"/>);<br/>
	/// <see langword="var"/> rowSet = <see langword="new"/> <see cref="RowSetResponse{T}"/>()<br/>
	/// {<br/>
	/// <see langword="    "/>Columns = reader.GetColumns(),<br/>
	/// <see langword="    "/>Rows = <see langword="await"/> reader.ReadRowsAsync&lt;<typeparamref name="T"/>&gt;(<paramref name="token"/>).ToArrayAsync(<paramref name="count"/>)<br/>
	/// };<br/>
	/// <see langword="await"/> reader.CloseAsync();<br/>
	/// rowSet.Count = reader.RecordsAffected;<br/>
	/// <see langword="return"/> rowSet;
	/// </code>
	/// </summary>
	public static async Task<RowSetResponse<T>> GetRowSetAsync<T>(this DbCommand @this, int count, CancellationToken token = default)
	{
		await using var reader = await @this.ExecuteReaderAsync(token);
		var columns = reader.GetColumns();
		var rows = await reader.ReadRowsAsync<T>(count, token);
		await reader.CloseAsync();
		return new()
		{
			Columns = columns,
			Count = reader.RecordsAffected,
			Rows = rows
		};
	}

	/// <summary>
	/// <c>=&gt; <see langword="await"/> @<paramref name="this"/>.ExecuteScalarAsync(<paramref name="token"/>) <see langword="is"/> <typeparamref name="T"/> value ? value : <see langword="default"/>;</c>
	/// </summary>
	public static async Task<T?> GetValueAsync<T>(this DbCommand @this, CancellationToken token = default)
		=> await @this.ExecuteScalarAsync(token) is T value ? value : default;
}
