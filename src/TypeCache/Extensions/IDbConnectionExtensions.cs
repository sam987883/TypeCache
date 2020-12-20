// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Data;
using TypeCache.Data;
using TypeCache.Data.Schema;

namespace TypeCache.Extensions
{
	public static class IDbConnectionExtensions
	{
		/// <summary>
		/// <code>command.CommandType = CommandType.StoredProcedure;</code>
		/// </summary>
		public static IDbCommand CreateProcedureCommand(this IDbConnection @this, string procedure)
		{
			var command = @this.CreateCommand();
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = procedure;
			return command;
		}

		/// <summary>
		/// <code>command.CommandType = CommandType.Text;</code>
		/// </summary>
		public static IDbCommand CreateSqlCommand(this IDbConnection @this, string sql)
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
			procedure.Parameters.Do(parameter => command.AddInputParameter(parameter.Name, parameter.Value));

			using var reader = command.ExecuteReader(CommandBehavior.SingleResult);
			return reader.ReadRowSets().ToList();
		}

		/// <summary>
		/// EXECUTE ...
		/// </summary>
		public static IEnumerable<RowSet> Run(this IDbConnection @this, SqlRequest request)
		{
			using var command = @this.CreateSqlCommand(request.SQL);
			request.Parameters.Do(parameter => command.AddInputParameter(parameter.Name, parameter.Value));

			using var reader = command.ExecuteReader(CommandBehavior.SingleResult);
			return reader.ReadRowSets().ToList();
		}

		/// <summary>
		/// DELETE FROM ... WHERE ...
		/// </summary>
		/// <param name="schemaProvider">ISchemaFactory or ISchemaStore</param>
		/// <returns>OUTPUT DELETED</returns>
		public static RowSet Delete(this IDbConnection @this, DeleteRequest delete, ISchemaProvider schemaProvider)
		{
			var schema = schemaProvider.GetObjectSchema(@this, delete.From);
			delete.From = schema.Name;

			var validator = new SchemaValidator(schema);
			validator.Validate(delete);

			using var command = @this.CreateSqlCommand(delete.ToSql());
			delete.Parameters.Do(parameter => command.AddInputParameter(parameter.Name, parameter.Value));

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
		/// SELECT ... FROM ... WHERE ... HAVING ... ORDER BY ...
		/// </summary>
		/// <param name="schemaProvider">ISchemaFactory or ISchemaStore</param>
		public static RowSet Insert(this IDbConnection @this, InsertRequest insert, ISchemaProvider schemaProvider)
		{
			var schema = schemaProvider.GetObjectSchema(@this, insert.From);
			insert.From = schema.Name;

			schema = schemaProvider.GetObjectSchema(@this, insert.Into);
			insert.Into = schema.Name;

			var validator = new SchemaValidator(schema);
			validator.Validate(insert);

			using var command = @this.CreateSqlCommand(insert.ToSql());
			insert.Parameters.Do(parameter => command.AddInputParameter(parameter.Name, parameter.Value));

			if (insert.Output.Any())
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
		/// <param name="schemaProvider">ISchemaFactory or ISchemaStore</param>
		/// <returns>OUTPUT DELETED, INSERTED</returns>
		public static RowSet Merge(this IDbConnection @this, BatchRequest batch, ISchemaProvider schemaProvider)
		{
			var schema = schemaProvider.GetObjectSchema(@this, batch.Table);
			batch.Table = schema.Name;
			batch.On = schema.Columns.If(column => column.PrimaryKey).To(column => column.Name).ToList().ToArray();

			var validator = new SchemaValidator(schema);
			validator.Validate(batch);

			using var command = @this.CreateSqlCommand(batch.ToSql());
			if (batch.Output.Any())
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
		/// SELECT ... FROM ... WHERE ... HAVING ... ORDER BY ...
		/// </summary>
		/// <param name="schemaProvider">ISchemaFactory or ISchemaStore</param>
		public static RowSet Select(this IDbConnection @this, SelectRequest select, ISchemaProvider schemaProvider)
		{
			var schema = schemaProvider.GetObjectSchema(@this, select.From);
			select.From = schema.Name;

			var validator = new SchemaValidator(schema);
			validator.Validate(select);

			using var command = @this.CreateSqlCommand(select.ToSql());
			select.Parameters.Do(parameter => command.AddInputParameter(parameter.Name, parameter.Value));

			using var transaction = command.BeginTransaction(IsolationLevel.ReadUncommitted);
			using var reader = command.ExecuteReader(CommandBehavior.SingleResult);
			return reader.ReadRowSet();
		}

		/// <summary>
		/// TRUNCATE TABLE ...
		/// </summary>
		public static int TruncateTable(this IDbConnection @this, string table)
		{
			using var command = @this.CreateSqlCommand($"TRUNCATE TABLE {table.EscapeIdentifier()};");
			return command.ExecuteNonQuery();
		}

		/// <summary>
		/// UPDATE ... SET ... WHERE ...
		/// </summary>
		/// <param name="schemaProvider">ISchemaFactory or ISchemaStore</param>
		/// <returns>OUTPUT DELETED, INSERTED</returns>
		public static RowSet Update(this IDbConnection @this, UpdateRequest update, ISchemaProvider schemaProvider)
		{
			var schema = schemaProvider.GetObjectSchema(@this, update.Table);
			update.Table = schema.Name;

			var validator = new SchemaValidator(schema);
			validator.Validate(update);

			using var command = @this.CreateSqlCommand(update.ToSql());
			update.Parameters.Do(parameter => command.AddInputParameter(parameter.Name, parameter.Value));

			if (update.Output.Any())
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
	}
}