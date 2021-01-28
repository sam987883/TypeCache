// Copyright (c) 2021 Samuel Abraham

using System.Data.Common;
using System.Runtime.CompilerServices;

namespace TypeCache.Extensions
{
	public static class DbProviderFactoryExtensions
	{
		public static DbConnection CreateConnection(this DbProviderFactory @this, string connectionString)
		{
			var dbConnection = @this.CreateConnection();
			dbConnection!.ConnectionString = connectionString;
			return dbConnection;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Register(this DbProviderFactory @this, string databaseProvider)
			=> DbProviderFactories.RegisterFactory(databaseProvider, @this);
	}
}
