// Copyright (c) 2021 Samuel Abraham

using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace TypeCache.Extensions
{
	public static class DbCommandExtensions
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

		public static async ValueTask<DbTransaction> BeginTransactionAsync(this DbCommand @this, IsolationLevel isolationLevel, CancellationToken cancellationToken = default)
		{
			var transaction = isolationLevel == IsolationLevel.Unspecified
				? await @this.Connection!.BeginTransactionAsync(cancellationToken)
				: await @this.Connection!.BeginTransactionAsync(isolationLevel, cancellationToken);
			@this.Transaction = transaction;
			return transaction;
		}
	}
}
