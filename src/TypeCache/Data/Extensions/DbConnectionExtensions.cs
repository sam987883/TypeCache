// Copyright (c) 2021 Samuel Abraham

using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Requests;
using TypeCache.Data.Schema;
using TypeCache.Extensions;
using static System.FormattableString;

namespace TypeCache.Data.Extensions
{
	public static class DbConnectionExtensions
	{
		/// <summary>
		/// <code>command.CommandType = CommandType.StoredProcedure;</code>
		/// </summary>
		public static DbCommand CreateProcedureCommand(this DbConnection @this, string procedure)
		{
			var command = @this.CreateCommand();
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = procedure;
			return command;
		}

		/// <summary>
		/// <code>command.CommandType = CommandType.Text;</code>
		/// </summary>
		public static DbCommand CreateSqlCommand(this DbConnection @this, string sql)
		{
			var command = @this.CreateCommand();
			command.CommandType = CommandType.Text;
			command.CommandText = sql;
			return command;
		}

		public static async ValueTask<ObjectSchema> GetObjectSchema(this DbConnection @this, string name)
		{
			var objectName = name.Split('.').Get(^1)!.TrimStart('[').TrimEnd(']');
			var schemaName = name.Contains("..") ? (object)DBNull.Value : name.Split('.').Get(^2)!.TrimStart('[').TrimEnd(']');
			var request = new SqlRequest(ObjectSchema.SQL);
			request.Parameters.Add(ObjectSchema.OBJECT_NAME, objectName);
			request.Parameters.Add(ObjectSchema.SCHEMA_NAME, schemaName);
			var (tableRowSet, columnRowSet, parameterRowSet, _) = await @this.RunAsync(request);

			if (tableRowSet?.Rows.Any() is not true)
				throw new ArgumentException($"{nameof(DbConnection)}.{nameof(GetObjectSchema)}: Database object was not found.", objectName);

			var columns = Array<ColumnSchema>.Empty;
			if (columnRowSet?.Rows.Any() is true)
			{
				columns = 0.Range(columnRowSet!.Rows.Length).To(i => new ColumnSchema
				{
					Hidden = (bool)columnRowSet[i, nameof(ColumnSchema.Hidden)]!,
					Id = (int)columnRowSet[i, nameof(ColumnSchema.Id)]!,
					Identity = (bool)columnRowSet[i, nameof(ColumnSchema.Identity)]!,
					Length = (int)columnRowSet[i, nameof(ColumnSchema.Length)]!,
					Name = (string)columnRowSet[i, nameof(ColumnSchema.Name)]!,
					Nullable = (bool)columnRowSet[i, nameof(ColumnSchema.Nullable)]!,
					PrimaryKey = (bool)columnRowSet[i, nameof(ColumnSchema.PrimaryKey)]!,
					ReadOnly = (bool)columnRowSet[i, nameof(ColumnSchema.ReadOnly)]!,
					Type = (SqlDbType)columnRowSet[i, nameof(ColumnSchema.Type)]!
				}).ToArray();
			}

			var parameters = Array<ParameterSchema>.Empty;
			if (parameterRowSet?.Rows.Any() is true)
			{
				parameters = 0.Range(parameterRowSet!.Rows.Length).To(i => new ParameterSchema
				{
					Id = (int)parameterRowSet[i, nameof(ParameterSchema.Id)]!,
					Name = (string)parameterRowSet[i, nameof(ParameterSchema.Name)]!,
					Output = (bool)parameterRowSet[i, nameof(ParameterSchema.Output)]!,
					Return = (bool)parameterRowSet[i, nameof(ParameterSchema.Return)]!,
					Type = (SqlDbType)parameterRowSet[i, nameof(ParameterSchema.Type)]!
				}).ToArray();
			}

			return new ObjectSchema(@this.DataSource, tableRowSet, columns, parameters);
		}

		/// <summary>
		/// <code>EXECUTE ...</code>
		/// </summary>
		public static async ValueTask<RowSet[]> CallAsync(this DbConnection @this, StoredProcedureRequest request, CancellationToken cancellationToken = default)
		{
			await using var command = @this.CreateProcedureCommand(request.Procedure);
			request.Parameters.Do(_ => command.AddInputParameter(_.Key, _.Value));

			await using var reader = await command.ExecuteReaderAsync(cancellationToken);
			return (await reader.ReadRowSetsAsync(cancellationToken).ToListAsync(cancellationToken)).ToArray();
		}

		/// <summary>
		/// <code>...</code>
		/// </summary>
		public static async ValueTask<RowSet[]> RunAsync(this DbConnection @this, SqlRequest request, CancellationToken cancellationToken = default)
		{
			await using var command = @this.CreateSqlCommand(request.SQL);
			request.Parameters.Do(_ => command.AddInputParameter(_.Key, _.Value));

			await using var reader = await command.ExecuteReaderAsync(cancellationToken);
			return (await reader.ReadRowSetsAsync(cancellationToken).ToListAsync(cancellationToken)).ToArray();
		}

		/// <summary>
		/// <code>DELETE ... OUTPUT ... FROM ... WHERE ...</code>
		/// </summary>
		/// <returns><code>OUTPUT DELETED</code></returns>
		public static async ValueTask<RowSet> DeleteAsync(this DbConnection @this, DeleteRequest request, CancellationToken cancellationToken = default)
		{
			await using var command = @this.CreateSqlCommand(request.ToSQL());
			request.Parameters.Do(_ => command.AddInputParameter(_.Key, _.Value));

			if (request.Output.Any())
			{
				await using var reader = await command.ReadSingleResultAsync(cancellationToken);
				return await reader.ReadRowSetAsync(cancellationToken);
			}
			else
			{
				await command.ExecuteNonQueryAsync(cancellationToken);
				return RowSet.Empty;
			}
		}

