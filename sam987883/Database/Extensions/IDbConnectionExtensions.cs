// Copyright (c) 2020 Samuel Abraham

using sam987883.Extensions;
using System;
using System.Data;

namespace sam987883.Database.Extensions
{
	public static class IDbConnectionExtensions
	{
		private static SqlDbType GetSqlDbType(TypeCode typeCode)
		{
			switch (typeCode)
			{
				case TypeCode.Object: return SqlDbType.Variant;
				case TypeCode.Boolean: return SqlDbType.Bit;
				case TypeCode.Char: return SqlDbType.NChar;
				case TypeCode.SByte: return SqlDbType.TinyInt;
				case TypeCode.Byte: return SqlDbType.BigInt;
				case TypeCode.Int16: return SqlDbType.SmallInt;
				case TypeCode.UInt16: return SqlDbType.BigInt;
				case TypeCode.Int32: return SqlDbType.Int;
				case TypeCode.UInt32: return SqlDbType.BigInt;
				case TypeCode.Int64: return SqlDbType.BigInt;
				case TypeCode.UInt64: return SqlDbType.BigInt;
				case TypeCode.Single: return SqlDbType.Money;
				case TypeCode.Double: return SqlDbType.Float;
				case TypeCode.Decimal: return SqlDbType.Decimal;
				case TypeCode.DateTime: return SqlDbType.DateTime2;
				case TypeCode.String: return SqlDbType.NVarChar;
				default: throw new ArgumentOutOfRangeException(nameof(typeCode), typeCode, null);
			}
		}

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

		public static int Delete(this IDbConnection @this, BatchDelete batch)
		{
			using var command = @this.CreateSqlCommand(batch.ToSql());
			if (batch.Output.Columns.Any())
			{
				using var reader = command.ExecuteReader();
				batch.Output.Rows = reader.ReadValues().ToArray();
				return batch.Output.Rows.Length;
			}
			else
				return command.ExecuteNonQuery();
		}

		public static int Delete(this IDbConnection @this, Delete delete)
		{
			using var command = @this.CreateSqlCommand(delete.ToSql());
			if (delete.Output.Columns.Any())
			{
				using var reader = command.ExecuteReader();
				delete.Output.Rows = reader.ReadValues().ToArray();
				return delete.Output.Rows.Length;
			}
			else
				return command.ExecuteNonQuery();
		}

		public static int Delete<T>(this IDbConnection @this, BatchDelete<T> batch) where T : class, new()
		{
			using var command = @this.CreateSqlCommand(batch.ToSql());
			if (batch.Output.Columns.Any())
			{
				using var reader = command.ExecuteReader();
				batch.Output.Rows = reader.Read(batch.PropertyCache).ToArray();
				return batch.Output.Rows.Length;
			}
			else
				return command.ExecuteNonQuery();
		}

		public static int Delete<T>(this IDbConnection @this, Delete<T> delete) where T : class, new()
		{
			using var command = @this.CreateSqlCommand(delete.ToSql());
			if (delete.Output.Columns.Any())
			{
				using var reader = command.ExecuteReader();
				delete.Output.Rows = reader.Read(delete.PropertyCache).ToArray();
				return delete.Output.Rows.Length;
			}
			else
				return command.ExecuteNonQuery();
		}

		public static int Insert(this IDbConnection @this, BatchInsert batch)
		{
			using var command = @this.CreateSqlCommand(batch.ToSql());
			if (batch.Output.Columns.Any())
			{
				using var reader = command.ExecuteReader();
				batch.Output.Rows = reader.ReadValues().ToArray();
				return batch.Output.Rows.Length;
			}
			else
				return command.ExecuteNonQuery();
		}

		public static int Insert<T>(this IDbConnection @this, BatchInsert<T> batch) where T : class, new()
		{
			using var command = @this.CreateSqlCommand(batch.ToSql());
			if (batch.Output.Columns.Any())
			{
				using var reader = command.ExecuteReader();
				batch.Output.Rows = reader.Read(batch.PropertyCache).ToArray();
				return batch.Output.Rows.Length;
			}
			else
				return command.ExecuteNonQuery();
		}

		public static int Select(this IDbConnection @this, Select select)
		{
			using var command = @this.CreateSqlCommand(select.ToSql());
			select.Parameters.Do(parameter => command.AddInputParameter(parameter.Name, parameter.Value));

			using var reader = command.ExecuteReader(CommandBehavior.SingleResult);
			select.Rows = reader.ReadValues().ToArray();
			return select.Rows.Length;
		}

		public static int Select<T>(this IDbConnection @this, Select<T> select) where T : class, new()
		{
			using var command = @this.CreateSqlCommand(select.ToSql());
			select.Parameters.Do(parameter => command.AddInputParameter(parameter.Name, parameter.Value));

			using var reader = command.ExecuteReader(CommandBehavior.SingleResult);
			select.Rows = reader.Read(select.PropertyCache).ToArray();
			return select.Rows.Length;
		}

		public static int TruncateTable(this IDbConnection @this, string table)
		{
			using var command = @this.CreateSqlCommand($"TRUNCATE TABLE [{table.EscapeIdentifier()}];");
			return command.ExecuteNonQuery();
		}

