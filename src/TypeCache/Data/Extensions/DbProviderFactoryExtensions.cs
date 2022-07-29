// Copyright (c) 2021 Samuel Abraham

using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using static TypeCache.Default;

namespace TypeCache.Data.Extensions;

public static class DbProviderFactoryExtensions
{
	public static DbConnection CreateConnection(this DbProviderFactory @this, string connectionString)
	{
		var dbConnection = @this.CreateConnection()!;
		dbConnection.ConnectionString = connectionString;
		return dbConnection;
	}

	public static DbConnectionStringBuilder CreateConnectionStringBuilder(this DbProviderFactory @this, string connectionString)
	{
		var dbConnectionStringBuilder = @this.CreateConnectionStringBuilder()!;
		dbConnectionStringBuilder.ConnectionString = connectionString;
		return dbConnectionStringBuilder;
	}

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static void Register(this DbProviderFactory @this, string databaseProvider)
		=> DbProviderFactories.RegisterFactory(databaseProvider, @this);
}
