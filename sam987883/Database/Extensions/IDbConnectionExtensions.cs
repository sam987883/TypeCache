// Copyright (c) 2020 Samuel Abraham

using sam987883.Common.Extensions;
using sam987883.Common.Models;
using sam987883.Database.Models;
using System.Collections.Generic;
using System.Data;

namespace sam987883.Database.Extensions
{
	public static class IDbConnectionExtensions
	{
		internal static IDbCommand CreateProcedureCommand(this IDbConnection @this, string procedure)
		{
			var command = @this.CreateCommand();
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = procedure;
			return command;
		}

		internal static IDbCommand CreateSqlCommand(this IDbConnection @this, string sql)
		{
			var command = @this.CreateCommand();
			command.CommandType = CommandType.Text;
			command.CommandText = sql;
			return command;
		}

		/// <summary>
		/// EXECUTE ...
		/// </summary>
		public static IEnumerable<RowSet> Call(this IDbConnection @this, StoredProcedureRequest procedure)
		{
			using var command = @this.CreateProcedureCommand(procedure.Procedure);
			procedure.Parameters.Do(_ => command.AddInputParameter(_.Key, _.Value));

			using var reader = command.ExecuteReader(CommandBehavior.SingleResult);
			return reader.ReadRowSets().ToList();
		}

		/// <summary>
		/// DELETE FROM ... WHERE ...
		/// </summary>
		/// <returns>OUTPUT DELETED</returns>
		public static RowSet Delete(this IDbConnection @this, DeleteRequest delete)
		{
			using var command = @this.CreateSqlCommand(delete.ToSql());
			if (delete.Output.Any())
			{
				using var reader = command.ExecuteReader();
				return reader.ReadRowSet();
			}
			else
			{
				command.ExecuteNonQuery();
				return default;
			}
		}

		/// <summary>
		/// MERGE ... USING ... ON ...
		/// WHEN MATCHED THEN UPDATE ...
		/// WHEN NOT MATCHED BY TARGET THEN INSERT ...
		/// WHEN NOT MATCHED BY SOURCE THEN DELETE ...
		/// </summary>
		/// <returns>OUTPUT DELETED, INSERTED</returns>
		public static Output Merge(this IDbConnection @this, BatchRequest batch)
		{
			using var command = @this.CreateSqlCommand(batch.ToSql());
			if (batch.OutputDeleted.Any() || batch.OutputInserted.Any())
			{
				using var reader = command.ExecuteReader();
				return reader.ReadRowSets(batch.OutputDeleted, batch.OutputInserted);
			}
			else
			{
				command.ExecuteNonQuery();
				return default;
			}
		}

		/// <summary>
		/// SELECT ... FROM ... WHERE ... HAVING ... ORDER BY ...
		/// </summary>
		public static RowSet Select(this IDbConnection @this, SelectRequest select)
		{
			using var command = @this.CreateSqlCommand(select.ToSql());
			select.Parameters.Do(parameter => command.AddInputParameter(parameter.Name, parameter.Value));

			using var transaction = @this.BeginTransaction(IsolationLevel.ReadUncommitted);
			command.Transaction = transaction;
			using var reader = command.ExecuteReader(CommandBehavior.SingleResult);
			return reader.ReadRowSet();
		}

		/// <summary>
		/// TRUNCATE TABLE ...
		/// </summary>
		public static int TruncateTable(this IDbConnection @this, string table)
		{
			using var command = @this.CreateSqlCommand($"TRUNCATE TABLE [{table.EscapeIdentifier()}];");
			return command.ExecuteNonQuery();
		}

		/// <summary>
		/// UPDATE ... SET ... WHERE ...
		/// </summary>
		/// <returns>OUTPUT DELETED, INSERTED</returns>
		public static Output Update(this IDbConnection @this, UpdateRequest update)
		{
			using var command = @this.CreateSqlCommand(update.ToSql());
			if (update.OutputDeleted.Any() || update.OutputInserted.Any())
			{
				using var reader = command.ExecuteReader();
				return reader.ReadRowSets(update.OutputDeleted, update.OutputInserted);
			}
			else
			{
				command.ExecuteNonQuery();
				return default;
			}
		}
	}
}