		/// <summary>
		/// <code>DELETE x ... OUTPUT ... FROM ... INNER JOIN (VALUES ...) AS i ...</code>
		/// </summary>
		/// <returns><code>OUTPUT DELETED</code></returns>
		public static async ValueTask<RowSet> DeleteDataAsync(this DbConnection @this, DeleteDataRequest request, CancellationToken cancellationToken = default)
		{
			await using var command = @this.CreateSqlCommand(request.ToSQL());

			if (request.Output.Any())
			{
				await using var reader = await command.ReadSingleResultAsync(cancellationToken);
				return await reader.ReadRowSetAsync(cancellationToken);
			}
			else
			{
				await command.ExecuteNonQueryAsync(cancellationToken);
				return RowSet.Empty;
			}
		}

		/// <summary>
		/// <code>INSERT INTO ... SELECT ... FROM ... WHERE ... HAVING ... ORDER BY ...</code>
		/// </summary>
		/// <returns><code>OUTPUT INSERTED</code></returns>
		public static async ValueTask<RowSet> InsertAsync(this DbConnection @this, InsertRequest request, CancellationToken cancellationToken = default)
		{
			await using var command = @this.CreateSqlCommand(request.ToSQL());
			request.Parameters.Do(_ => command.AddInputParameter(_.Key, _.Value));

			if (request.Output.Any())
			{
				await using var reader = await command.ReadSingleResultAsync(cancellationToken);
				return await reader.ReadRowSetAsync(cancellationToken);
			}
			else
			{
				await command.ExecuteNonQueryAsync(cancellationToken);
				return RowSet.Empty;
			}
		}

		/// <summary>
		/// <code>INSERT INTO ... VALUES ...</code>
		/// </summary>
		/// <returns><code>OUTPUT INSERTED</code></returns>
		public static async ValueTask<RowSet> InsertDataAsync(this DbConnection @this, InsertDataRequest request, CancellationToken cancellationToken = default)
		{
			await using var command = @this.CreateSqlCommand(request.ToSQL());

			if (request.Output.Any())
			{
				await using var reader = await command.ReadSingleResultAsync(cancellationToken);
				return await reader.ReadRowSetAsync(cancellationToken);
			}
			else
			{
				await command.ExecuteNonQueryAsync(cancellationToken);
				return RowSet.Empty;
			}
		}

		/// <summary>
		/// <code>SELECT ... FROM ... WHERE ... HAVING ... ORDER BY ...</code>
		/// </summary>
		public static async ValueTask<RowSet> SelectAsync(this DbConnection @this, SelectRequest request, CancellationToken cancellationToken = default)
		{
			await using var command = @this.CreateSqlCommand(request.ToSQL());
			request.Parameters.Do(_ => command.AddInputParameter(_.Key, _.Value));

			await using var transaction = await command.BeginTransactionAsync(IsolationLevel.ReadUncommitted, cancellationToken);
			await using var reader = await command.ReadSingleResultAsync(cancellationToken);
			return await reader.ReadRowSetAsync(cancellationToken);
		}

		/// <summary>
		/// <code>TRUNCATE TABLE ...</code>
		/// </summary>
		public static async ValueTask<int> TruncateTableAsync(this DbConnection @this, string table, CancellationToken cancellationToken = default)
		{
			await using var command = @this.CreateSqlCommand(Invariant($"TRUNCATE TABLE {table.EscapeIdentifier()};"));
			return await command.ExecuteNonQueryAsync(cancellationToken);
		}

		/// <summary>
		/// <code>UPDATE ... SET ... OUTPUT ... WHERE ...</code>
		/// </summary>
		/// <returns><code>OUTPUT DELETED, INSERTED</code></returns>
		public static async ValueTask<RowSet> UpdateAsync(this DbConnection @this, UpdateRequest request, CancellationToken cancellationToken = default)
		{
			await using var command = @this.CreateSqlCommand(request.ToSQL());
			request.Parameters.Do(_ => command.AddInputParameter(_.Key, _.Value));

			if (request.Output.Any())
			{
				await using var reader = await command.ReadSingleResultAsync(cancellationToken);
				return await reader.ReadRowSetAsync(cancellationToken);
			}
			else
			{
				await command.ExecuteNonQueryAsync(cancellationToken);
				return RowSet.Empty;
			}
		}

		/// <summary>
		/// <code>UPDATE ... SET ... OUTPUT ... VALUES ...</code>
		/// </summary>
		/// <returns><code>OUTPUT DELETED, INSERTED</code></returns>
		public static async ValueTask<RowSet> UpdateDataAsync(this DbConnection @this, UpdateDataRequest request, CancellationToken cancellationToken = default)
		{
			await using var command = @this.CreateSqlCommand(request.ToSQL());

			if (request.Output.Any())
			{
				await using var reader = await command.ReadSingleResultAsync(cancellationToken);
				return await reader.ReadRowSetAsync(cancellationToken);
			}
			else
			{
				await command.ExecuteNonQueryAsync(cancellationToken);
				return RowSet.Empty;
			}
		}
	}
}
