// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Domain;
using TypeCache.Extensions;
using TypeCache.Reflection;
using static System.FormattableString;

namespace TypeCache.Data.Extensions;

public static class DbConnectionExtensions
{
	/// <summary>
	/// <code>
	/// <see langword="var"/> command = @<paramref name="this"/>.CreateCommand();<br/>
	/// command.CommandType = <paramref name="sqlCommand"/>.CommandType;<br/>
	/// command.CommandText = <paramref name="sqlCommand"/>.ToSQL();<br/>
	/// <paramref name="sqlCommand"/>.InputParameters.Keys<br/>
	/// <see langword="    "/>.Without(<paramref name="sqlCommand"/>.OutputParameters.Keys)<br/>
	/// <see langword="    "/>.Do(parameter => command.AddInputParameter(parameter, <paramref name="sqlCommand"/>.InputParameters[parameter]));<br/>
	/// <paramref name="sqlCommand"/>.InputParameters.Keys<br/>
	/// <see langword="    "/>.Match(<paramref name="sqlCommand"/>.OutputParameters.Keys)<br/>
	/// <see langword="    "/>.Do(parameter => command.AddInputOutputParameter(parameter, <paramref name="sqlCommand"/>.InputParameters[parameter], <paramref name="sqlCommand"/>.OutputParameters[parameter]));<br/>
	/// <paramref name="sqlCommand"/>.OutputParameters.Keys<br/>
	/// <see langword="    "/>.Without(<paramref name="sqlCommand"/>.InputParameters.Keys)<br/>
	/// <see langword="    "/>.Do(parameter => command.AddOutputParameter(parameter, <paramref name="sqlCommand"/>.OutputParameters[parameter]));<br/>
	/// <see langword="return"/> command;
	/// </code>
	/// </summary>
	public static DbCommand CreateCommand(this DbConnection @this, Command sqlCommand)
	{
		var command = @this.CreateCommand();
		command.CommandType = sqlCommand.CommandType;
		command.CommandText = sqlCommand.ToSQL();
		sqlCommand.InputParameters.Keys
			.Without(sqlCommand.OutputParameters.Keys)
			.Do(parameter => command.AddInputParameter(parameter, sqlCommand.InputParameters[parameter]));
		sqlCommand.InputParameters.Keys
			.Match(sqlCommand.OutputParameters.Keys)
			.Do(parameter => command.AddInputOutputParameter(parameter, sqlCommand.InputParameters[parameter], sqlCommand.OutputParameters[parameter]));
		sqlCommand.OutputParameters.Keys
			.Without(sqlCommand.InputParameters.Keys)
			.Do(parameter => command.AddOutputParameter(parameter, sqlCommand.OutputParameters[parameter]));
		return command;
	}

	/// <summary>
	/// <code>
	/// <see langword="await using var"/> command = @<paramref name="this"/>.CreateSqlCommand(<paramref name="sql"/>, <paramref name="parameters"/>);<br/>
	/// <see langword="return await"/> command.ExecuteNonQueryAsync(<paramref name="token"/>);
	/// </code>
	/// </summary>
	public static async Task<int> ExecuteAsync(this DbConnection @this, Command sqlCommand, CancellationToken token = default)
	{
		await using var command = @this.CreateCommand(sqlCommand);
		return await command.ExecuteNonQueryAsync(token);
	}

	/// <summary>
	/// <code>
	/// <paramref name="readData"/>.AssertNotNull();<br/>
	/// <see langword="await using var"/> command = @<paramref name="this"/>.CreateCommand(<paramref name="sqlCommand"/>);<br/>
	/// <see langword="await"/> @<paramref name="this"/>.OpenAsync(<paramref name="token"/>);<br/>
	/// <see langword="return await"/> command.ExecuteReaderAsync&lt;<typeparamref name="T"/>&gt;(<paramref name="readData"/>, <paramref name="token"/>);
	/// </code>
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public static async Task<object> ExecuteAsync(this DbConnection @this, Command sqlCommand, Func<DbDataReader, CancellationToken, ValueTask<object>> readData, CancellationToken token = default)
	{
		readData.AssertNotNull();
		await using var command = @this.CreateCommand(sqlCommand);
		return await command.ExecuteReaderAsync<object>(readData, token);
	}

	/// <summary>
	/// <code>
	/// <see langword="await using var"/> command = @<paramref name="this"/>.CreateSqlCommand(<paramref name="sql"/>, <paramref name="parameters"/>);<br/>
	/// <see langword="await"/> reader.GetRowSet&lt;<typeparamref name="T"/>&gt;(<paramref name="count"/>, <paramref name="token"/>);
	/// </code>
	/// </summary>
	public static async Task<RowSetResponse<T>> GetRowSetAsync<T>(this DbConnection @this, Command sqlCommand, int count, CancellationToken token = default)
	{
		await using var command = @this.CreateCommand(sqlCommand);
		return await command.GetRowSetAsync<T>(count, token);
	}

