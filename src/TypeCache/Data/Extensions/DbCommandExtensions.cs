// Copyright (c) 2021 Samuel Abraham

using System.Data;
using System.Data.Common;
using System.Text.Json;
using System.Text.Json.Nodes;

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
	/// {<br/>
	/// <see langword="    var"/> parameters = @<paramref name="this"/>.Parameters<br/>
	/// <see langword="        "/>.OfType&lt;<see cref="DbParameter"/>&gt;()<br/>
	/// <see langword="        "/>.Where(parameter =&gt; parameter.Direction <see langword="is"/> <see cref="ParameterDirection.InputOutput"/> || parameter.Direction <see langword="is"/> <see cref="ParameterDirection.Output"/>);<br/>
	/// <br/>
	/// <see langword="    foreach"/> (<see langword="var"/> parameter <see langword="in"/> parameters)<br/>
	/// <see langword="        "/><paramref name="sqlCommand"/>.Parameters[parameter.ParameterName] = parameter.Value;<br/>
	/// }
	/// </code>
	/// </summary>
	public static void CopyOutputParameters(this DbCommand @this, SqlCommand sqlCommand)
	{
		var parameters = @this.Parameters
			.OfType<DbParameter>()
			.Where(parameter => parameter.Direction is ParameterDirection.InputOutput || parameter.Direction is ParameterDirection.Output);

		foreach (var parameter in parameters)
			sqlCommand.Parameters[parameter.ParameterName] = parameter.Value;
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
	/// <see langword="    var"/> rows = <see langword="new"/> <see cref="List{T}"/>(<paramref name="listInitialCapacity"/>);<br/>
	/// <see langword="    await using var"/> reader = <see langword="await"/> @<paramref name="this"/>.ExecuteReaderAsync(<paramref name="token"/>);<br/>
	/// <see langword="    await"/> reader.ReadModelsAsync&lt;<typeparamref name="T"/>&gt;(rows, <paramref name="token"/>);<br/>
	/// <see langword="    await"/> reader.CloseAsync();<br/>
	/// <see langword="    "/>@<paramref name="this"/>.AddInputParameter(<see langword="nameof"/>(reader.RecordsAffected), reader.RecordsAffected);<br/>
	/// <see langword="    return"/> rows;<br/>
	/// }
	/// </code>
	/// </summary>
	public static async Task<IList<T>> GetModelsAsync<T>(this DbCommand @this, int listInitialCapacity, CancellationToken token = default)
		where T : new()
	{
		var rows = new List<T>(listInitialCapacity);
		await using var reader = await @this.ExecuteReaderAsync(token);
		await reader.ReadModelsAsync<T>(rows, token);
		await reader.CloseAsync();
		@this.AddInputParameter(nameof(reader.RecordsAffected), reader.RecordsAffected);
		return rows;
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    var"/> rows = <see langword="new"/> List&lt;<see cref="object"/>&gt;(<paramref name="listInitialCapacity"/>);<br/>
	/// <see langword="    await using var"/> reader = <see langword="await"/> @<paramref name="this"/>.ExecuteReaderAsync(<paramref name="token"/>);<br/>
	/// <see langword="    await"/> reader.ReadModelsAsync(modelType, rows, <paramref name="token"/>);<br/>
	/// <see langword="    await"/> reader.CloseAsync();<br/>
	/// <see langword="    "/>@<paramref name="this"/>.AddInputParameter(<see langword="nameof"/>(reader.RecordsAffected), reader.RecordsAffected);<br/>
	/// <see langword="    return"/> rows;<br/>
	/// }
	/// </code>
	/// </summary>
	public static async Task<IList<object>> GetModelsAsync(this DbCommand @this, Type modelType, int listInitialCapacity, CancellationToken token = default)
	{
		var rows = new List<object>(listInitialCapacity);
		await using var reader = await @this.ExecuteReaderAsync(token);
		await reader.ReadModelsAsync(modelType, rows, token);
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