		public static int Update(this IDbConnection @this, Update update)
		{
			using var command = @this.CreateSqlCommand(update.ToSql());
			if (update.Output.Deleted.Columns.Any() || update.Output.Inserted.Columns.Any())
			{
				using var reader = command.ExecuteReader();
				var outputRows = reader.ReadValues().ToArray();
				if (update.Output.Deleted.Columns.Any())
					update.Output.Deleted.Rows = outputRows.To(row => row.Subarray(0, update.Output.Deleted.Columns.Length)).ToArray(update.Output.Deleted.Columns.Length);
				if (update.Output.Inserted.Columns.Any())
					update.Output.Inserted.Rows = outputRows.To(row => row.Subarray(update.Output.Deleted.Columns.Length, update.Output.Inserted.Columns.Length)).ToArray(update.Output.Inserted.Columns.Length);
				return outputRows.Length;
			}
			else
				return command.ExecuteNonQuery();
		}

		public static int Update(this IDbConnection @this, BatchUpdate batch)
		{
			using var command = @this.CreateSqlCommand(batch.ToSql());
			if (batch.Output.Deleted.Columns.Any() || batch.Output.Inserted.Columns.Any())
			{
				using var reader = command.ExecuteReader();
				var outputRows = reader.ReadValues().ToArray();
				if (batch.Output.Deleted.Columns.Any())
					batch.Output.Deleted.Rows = outputRows.To(row => row.Subarray(0, batch.Output.Deleted.Columns.Length)).ToArray(batch.Output.Deleted.Columns.Length);
				if (batch.Output.Inserted.Columns.Any())
					batch.Output.Inserted.Rows = outputRows.To(row => row.Subarray(batch.Output.Deleted.Columns.Length, batch.Output.Inserted.Columns.Length)).ToArray(batch.Output.Inserted.Columns.Length);
				return outputRows.Length;
			}
			else
				return command.ExecuteNonQuery();
		}

		public static int Update<T>(this IDbConnection @this, Update<T> update) where T : class, new()
		{
			using var command = @this.CreateSqlCommand(update.ToSql());
			if (update.Output.Deleted.Columns.Any() || update.Output.Inserted.Columns.Any())
			{
				using var reader = command.ExecuteReader();
				var outputRows = reader.Read(update.PropertyCache, update.Output.Deleted.Columns, update.Output.Inserted.Columns).ToArray();
				if (update.Output.Deleted.Columns.Any())
					update.Output.Deleted.Rows = outputRows.To(outputRow => outputRow.Deleted).ToArray(outputRows.Length);
				if (update.Output.Inserted.Columns.Any())
					update.Output.Inserted.Rows = outputRows.To(outputRow => outputRow.Inserted).ToArray(outputRows.Length);
				return outputRows.Length;
			}
			else
				return command.ExecuteNonQuery();
		}

		public static int Update<T>(this IDbConnection @this, BatchUpdate<T> batch) where T : class, new()
		{
			using var command = @this.CreateSqlCommand(batch.ToSql());
			if (batch.Output.Deleted.Columns.Any() || batch.Output.Inserted.Columns.Any())
			{
				using var reader = command.ExecuteReader();
				var outputRows = reader.Read(batch.PropertyCache, batch.Output.Deleted.Columns, batch.Output.Inserted.Columns).ToArray();
				if (batch.Output.Deleted.Columns.Any())
					batch.Output.Deleted.Rows = outputRows.To(outputRow => outputRow.Deleted).ToArray(outputRows.Length);
				if (batch.Output.Inserted.Columns.Any())
					batch.Output.Inserted.Rows = outputRows.To(outputRow => outputRow.Inserted).ToArray(outputRows.Length);
				return outputRows.Length;
			}
			else
				return command.ExecuteNonQuery();
		}

		public static int Upsert(this IDbConnection @this, BatchUpsert batch)
		{
			using var command = @this.CreateSqlCommand(batch.ToSql());
			if (batch.Output.Deleted.Columns.Any() || batch.Output.Inserted.Columns.Any())
			{
				using var reader = command.ExecuteReader();
				var outputRows = reader.ReadValues().ToArray();
				if (batch.Output.Deleted.Columns.Any())
					batch.Output.Deleted.Rows = outputRows.To(row => row.Subarray(0, batch.Output.Deleted.Columns.Length)).ToArray(batch.Output.Deleted.Columns.Length);
				if (batch.Output.Inserted.Columns.Any())
					batch.Output.Inserted.Rows = outputRows.To(row => row.Subarray(batch.Output.Deleted.Columns.Length, batch.Output.Inserted.Columns.Length)).ToArray(batch.Output.Inserted.Columns.Length);
				return outputRows.Length;
			}
			else
				return command.ExecuteNonQuery();
		}

		public static int Upsert<T>(this IDbConnection @this, BatchUpsert<T> batch) where T : class, new()
		{
			using var command = @this.CreateSqlCommand(batch.ToSql());
			if (batch.Output.Deleted.Columns.Any() || batch.Output.Inserted.Columns.Any())
			{
				using var reader = command.ExecuteReader();
				var outputRows = reader.Read(batch.PropertyCache, batch.Output.Deleted.Columns, batch.Output.Inserted.Columns).ToArray();
				if (batch.Output.Deleted.Columns.Any())
					batch.Output.Deleted.Rows = outputRows.To(outputRow => outputRow.Deleted).ToArray(outputRows.Length);
				if (batch.Output.Inserted.Columns.Any())
					batch.Output.Inserted.Rows = outputRows.To(outputRow => outputRow.Inserted).ToArray(outputRows.Length);
				return outputRows.Length;
			}
			else
				return command.ExecuteNonQuery();
		}
	}
}