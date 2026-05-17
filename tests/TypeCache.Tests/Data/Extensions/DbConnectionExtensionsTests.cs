// Copyright (c) 2021 Samuel Abraham

using System.Data;
using System.Data.Common;
using NSubstitute;
using TypeCache.Data;
using TypeCache.Data.Extensions;
using Xunit;

namespace TypeCache.Tests.Data.Extensions;

public class DbConnectionExtensionsTests
{
	[Fact]
	public void CreateCommand_WithSqlCommand()
	{
		var dataSource = Substitute.For<IDataSource>();
		var connection = Substitute.For<DbConnection>();
		var dbCommand = Substitute.For<DbCommand>();
		connection.CreateCommand().Returns(dbCommand);

		var sqlCommand = new SqlCommand(dataSource, "SELECT * FROM Users")
		{ 
			Type = CommandType.Text,
		};

		var result = connection.CreateCommand(sqlCommand);

		Assert.NotNull(result);
		Assert.Equal(dbCommand, result);
	}

	[Fact]
	public void CreateCommand_WithTimeout()
	{
		var dataSource = Substitute.For<IDataSource>();
		var connection = Substitute.For<DbConnection>();
		var dbCommand = Substitute.For<DbCommand>();
		connection.CreateCommand().Returns(dbCommand);

		var sqlCommand = new SqlCommand(dataSource, "SELECT * FROM Users")
		{
			Type = System.Data.CommandType.Text,
			Timeout = TimeSpan.FromSeconds(30)
		};

		var result = connection.CreateCommand(sqlCommand);

		Assert.NotNull(result);
		Assert.Equal(30, dbCommand.CommandTimeout);
	}
}