	/// <summary>
	/// <code>
	/// <see langword="await using var"/> command = @<paramref name="this"/>.CreateSqlCommand(<paramref name="sql"/>, <paramref name="parameters"/>);<br/>
	/// <see langword="return await"/> command.ExecuteScalarAsync(<paramref name="token"/>);
	/// </code>
	/// </summary>
	public static async Task<T?> GetValueAsync<T>(this DbConnection @this, Command sqlCommand, CancellationToken token = default)
	{
		await using var command = @this.CreateCommand(sqlCommand);
		return await command.GetValueAsync<T>(token);
	}

	/// <summary>
	/// Get the SQL Server [object_id] for a database object.
	/// </summary>
	public static async Task<int?> GetObjectId(this DbConnection @this, string name, CancellationToken token = default)
	{
		var command = new SqlCommand
		{
			DataSource = @this.DataSource,
			SQL = Invariant(@$"
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SET NOCOUNT ON;

SELECT OBJECT_ID(@{nameof(name)});
")
		};
		command.InputParameters.Add(nameof(name), name);

		await using var dbCommand = @this.CreateCommand(command);
		return await dbCommand.GetValueAsync<int?>(token);
	}

	/// <summary>
	/// <code>
	/// <see langword="await using var"/> command = @<paramref name="this"/>.CreateSqlCommand(<paramref name="sqlCommand"/>.ToSQL(), <paramref name="sqlCommand"/>.Parameters);<br/>
	/// <see langword="return await"/> command.GetValueAsync&lt;<see cref="long"/>&gt;(<paramref name="sqlCommand"/>.Token);
	/// </code>
	/// </summary>
	public static async Task<long> CountAsync(this DbConnection @this, CountCommand sqlCommand, CancellationToken token = default)
	{
		await using var command = @this.CreateCommand(sqlCommand);
		return await command.GetValueAsync<long>(token);
	}

	/// <summary>
	/// <code>
	/// <see langword="var"/> capacity = <paramref name="sqlCommand"/>.Output.Any() ? (<see cref="int"/>)<see langword="await"/> @<paramref name="this"/>.CountAsync(<paramref name="sqlCommand"/>.ToCountRequest()) : 0;<br/>
	/// <br/>
	/// <see langword="await using var"/> command = @<paramref name="this"/>.CreateSqlCommand(<paramref name="sqlCommand"/>.ToSQL(), <paramref name="sqlCommand"/>.Parameters);<br/>
	/// <see langword="if"/> (<paramref name="sqlCommand"/>.Output.Any())<br/>
	/// <see langword="    return await"/> @<paramref name="this"/>.GetRowSetAsync&lt;<typeparamref name="T"/>&gt;(capacity, <paramref name="sqlCommand"/>.Token);<br/>
	/// <br/>
	/// <see langword="return new"/>()<br/>
	/// {<br/>
	/// <see langword="    "/>Count = (<see cref="long"/>)(<see langword="await"/> command.GetValueAsync(<paramref name="sqlCommand"/>.Token) ?? 0L)<br/>
	/// };
	/// </code>
	/// </summary>
	/// <returns><code>OUTPUT DELETED</code></returns>
	public static async Task<RowSetResponse<T>> DeleteAsync<T>(this DbConnection @this, DeleteCommand sqlCommand, CancellationToken token = default)
	{
		await using var command = @this.CreateCommand(sqlCommand);
		if (sqlCommand.Output.Any())
		{
			await using var countCommand = @this.CreateCommand(sqlCommand.ToCountCommand());
			var count = (int)await countCommand.GetValueAsync<long>();
			return await command.GetRowSetAsync<T>(count, token);
		}
		return new() { Count = await command.ExecuteNonQueryAsync(token) };
	}

	/// <summary>
	/// <code>
	/// <see langword="await using var"/> command = @<paramref name="this"/>.CreateSqlCommand(<paramref name="sqlCommand"/>.ToSQL());<br/>
	/// <see langword="if"/> (<paramref name="sqlCommand"/>.Output.Any())<br/>
	/// <see langword="    return await"/> command.GetRowSetAsync&lt;<typeparamref name="T"/>&gt;(<paramref name="sqlCommand"/>.Input.Count, <paramref name="sqlCommand"/>.Token);<br/>
	/// <br/>
	/// <see langword="return new"/>()<br/>
	/// {<br/>
	/// <see langword="    "/>Count = (<see cref="long"/>)(<see langword="await"/> command.GetValueAsync(<paramref name="sqlCommand"/>.Token) ?? 0L)<br/>
	/// };
	/// </code>
	/// </summary>
	/// <returns><code>OUTPUT DELETED</code></returns>
	public static async Task<RowSetResponse<T>> DeleteDataAsync<T>(this DbConnection @this, DeleteDataCommand<T> sqlCommand, CancellationToken token = default)
	{
		await using var command = @this.CreateCommand(sqlCommand);
		return sqlCommand.Output.Any()
			? await command.GetRowSetAsync<T>(sqlCommand.Input.Length, token)
			: new() { Count = await command.ExecuteNonQueryAsync(token) };
	}

	/// <summary>
	/// <code>
	/// <see langword="var"/> capacity = <paramref name="sqlCommand"/>.Output.Any() ? (<see cref="int"/>)<see langword="await"/> @<paramref name="this"/>.CountAsync(<paramref name="sqlCommand"/>.ToCountRequest()) : 0;<br/>
	/// <br/>
	/// <see langword="await using var"/> command = @<paramref name="this"/>.CreateSqlCommand(<paramref name="sqlCommand"/>.ToSQL(), <paramref name="sqlCommand"/>.Parameters);<br/>
	/// <see langword="if"/> (<paramref name="sqlCommand"/>.Output.Any())<br/>
	/// <see langword="    return await"/> @<paramref name="this"/>.GetRowSetAsync&lt;<typeparamref name="T"/>&gt;(capacity, <paramref name="sqlCommand"/>.Token);<br/>
	/// <br/>
	/// <see langword="return new"/>()<br/>
	/// {<br/>
	/// <see langword="    "/>Count = (<see cref="long"/>)(<see langword="await"/> command.GetValueAsync(<paramref name="sqlCommand"/>.Token) ?? 0L)<br/>
	/// };
	/// </code>
	/// </summary>
	/// <returns><code>OUTPUT INSERTED</code></returns>
	public static async Task<RowSetResponse<T>> InsertAsync<T>(this DbConnection @this, InsertCommand sqlCommand, CancellationToken token = default)
	{
		await using var command = @this.CreateCommand(sqlCommand);
		if (sqlCommand.Output.Any())
		{
			await using var countCommand = @this.CreateCommand(sqlCommand.ToCountCommand());
			var capacity = (int)await countCommand.GetValueAsync<long>();
			return await command.GetRowSetAsync<T>(capacity, token);
		}
		return new() { Count = await command.ExecuteNonQueryAsync(token) };
	}

	/// <summary>
	/// <code>
	/// <see langword="await using var"/> command = @<paramref name="this"/>.CreateSqlCommand(<paramref name="sqlCommand"/>.ToSQL());<br/>
	/// <see langword="if"/> (<paramref name="sqlCommand"/>.Output.Any())<br/>
	/// <see langword="    return await"/> command.GetRowSetAsync&lt;<typeparamref name="T"/>&gt;(<paramref name="sqlCommand"/>.Input.Length, <paramref name="sqlCommand"/>.Token);<br/>
	/// <br/>
	/// <see langword="return new"/>()<br/>
	/// {<br/>
	/// <see langword="    "/>Count = (<see cref="long"/>)(<see langword="await"/> command.GetValueAsync(<paramref name="sqlCommand"/>.Token) ?? 0L)<br/>
	/// };
	/// </code>
	/// </summary>
	/// <returns><code>OUTPUT INSERTED</code></returns>
	public static async Task<RowSetResponse<T>> InsertDataAsync<T>(this DbConnection @this, InsertDataCommand<T> sqlCommand, CancellationToken token = default)
	{
		await using var command = @this.CreateCommand(sqlCommand);
		return sqlCommand.Output.Any()
			? await command.GetRowSetAsync<T>(sqlCommand.Input.Length, token)
			: new() { Count = await command.ExecuteNonQueryAsync(token) };
	}

	/// <summary>
	/// <code>
	/// <see langword="await using var"/> command = @<paramref name="this"/>.CreateSqlCommand(<paramref name="sqlCommand"/>.ToSQL(), <paramref name="sqlCommand"/>.Parameters);<br/>
	/// <br/>
	/// <see langword="if"/> (<paramref name="sqlCommand"/>.Pager.HasValue)<br/>
	/// {<br/>
	/// <see langword="    const"/> <see cref="string"/> ROW_COUNT = "RowCount";<br/>
	/// <see langword="    "/>command.AddOutputParameter(ROW_COUNT, <see cref="DbType.Int64"/>);<br/>
	/// <see langword="    var"/> data = <see langword="await"/> command.GetRowSet&lt;<typeparamref name="T"/>&gt;((<see cref="int"/>)<paramref name="sqlCommand"/>.Pager.Value.First, <paramref name="sqlCommand"/>.Token);<br/>
	/// <see langword="    "/>data.Count = (<see cref="long"/>)command.Parameters[ROW_COUNT].Value;<br/>
	/// <see langword="    return"/> data;<br/>
	/// }<br/>
	/// <see langword="else"/><br/>
	/// {<br/>
	/// <see langword="    var"/> count = (<see cref="int"/>)<see langword="await"/> @<paramref name="this"/>.CountAsync(<paramref name="sqlCommand"/>.ToCountCommand());<br/>
	/// <see langword="    return await"/> reader.GetRowSet&lt;<typeparamref name="T"/>&gt;(count, <paramref name="sqlCommand"/>.Token);<br/>
	/// }
	/// </code>
	/// </summary>
	public static async Task<RowSetResponse<T>> SelectAsync<T>(this DbConnection @this, SelectCommand sqlCommand, CancellationToken token = default)
	{
		await using var command = @this.CreateCommand(sqlCommand);

		if (sqlCommand.Pager.HasValue)
		{
			const string ROW_COUNT = "RowCount";
			command.AddOutputParameter(ROW_COUNT, DbType.Int64);
			var data = await command.GetRowSetAsync<T>((int)sqlCommand.Pager.Value.First, token);
			data.Count = (long)command.Parameters[ROW_COUNT].Value!;
			return data;
		}
		else
		{
			var count = (int)await @this.CountAsync(sqlCommand.ToCountCommand());
			return await command.GetRowSetAsync<T>(count, token);
		}
	}

	/// <summary>
	/// <code>
	/// <see langword="await using var"/> command = @<paramref name="this"/>.CreateSqlCommand(Invariant($"TRUNCATE TABLE {<paramref name="sqlCommand"/>.Table.EscapeIdentifier()};"));<br/>
	/// <see langword="return await"/> command.ExecuteNonQueryAsync(<paramref name="sqlCommand"/>.Token);
	/// </code>
	/// </summary>
	public static async ValueTask<int> TruncateTableAsync(this DbConnection @this, TruncateTableCommand sqlCommand, CancellationToken token = default)
	{
		await using var command = @this.CreateCommand(sqlCommand);
		return await command.ExecuteNonQueryAsync(token);
	}

	/// <summary>
	/// <code>
	/// <see langword="await using var"/> command = @<paramref name="this"/>.CreateSqlCommand(<paramref name="sqlCommand"/>.ToSQL(), <paramref name="sqlCommand"/>.Parameters);<br/>
	/// <see langword="if"/> (<paramref name="sqlCommand"/>.Output.Any())<br/>
	/// <see langword="    return await"/> @<paramref name="this"/>.GetRowSetAsync(<paramref name="sqlCommand"/>.Token);<br/>
	/// <br/>
	/// <see langword="return new"/>()<br/>
	/// {<br/>
	/// <see langword="    "/>Count = (<see cref="long"/>)(<see langword="await"/> command.GetValueAsync(<paramref name="sqlCommand"/>.Token) ?? 0L)<br/>
	/// };
	/// </code>
	/// </summary>
	/// <returns><code>OUTPUT DELETED, INSERTED</code></returns>
	public static async ValueTask<UpdateRowSetResponse<T>> UpdateAsync<T>(this DbConnection @this, UpdateCommand sqlCommand, CancellationToken token = default)
	{
		await using var command = @this.CreateCommand(sqlCommand);
		if (sqlCommand.Output.Any())
		{
			await using var countCommand = @this.CreateCommand(sqlCommand.ToCountCommand());
			var count = (int)await countCommand.GetValueAsync<long>();
			var anyDeleted = sqlCommand.Columns.AnyLeft("DELETED.");
			var anyInserted = sqlCommand.Columns.AnyLeft("INSERTED.");
			if (anyDeleted && anyInserted)
			{
				var rowSet = await command.GetRowSetAsync<object[]>(count, token);

				var propertyMap = TypeOf<T>.Properties.ToDictionary(property => property.Name, property => property);
				var deletedColumns = new List<(PropertyMember Property, int Index)>(sqlCommand.Columns.Length);
				var insertedColumns = new List<(PropertyMember Property, int Index)>(sqlCommand.Columns.Length);
				sqlCommand.Columns.Do((column, i) =>
				{
					if (column.StartsWith("DELETED."))
						deletedColumns.Add((propertyMap[column.TrimStart("DELETED.")], i));
					else if (column.StartsWith("INSERTED."))
						insertedColumns.Add((propertyMap[column.TrimStart("INSERTED.")], i));
				});

				var deleted = rowSet.Rows.Map(row =>
				{
					var item = TypeOf<T>.Create();
					deletedColumns.Do(column => column.Property.SetValue(item, row[column.Index]));
					return item;
				}).ToArray();
				var inserted = rowSet.Rows.Map(row =>
				{
					var item = TypeOf<T>.Create();
					insertedColumns.Do(column => column.Property.SetValue(item, row[column.Index]));
					return item;
				}).ToArray();

				return new()
				{
					Columns = rowSet.Columns,
					Count = rowSet.Count,
					Deleted = deleted!,
					Inserted = inserted!
				};
			}
			else
			{
				var rowSet = await command.GetRowSetAsync<T>(count, token);

				return new()
				{
					Columns = rowSet.Columns,
					Count = rowSet.Count,
					Deleted = anyDeleted ? rowSet.Rows : Array<T>.Empty,
					Inserted = anyInserted ? rowSet.Rows : Array<T>.Empty
				};
			}
		}
		return new() { Count = await command.ExecuteNonQueryAsync(token) };
	}

	/// <summary>
	/// <code>
	/// <see langword="await using var"/> command = @<paramref name="this"/>.CreateSqlCommand(<paramref name="sqlCommand"/>.ToSQL());<br/>
	/// <see langword="if"/> (<paramref name="sqlCommand"/>.Output.Any())<br/>
	/// <see langword="    return await"/> command.GetRowSetAsync(<paramref name="sqlCommand"/>.Token);<br/>
	/// <br/>
	/// <see langword="return new"/>()<br/>
	/// {<br/>
	/// <see langword="    "/>Count = (<see cref="long"/>)(<see langword="await"/> command.GetValueAsync(<paramref name="sqlCommand"/>.Token) ?? 0L)<br/>
	/// };
	/// </code>
	/// </summary>
	/// <returns><code>OUTPUT DELETED, INSERTED</code></returns>
	public static async ValueTask<UpdateRowSetResponse<T>> UpdateDataAsync<T>(this DbConnection @this, UpdateDataCommand<T> sqlCommand, CancellationToken token = default)
	{
		await using var command = @this.CreateCommand(sqlCommand);
		if (sqlCommand.Output.Any())
		{
			var anyDeleted = sqlCommand.Columns.AnyLeft("DELETED.");
			var anyInserted = sqlCommand.Columns.AnyLeft("INSERTED.");
			if (anyDeleted && anyInserted)
			{
				var rowSet = await command.GetRowSetAsync<object[]>(sqlCommand.Input.Length, token);

				var propertyMap = TypeOf<T>.Properties.ToDictionary(property => property.Name, property => property);
				var deletedColumns = new List<(PropertyMember Property, int Index)>(sqlCommand.Columns.Length);
				var insertedColumns = new List<(PropertyMember Property, int Index)>(sqlCommand.Columns.Length);
				sqlCommand.Columns.Do((column, i) =>
				{
					if (column.StartsWith("DELETED."))
						deletedColumns.Add((propertyMap[column.TrimStart("DELETED.")], i));
					else if (column.StartsWith("INSERTED."))
						insertedColumns.Add((propertyMap[column.TrimStart("INSERTED.")], i));
				});

				var deleted = rowSet.Rows.Map(row =>
				{
					var item = TypeOf<T>.Create();
					deletedColumns.Do(column => column.Property.SetValue(item, row[column.Index]));
					return item;
				}).ToArray();
				var inserted = rowSet.Rows.Map(row =>
				{
					var item = TypeOf<T>.Create();
					insertedColumns.Do(column => column.Property.SetValue(item, row[column.Index]));
					return item;
				}).ToArray();

				return new()
				{
					Columns = rowSet.Columns,
					Count = rowSet.Count,
					Deleted = deleted!,
					Inserted = inserted!
				};
			}
			else
			{
				var rowSet = await command.GetRowSetAsync<T>(sqlCommand.Input.Length, token);

				return new()
				{
					Columns = rowSet.Columns,
					Count = rowSet.Count,
					Deleted = anyDeleted ? rowSet.Rows : Array<T>.Empty,
					Inserted = anyInserted ? rowSet.Rows : Array<T>.Empty
				};
			}
		}
		else
			return new() { Count = await command.ExecuteNonQueryAsync(token) };
	}
}
