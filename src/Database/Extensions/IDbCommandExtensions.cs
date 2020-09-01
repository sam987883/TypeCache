// Copyright (c) 2020 Samuel Abraham

using System.Data;

namespace Sam987883.Database.Extensions
{
	public static class IDbCommandExtensions
	{
		public static int AddInputParameter(this IDbCommand @this, string name, object? value, DbType? dbType = null)
		{
			var parameter = @this.CreateParameter();
			parameter.Direction = ParameterDirection.Input;
			parameter.ParameterName = name;
			parameter.Value = value;
			if (dbType.HasValue)
				parameter.DbType = dbType.Value;
			return @this.Parameters.Add(parameter);
		}

		public static int AddOutputParameter(this IDbCommand @this, string name, object? value, DbType dbType)
		{
			var parameter = @this.CreateParameter();
			parameter.Direction = value != null ? ParameterDirection.Output : ParameterDirection.InputOutput;
			parameter.ParameterName = name;
			parameter.Value = value;
			parameter.DbType = dbType;
			return @this.Parameters.Add(parameter);
		}

		public static IDbTransaction BeginTransaction(this IDbCommand @this, IsolationLevel isolationLevel)
		{
			using var transaction = isolationLevel == IsolationLevel.Unspecified ? @this.Connection.BeginTransaction() : @this.Connection.BeginTransaction(isolationLevel);
			@this.Transaction = transaction;
			return transaction;
		}
	}
}