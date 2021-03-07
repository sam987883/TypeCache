// Copyright (c) 2021 Samuel Abraham

using System.Data;
using System.Data.Common;

namespace TypeCache.Data.Extensions
{
	public static class DbConnectionExtensions
	{
		/// <summary>
		/// <code>CommandType = CommandType.StoredProcedure</code>
		/// </summary>
		public static DbCommand CreateProcedureCommand(this DbConnection @this, string procedure)
		{
			var command = @this.CreateCommand();
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = procedure;
			return command;
		}

		/// <summary>
		/// <code>CommandType = CommandType.Text</code>
		/// </summary>
		public static DbCommand CreateSqlCommand(this DbConnection @this, string sql)
		{
			var command = @this.CreateCommand();
			command.CommandType = CommandType.Text;
			command.CommandText = sql;
			return command;
		}
	}
}
