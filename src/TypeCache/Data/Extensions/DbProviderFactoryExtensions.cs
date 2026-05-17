// Copyright (c) 2021 Samuel Abraham

using System.Data.Common;

namespace TypeCache.Data.Extensions;

public static class DbProviderFactoryExtensions
{
	extension<T>(T @this) where T : DbProviderFactory
	{
		public DbConnection CreateConnection(string connectionString)
		{
			var dbConnection = @this.CreateConnection()!;
			dbConnection.ConnectionString = connectionString;
			return dbConnection;
		}

		public DbConnectionStringBuilder CreateConnectionStringBuilder(string connectionString)
		{
			var dbConnectionStringBuilder = @this.CreateConnectionStringBuilder()!;
			dbConnectionStringBuilder.ConnectionString = connectionString;
			return dbConnectionStringBuilder;
		}

		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public void Register(string databaseProvider)
			=> DbProviderFactories.RegisterFactory(databaseProvider, @this);
	}
}
