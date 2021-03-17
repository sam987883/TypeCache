// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Data.Extensions
{
	public static class DbConnectionExtensions
	{
		private static readonly LazyDictionary<string, ConcurrentDictionary<string, ObjectSchema>> SchemaCache =
			new LazyDictionary<string, ConcurrentDictionary<string, ObjectSchema>>(connectionString => new ConcurrentDictionary<string, ObjectSchema>(StringComparer.OrdinalIgnoreCase), StringComparer.OrdinalIgnoreCase);

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

		/// <summary>
		/// EXECUTE ...
		/// </summary>
		public static async ValueTask<RowSet[]> CallAsync(this DbConnection @this, StoredProcedureRequest procedure, CancellationToken cancellationToken = default)
		{
			await using var command = @this.CreateProcedureCommand(procedure.Procedure);
			procedure.Parameters.Do(parameter => command.AddInputParameter(parameter.Name, parameter.Value));

			await using var reader = await command.ExecuteReaderAsync(cancellationToken);
			return (await reader.ReadRowSetsAsync(cancellationToken).ToListAsync(cancellationToken)).ToArray();
		}

		/// <summary>
		/// EXECUTE ...
		/// </summary>
		public static async ValueTask<RowSet[]> RunAsync(this DbConnection @this, SqlRequest request, CancellationToken cancellationToken = default)
		{
			await using var command = @this.CreateSqlCommand(request.SQL);
			request.Parameters.Do(parameter => command.AddInputParameter(parameter.Name, parameter.Value));

			await using var reader = await command.ExecuteReaderAsync(cancellationToken);
			return (await reader.ReadRowSetsAsync(cancellationToken).ToListAsync(cancellationToken)).ToArray();
		}

		/// <summary>
		/// DELETE FROM ... WHERE ...
		/// </summary>
		/// <returns>OUTPUT DELETED</returns>
		public static async ValueTask<RowSet> DeleteAsync(this DbConnection @this, DeleteRequest delete, CancellationToken cancellationToken = default)
		{
			await using var command = @this.CreateSqlCommand(delete.ToSql());
			delete.Parameters.Do(parameter => command.AddInputParameter(parameter.Name, parameter.Value));

			if (delete.Output.Any())
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
		/// SELECT ... FROM ... WHERE ... HAVING ... ORDER BY ...
		/// </summary>
		public static async ValueTask<RowSet> InsertAsync(this DbConnection @this, InsertRequest insert, CancellationToken cancellationToken = default)
		{
			await using var command = @this.CreateSqlCommand(insert.ToSql());
			insert.Parameters.Do(parameter => command.AddInputParameter(parameter.Name, parameter.Value));

			if (insert.Output.Any())
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
		/// <code>
		/// <list>
		/// <item>MERGE ... USING ... ON ... WHEN MATCHED THEN UPDATE ... WHEN NOT MATCHED BY TARGET THEN INSERT ... WHEN NOT MATCHED BY SOURCE THEN DELETE ... OUTPUT ...;</item>
		/// <item>MERGE ... USING ... ON ... WHEN NOT MATCHED BY TARGET THEN INSERT ... WHEN NOT MATCHED BY SOURCE THEN DELETE ... OUTPUT ...;</item>
		/// <item>MERGE ... USING ... ON ... WHEN MATCHED THEN UPDATE ... WHEN NOT MATCHED BY SOURCE THEN DELETE ... OUTPUT ...;</item>
		/// <item>MERGE ... USING ... ON ... WHEN MATCHED THEN DELETE ... OUTPUT ...;</item>
		/// <item>INSERT INTO ... (...) OUTPUT ... VALUES ...;</item>
		/// </list>
		/// </code>
		/// </summary>
		/// <returns>OUTPUT DELETED, INSERTED</returns>
		public static async ValueTask<RowSet> MergeAsync(this DbConnection @this, BatchRequest batch, CancellationToken cancellationToken = default)
		{
			await using var command = @this.CreateSqlCommand(batch.ToSql());

			if (batch.Output.Any())
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
		/// SELECT ... FROM ... WHERE ... HAVING ... ORDER BY ...
		/// </summary>
		public static async ValueTask<RowSet> SelectAsync(this DbConnection @this, SelectRequest select, CancellationToken cancellationToken = default)
		{
			await using var command = @this.CreateSqlCommand(select.ToSql());
			select.Parameters.Do(parameter => command.AddInputParameter(parameter.Name, parameter.Value));

			await using var transaction = await command.BeginTransactionAsync(IsolationLevel.ReadUncommitted, cancellationToken);
			await using var reader = await command.ReadSingleResultAsync(cancellationToken);
			return await reader.ReadRowSetAsync(cancellationToken);
		}

		/// <summary>
		/// TRUNCATE TABLE ...
		/// </summary>
		public static async ValueTask<int> TruncateTableAsync(this DbConnection @this, string table, CancellationToken cancellationToken = default)
		{
			await using var command = @this.CreateSqlCommand($"TRUNCATE TABLE {table.EscapeIdentifier()};");
			return await command.ExecuteNonQueryAsync(cancellationToken);
		}

		/// <summary>
		/// UPDATE ... SET ... WHERE ...
		/// </summary>
		/// <returns>OUTPUT DELETED, INSERTED</returns>
		public static async ValueTask<RowSet> UpdateAsync(this DbConnection @this, UpdateRequest update, CancellationToken cancellationToken = default)
		{
			await using var command = @this.CreateSqlCommand(update.ToSql());
			update.Parameters.Do(parameter => command.AddInputParameter(parameter.Name, parameter.Value));

			if (update.Output.Any())
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
