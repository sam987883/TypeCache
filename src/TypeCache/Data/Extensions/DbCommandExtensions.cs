﻿// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Data.Extensions;

public static class DbCommandExtensions
{
	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    var"/> parameter = @<paramref name="this"/>.CreateParameter();<br/>
	/// <see langword="    "/>parameter.Direction = <see cref="ParameterDirection.Input"/>;<br/>
	/// <see langword="    "/>parameter.ParameterName = <paramref name="name"/>;<br/>
	/// <see langword="    "/>parameter.Value = <paramref name="value"/> ?? <see cref="DBNull.Value"/>;<br/>
	/// <see langword="    return"/> @<paramref name="this"/>.Parameters.Add(parameter);<br/>
	/// }
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
	/// {<br/>
	/// <see langword="    var"/> parameter = @<paramref name="this"/>.CreateParameter();<br/>
	/// <see langword="    "/>parameter.Direction = <see cref="ParameterDirection.InputOutput"/>;<br/>
	/// <see langword="    "/>parameter.ParameterName = <paramref name="name"/>;<br/>
	/// <see langword="    "/>parameter.Value = <paramref name="value"/> ?? <see cref="DBNull.Value"/>;<br/>
	/// <see langword="    "/>parameter.DbType = <paramref name="dbType"/>;<br/>
	/// <see langword="    return"/> @<paramref name="this"/>.Parameters.Add(parameter);<br/>
	/// }
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
	/// {<br/>
	/// <see langword="    var"/> parameter = @<paramref name="this"/>.CreateParameter();<br/>
	/// <see langword="    "/>parameter.Direction = <see cref="ParameterDirection.Output"/>;<br/>
	/// <see langword="    "/>parameter.ParameterName = <paramref name="name"/>;<br/>
	/// <see langword="    "/>parameter.DbType = <paramref name="dbType"/>;<br/>
	/// <see langword="    return"/> @<paramref name="this"/>.Parameters.Add(parameter);<br/>
	/// }
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
	/// =&gt; @<paramref name="this"/>.Parameters.If&lt;<see cref="DbParameter"/>&gt;()<br/>
	/// <see langword="    "/>.If(parameter =&gt; parameter.Direction <see langword="is"/> <see cref="ParameterDirection.Output"/> || parameter.Direction <see langword="is"/> <see cref="ParameterDirection.InputOutput"/>)<br/>
	/// <see langword="    "/>.Do(parameter => <paramref name="sqlCommand"/>.Parameters[parameter.ParameterName] = parameter.Value);
	/// </code>
	/// </summary>
	public static void CopyOutputParameters(this DbCommand @this, SqlCommand sqlCommand)
		=> @this.Parameters.If<DbParameter>()
			.If(parameter => parameter.Direction is ParameterDirection.InputOutput || parameter.Direction is ParameterDirection.Output)
			.Do(parameter => sqlCommand.Parameters[parameter.ParameterName] = parameter.Value);

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    await using var"/> reader = <see langword="await"/> @<paramref name="this"/>.ExecuteReaderAsync(<paramref name="token"/>);<br/>
	/// <see langword="    await"/> <paramref name="readData"/>(reader, <paramref name="token"/>)<br/>
	/// <see langword="    await"/> reader.CloseAsync();<br/>
	/// <see langword="    return"/> reader.RecordsAffected;<br/>
	/// }
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
	/// {<br/>
	/// <see langword="    await using var"/> reader = <see langword="await"/> @<paramref name="this"/>.ExecuteReaderAsync(<paramref name="token"/>);<br/>
	/// <see langword="    await"/> <paramref name="readData"/>(reader, <paramref name="token"/>)<br/>
	/// <see langword="    await"/> reader.CloseAsync();<br/>
	/// <see langword="    return"/> reader.RecordsAffected;<br/>
	/// }
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
	/// {<br/>
	/// <see langword="    await using var"/> reader = <see langword="await"/> @<paramref name="this"/>.ExecuteReaderAsync(<see cref="CommandBehavior.SingleResult"/>, <paramref name="token"/>);<br/>
	/// <see langword="    return"/> reader.ReadDataTable();<br/>
	/// }
	/// </code>
	/// </summary>
	public static async Task<DataTable> GetDataTableAsync(this DbCommand @this, CancellationToken token = default)
	{
		await using var reader = await @this.ExecuteReaderAsync(CommandBehavior.SingleResult, token);
		return reader.ReadDataTable();
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    await using var"/> reader = <see langword="await"/> @<paramref name="this"/>.ExecuteReaderAsync(<see cref="CommandBehavior.SingleResult"/>, <paramref name="token"/>);<br/>
	/// <see langword="    return await"/> reader.ReadJsonArrayAsync(<paramref name="token"/>);<br/>
	/// }
	/// </code>
	/// </summary>
	public static async Task<JsonArray> GetJsonArrayAsync(this DbCommand @this, CancellationToken token = default)
	{
		await using var reader = await @this.ExecuteReaderAsync(CommandBehavior.SingleResult, token);
		return await reader.ReadJsonArrayAsync(token);
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    await using var"/> reader = <see langword="await"/> @<paramref name="this"/>.ExecuteReaderAsync(<see cref="CommandBehavior.SingleResult"/>, <paramref name="token"/>);<br/>
	/// <see langword="    await"/> reader.ReadJsonAsync(<paramref name="writer"/>, <paramref name="token"/>);<br/>
	/// }
	/// </code>
	/// </summary>
	public static async Task GetJsonAsync(this DbCommand @this, Utf8JsonWriter writer, CancellationToken token = default)
	{
		await using var reader = await @this.ExecuteReaderAsync(CommandBehavior.SingleResult, token);
		await reader.ReadJsonAsync(writer, token);
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    await using var"/> reader = <see langword="await"/> @<paramref name="this"/>.ExecuteReaderAsync(<paramref name="token"/>);<br/>
	/// <see langword="    var"/> rows = <see langword="await"/> reader.ReadModelsAsync&lt;<typeparamref name="T"/>&gt;(<paramref name="count"/>, <paramref name="token"/>);<br/>
	/// <see langword="    await"/> reader.CloseAsync();<br/>
	/// <see langword="    "/>@<paramref name="this"/>.AddInputParameter(<see langword="nameof"/>(reader.RecordsAffected), reader.RecordsAffected);<br/>
	/// <see langword="    return"/> rows;<br/>
	/// }
	/// </code>
	/// </summary>
	public static async Task<IList<T>> GetModelsAsync<T>(this DbCommand @this, int initialCapacity = 0, CancellationToken token = default)
		where T : new()
	{
		await using var reader = await @this.ExecuteReaderAsync(token);
		var rows = await reader.ReadModelsAsync<T>(initialCapacity, token);
		await reader.CloseAsync();
		@this.AddInputParameter(nameof(reader.RecordsAffected), reader.RecordsAffected);
		return rows;
	}

	/// <inheritdoc cref="DbCommand.ExecuteScalarAsync(CancellationToken)"/>
	/// <remarks>
	/// <c>=&gt; <see langword="await"/> @<paramref name="this"/>.ExecuteScalarAsync(<paramref name="token"/>)<br/>
	/// {<br/>
	/// <see langword="    "/><see cref="DBNull"/> <see langword="or null"/> =&gt; <see langword="null"/>,<br/>
	/// <see langword="    "/><see cref="string"/> value =&gt; value,<br/>
	/// <see langword="    "/><see cref="object"/> value =&gt; value.ToString()<br/>
	/// };<br/>
	/// </c>
	/// </remarks>
	public static async Task<string?> GetStringAsync(this DbCommand @this, CancellationToken token = default)
		=> await @this.ExecuteScalarAsync(token) switch
		{
			DBNull or null => null,
			string value => value,
			object value => value.ToString()
		};

	/// <inheritdoc cref="DbCommand.ExecuteScalarAsync(CancellationToken)"/>
	/// <remarks>
	/// <c>=&gt; <see langword="await"/> @<paramref name="this"/>.ExecuteScalarAsync(<paramref name="token"/>)<br/>
	/// {<br/>
	/// <see langword="    "/><typeparamref name="T"/> value =&gt; value,<br/>
	/// <see langword="    "/>_ =&gt; <see langword="null"/><br/>
	/// };<br/>
	/// </c>
	/// </remarks>
	public static async Task<T?> GetValueAsync<T>(this DbCommand @this, CancellationToken token = default)
		where T : unmanaged
		=> await @this.ExecuteScalarAsync(token) switch
		{
			T value => value,
			_ => null
		};
}
