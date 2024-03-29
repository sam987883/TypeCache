﻿// Copyright (c) 2021 Samuel Abraham

using NSubstitute;
using TypeCache.Data;
using Xunit;
using static System.FormattableString;
using static TypeCache.Data.DataSourceType;

namespace TypeCache.Tests.Data.Extensions;

public class SqlExtensions
{
	public class Person
	{
		public int ID { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public int Age { get; set; }
	}

	[Fact]
	public void CreateCountSQL()
	{
		var dataSource = CreateDataSourceMock(SqlServer);
		var objectSchema = new ObjectSchema(dataSource, DatabaseObjectType.Table, "db", "dbo", "Test", null, null);

		var expected = Invariant($@"SELECT COUNT(*)
FROM {objectSchema} WITH(NOLOCK)
WHERE [First Name] = N'Sarah' AND [Last_Name] = N'Marshal';
");
		var actual = objectSchema.CreateCountSQL(null, "[First Name] = N'Sarah' AND [Last_Name] = N'Marshal'");

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void CreateDeleteBatchSQL()
	{
		var dataSource = CreateDataSourceMock(SqlServer);
		var objectSchema = new ObjectSchema(dataSource, DatabaseObjectType.Table, "db", "dbo", "Test",
		[
			new ColumnSchema("ID", false, true, true, true, typeof(int).TypeHandle),
			new ColumnSchema("First Name", false, false, false, false, typeof(string).TypeHandle),
			new ColumnSchema("Last Name", false, false, false, false, typeof(string).TypeHandle),
		], null);

		var expected = Invariant($@"DELETE {objectSchema}
OUTPUT INSERTED.[First Name] AS [First Name], DELETED.[Last_Name] AS [Last_Name], INSERTED.ID
WHERE [ID] IN (1, 2, 3);
");
		Person[] data = [new() { ID = 1 }, new() { ID = 2 }, new() { ID = 3 }];
		var actual = objectSchema.CreateDeleteSQL<Person>(data, new[] { "INSERTED.[First Name] AS [First Name]", "DELETED.[Last_Name] AS [Last_Name]", "INSERTED.ID" });

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void CreateDeleteSQL()
	{
		var dataSource = CreateDataSourceMock(SqlServer);
		var objectSchema = new ObjectSchema(dataSource, DatabaseObjectType.Table, "db", "dbo", "Test",
		[
			new ColumnSchema("ID", false, true, true, true, typeof(int).TypeHandle),
			new ColumnSchema("First Name", false, false, false, false, typeof(string).TypeHandle),
			new ColumnSchema("Last Name", false, false, false, false, typeof(string).TypeHandle),
		], null);

		var expected = Invariant($@"DELETE {objectSchema}
OUTPUT DELETED.[ID], DELETED.[First Name], DELETED.[Last_Name]
WHERE [First Name] = N'Sarah' AND [Last_Name] = N'Marshal';
");
		var actual = objectSchema.CreateDeleteSQL("[First Name] = N'Sarah' AND [Last_Name] = N'Marshal'", ["DELETED.[ID]", "DELETED.[First Name]", "DELETED.[Last_Name]"]);

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void CreateBatchInsertSQL()
	{
		var dataSource = CreateDataSourceMock(SqlServer);
		var objectSchema = new ObjectSchema(dataSource, DatabaseObjectType.Table, "db", "dbo", "Test",
		[
			new ColumnSchema("ID", false, true, true, true, typeof(int).TypeHandle),
			new ColumnSchema("FirstName", false, false, false, false, typeof(string).TypeHandle),
			new ColumnSchema("LastName", false, false, false, false, typeof(string).TypeHandle),
		], null);
		Person[] data =
		[
			new() { ID = 1, FirstName = "FirstName1", LastName = "LastName1", Age = 30 },
			new() { ID = 2, FirstName = "FirstName2", LastName = "LastName2", Age = 31 },
			new() { ID = 3, FirstName = "FirstName3", LastName = "LastName3", Age = 32 }
		];

		var expected = Invariant($@"INSERT INTO {objectSchema}
([FirstName], [LastName])
OUTPUT INSERTED.ID, INSERTED.[LastName]
VALUES (N'FirstName1', N'LastName1')
	, (N'FirstName2', N'LastName2')
	, (N'FirstName3', N'LastName3');
");
		var actual = objectSchema.CreateInsertSQL<Person>(["FirstName", "LastName"], data, ["INSERTED.ID", "INSERTED.[LastName]"]);

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void CreateInsertSQL()
	{
		var dataSource = CreateDataSourceMock(SqlServer);
		var objectSchema = new ObjectSchema(dataSource, DatabaseObjectType.Table, "db", "dbo", "Test",
		[
			new ColumnSchema("ID", false, true, true, true, typeof(int).TypeHandle),
			new ColumnSchema("First Name", false, false, false, false, typeof(string).TypeHandle),
			new ColumnSchema("Last Name", false, false, false, false, typeof(string).TypeHandle),
		], null);
		var selectQuery = new SelectQuery
		{
			From = new("[dbo].[NonCustomers]"),
			Having = "MAX([Age]) > 40",
			OrderBy = ["[First Name] ASC", "Last_Name DESC"],
			Select = ["ID", "TRIM([First Name]) AS [First Name]", "UPPER([LastName]) AS LastName", "40 Age", "Amount AS Amount"],
			TableHints = "NOLOCK",
			Where = "[First Name] = N'Sarah' AND [Last_Name] = N'Marshal'",
		};

		var expected = Invariant($@"INSERT INTO {objectSchema}
([[First Name]]], [[Last_Name]]], [Age], [Amount])
OUTPUT INSERTED.[First Name] AS [First Name], INSERTED.[ID] AS [ID]
SELECT ID, TRIM([First Name]) AS [First Name], UPPER([LastName]) AS LastName, 40 Age, Amount AS Amount
FROM [dbo].[NonCustomers] WITH(NOLOCK)
WHERE [First Name] = N'Sarah' AND [Last_Name] = N'Marshal'
HAVING MAX([Age]) > 40
ORDER BY [First Name] ASC, Last_Name DESC;
");
		var actual = objectSchema.CreateInsertSQL(["[First Name]", "[Last_Name]", "Age", "Amount"], selectQuery, ["INSERTED.[First Name] AS [First Name]", "INSERTED.[ID] AS [ID]"]);

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void CreateSelectSQL()
	{
		var dataSource = CreateDataSourceMock(SqlServer);
		var objectSchema = new ObjectSchema(dataSource, DatabaseObjectType.Table, "db", "dbo", "Test",
		[
			new ColumnSchema("ID", false, true, true, true, typeof(int).TypeHandle),
			new ColumnSchema("FirstName", false, false, false, false, typeof(string).TypeHandle),
			new ColumnSchema("LastName", false, false, false, false, typeof(string).TypeHandle),
		], null);
		var selectQuery = new SelectQuery
		{
			Select = ["ID", "TRIM([FirstName]) AS [FirstName]", "UPPER([LastName]) AS LastName", "40 Age", "Amount AS Amount"],
			From = new("[dbo].[NonCustomers]"),
			Having = "MAX([Age]) > 40",
			Where = "[FirstName] = N'Sarah' AND [LastName] = N'Marshal'",
			OrderBy = ["[FirstName] ASC", "LastName DESC"],
			Fetch = 100,
			Offset = 0,
			TableHints = "NOLOCK"
		};

		var expected = Invariant($@"SELECT ID, TRIM([FirstName]) AS [FirstName], UPPER([LastName]) AS LastName, 40 Age, Amount AS Amount
FROM [dbo].[NonCustomers] WITH(NOLOCK)
WHERE [FirstName] = N'Sarah' AND [LastName] = N'Marshal'
HAVING MAX([Age]) > 40
ORDER BY [FirstName] ASC, LastName DESC
FETCH NEXT 100 ROWS ONLY;
");
		var actual = objectSchema.CreateSelectSQL(selectQuery);

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void CreateBatchUpdateSQL()
	{
		var dataSource = CreateDataSourceMock(SqlServer);
		var objectSchema = new ObjectSchema(dataSource, DatabaseObjectType.Table, "db", "dbo", "Test",
		[
			new ColumnSchema("ID", false, true, true, true, typeof(int).TypeHandle),
			new ColumnSchema("FirstName", false, false, false, false, typeof(string).TypeHandle),
			new ColumnSchema("LastName", false, false, false, false, typeof(string).TypeHandle),
		], null);
		Person[] data =
		[
			new() { ID = 1, FirstName = "FirstName1", LastName = "LastName1", Age = 30 },
			new() { ID = 2, FirstName = "FirstName2", LastName = "LastName2", Age = 31 },
			new() { ID = 3, FirstName = "FirstName3", LastName = "LastName3", Age = 32 }
		];

		var expected = Invariant($@"UPDATE {objectSchema} WITH(UPDLOCK)
SET [FirstName] = data.[FirstName], [LastName] = data.[LastName]
OUTPUT INSERTED.[FirstName] AS [FirstName], DELETED.LastName AS LastName, INSERTED.[ID] AS [ID]
FROM {objectSchema} AS _
INNER JOIN
(
VALUES (N'FirstName1', N'LastName1')
	, (N'FirstName2', N'LastName2')
	, (N'FirstName3', N'LastName3')
) AS data ([FirstName], [LastName])
ON data.[ID] = _.[ID];
");
		var actual = objectSchema.CreateUpdateSQL<Person>(["FirstName", "LastName"], data
			, ["INSERTED.[FirstName] AS [FirstName]", "DELETED.LastName AS LastName", "INSERTED.[ID] AS [ID]"]);

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void CreateUpdateSQL()
	{
		var dataSource = CreateDataSourceMock(SqlServer);
		var objectSchema = new ObjectSchema(dataSource, DatabaseObjectType.Table, "db", "dbo", "Test",
		[
			new ColumnSchema("ID", false, true, true, true, typeof(int).TypeHandle),
			new ColumnSchema("First Name", false, false, false, false, typeof(string).TypeHandle),
			new ColumnSchema("Last Name", false, false, false, false, typeof(string).TypeHandle),
		], null);

		var expected = Invariant($@"UPDATE {objectSchema} WITH(UPDLOCK)
SET [First Name] = N'Sarah', Last_Name = N'Marshal', Age = @Param1
OUTPUT INSERTED.[First Name] AS [First Name], DELETED.[Last_Name] AS [Last_Name], INSERTED.ID AS [ID]
WHERE [First Name] = N'Sarah' AND [Last_Name] = N'Marshal';
");
		var actual = objectSchema.CreateUpdateSQL(["[First Name] = N'Sarah'", "Last_Name = N'Marshal'", "Age = @Param1"]
			, "[First Name] = N'Sarah' AND [Last_Name] = N'Marshal'"
			, ["INSERTED.[First Name] AS [First Name]", "DELETED.[Last_Name] AS [Last_Name]", "INSERTED.ID AS [ID]"]);

		Assert.Equal(expected, actual);
	}

	private static IDataSource CreateDataSourceMock(DataSourceType dataSourceType)
	{
		var dataSourceMock = Substitute.For<IDataSource>();
		dataSourceMock.Type.Returns(dataSourceType);
		return dataSourceMock;
	}
}
