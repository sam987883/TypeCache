// Copyright (c) 2021 Samuel Abraham

using System.Data;
using System.Data.Common;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using TypeCache.Data;
using TypeCache.Data.Extensions;
using Xunit;

namespace TypeCache.Tests.Data.Extensions;

public class DataServiceCollectionExtensionsTests
{
	[Fact]
	public void AddDataSource()
	{
		var connectionString = "Server=localhost;Database=test";
		var services = new ServiceCollection();
		var factory = Substitute.For<DbProviderFactory>();
		var connection = Substitute.For<DbConnection>();
		factory.CreateConnection().Returns(connection);
		factory.CreateDataAdapter().Returns(Substitute.For<DbDataAdapter>());
		connection.CreateCommand().Returns(Substitute.For<DbCommand>());
		connection.GetSchema(Arg.Any<string>()).Returns(new DataTable());

		services.AddDataSource("TestDb", factory, connectionString, new[] { "db1", "db2" });

		var provider = services.BuildServiceProvider();
		var dataSource = provider.GetKeyedService<IDataSource>("TestDb");

		Assert.NotNull(dataSource);
	}

	[Fact]
	public void AddSqlCommandRules()
	{
		var services = new ServiceCollection();
		services.AddSqlCommandRules();

		var provider = services.BuildServiceProvider();

		Assert.NotNull(provider);
	}

	[Fact]
	public void AddSqlDataSetRule()
	{
		var services = new ServiceCollection();
		services.AddSqlDataSetRule();

		var provider = services.BuildServiceProvider();

		Assert.NotNull(provider);
	}

	[Fact]
	public void AddSqlDataTableRule()
	{
		var services = new ServiceCollection();
		services.AddSqlDataTableRule();

		var provider = services.BuildServiceProvider();

		Assert.NotNull(provider);
	}
}
