// Copyright (c) 2021 Samuel Abraham

using System.Data.Common;
using NSubstitute;
using TypeCache.Data.Extensions;
using Xunit;

namespace TypeCache.Tests.Data.Extensions;

public class DbProviderFactoryExtensionsTests
{
	[Fact]
	public void CreateConnection_WithConnectionString()
	{
		var factory = Substitute.For<DbProviderFactory>();
		var connection = Substitute.For<DbConnection>();
		factory.CreateConnection().Returns(connection);

		var connectionString = "Server=localhost;Database=test";
		var result = factory.CreateConnection(connectionString);

		Assert.NotNull(result);
		Assert.Equal(connectionString, connection.ConnectionString);
	}

	[Fact]
	public void CreateConnectionStringBuilder()
	{
		var factory = Substitute.For<DbProviderFactory>();
		var builder = new DbConnectionStringBuilder();
		factory.CreateConnectionStringBuilder().Returns(builder);

		var connectionString = "Server=localhost;Database=test";
		var result = factory.CreateConnectionStringBuilder(connectionString);

		Assert.NotNull(result);
		Assert.Equal(connectionString.ToLowerInvariant(), builder.ConnectionString);
	}
}
