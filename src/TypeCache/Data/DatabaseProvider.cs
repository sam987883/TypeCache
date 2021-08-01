// Copyright(c) 2020 Samuel Abraham

using System.Data.Common;
using System.Runtime.CompilerServices;
using TypeCache.Data.Extensions;

namespace TypeCache.Data
{
	internal readonly struct DatabaseProvider
	{
		private readonly string _ConnectionString;

		public DatabaseProvider(DataSource dataSource)
		{
			this._ConnectionString = dataSource.ConnectionString;
			this.Factory = DbProviderFactories.GetFactory(dataSource.DatabaseProvider);
		}

		public DbProviderFactory Factory { get; init; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DbConnection CreateConnection()
			=> this.Factory.CreateConnection(this._ConnectionString);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DbConnectionStringBuilder CreateConnectionStringBuilder()
			=> this.Factory.CreateConnectionStringBuilder(this._ConnectionString);
	}
}
