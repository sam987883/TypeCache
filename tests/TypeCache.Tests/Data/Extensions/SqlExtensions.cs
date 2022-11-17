// Copyright (c) 2021 Samuel Abraham

using Moq;
using TypeCache.Attributes;
using TypeCache.Data;
using TypeCache.Data.Extensions;
using Xunit;
using static System.FormattableString;
using static TypeCache.Data.DataSourceType;

namespace TypeCache.Tests.Data.Extensions;

public class SqlExtensions
{
	public class Person
	{
		public int ID { get; set; }

		[Name("First Name")]
		public string FirstName { get; set; }

		[Name("Last_Name")]
		public string LastName { get; set; }

		public int Age { get; set; }
	}

	[Fact]
	public void EscapeIdentifier()
	{
		var sqlServerDataSource = CreateDataSourceMock(SqlServer);

		Assert.Equal("[First Name]", sqlServerDataSource.EscapeIdentifier("First Name"));
		Assert.Equal("[[First Name]]]]]]]", sqlServerDataSource.EscapeIdentifier("[First Name]]]"));
		Assert.Equal("[Last[Name]", sqlServerDataSource.EscapeIdentifier("Last[Name"));
		Assert.Equal("[Last]]Name]", sqlServerDataSource.EscapeIdentifier("Last]Name"));

		var postGreDataSource = CreateDataSourceMock(PostgreSql);
		Assert.Equal("\"First Name\"", postGreDataSource.EscapeIdentifier("First Name"));
	}

	[Fact]
	public void EscapeLikeValue()
	{
		var dataSource = CreateDataSourceMock(SqlServer);

		Assert.Equal("aaa", dataSource.EscapeLikeValue("aaa"));
		Assert.Equal("aa[%]a", dataSource.EscapeLikeValue("aa%a"));
		Assert.Equal("[%]bbb[%]", dataSource.EscapeLikeValue("%bbb%"));
		Assert.Equal("[[]aaa[_]bbb[%]ccc''", dataSource.EscapeLikeValue("[aaa_bbb%ccc'"));
		Assert.Equal("[[][[][[][_][_][_][%][%][%]''''''", dataSource.EscapeLikeValue("[[[___%%%'''"));
	}

	[Fact]
	public void EscapeValue()
	{
		Assert.Equal("aaa", "aaa".EscapeValue());
		Assert.Equal("''a%a%a''", "'a%a%a'".EscapeValue());
		Assert.Equal("''''''", "'''".EscapeValue());
	}

	[Fact]
	public void CreateCountSQL()
	{
		var dataSource = CreateDataSourceMock(SqlServer);
		var table = new DatabaseObject("db.dbo.Test");
		var objectSchema = new ObjectSchema(dataSource, ObjectType.Table, table, "db", "dbo", "Test");

		var expected = Invariant($@"SELECT COUNT(*)
FROM {table} WITH(NOLOCK)
WHERE [First Name] = N'Sarah' AND [Last_Name] = N'Marshal';
");
		var actual = objectSchema.CreateCountSQL(null, "[First Name] = N'Sarah' AND [Last_Name] = N'Marshal'");

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void CreateDeleteBatchSQL()
	{
		var dataSource = CreateDataSourceMock(SqlServer);
		var table = new DatabaseObject("db.dbo.Test");
		var objectSchema = new ObjectSchema(dataSource, ObjectType.Table, table, "db", "dbo", "Test", new[]
		{
			new ColumnSchema("ID", false, true, true, true, typeof(int).TypeHandle),
			new ColumnSchema("First Name", false, false, false, false, typeof(string).TypeHandle),
			new ColumnSchema("Last Name", false, false, false, false, typeof(string).TypeHandle),
		});

		var expected = Invariant($@"DELETE {table}
OUTPUT INSERTED.[First Name] AS [First Name], DELETED.[Last_Name] AS [Last_Name], INSERTED.ID
FROM {table} AS _
INNER JOIN
(
VALUES (1)
	, (2)
	, (3)
) AS data ([ID])
ON data.[ID] = _.[ID];
");
		var data = new[] { new Person { ID = 1 }, new Person { ID = 2 }, new Person { ID = 3 } };
		var actual = objectSchema.CreateDeleteSQL<Person>(data, new[] { "INSERTED.[First Name] AS [First Name]", "DELETED.[Last_Name] AS [Last_Name]", "INSERTED.ID" });

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void CreateDeleteSQL()
	{
		var dataSource = CreateDataSourceMock(SqlServer);
		var table = new DatabaseObject("db.dbo.Test");
		var objectSchema = new ObjectSchema(dataSource, ObjectType.Table, table, "db", "dbo", "Test", new[]
		{
			new ColumnSchema("ID", false, true, true, true, typeof(int).TypeHandle),
			new ColumnSchema("First Name", false, false, false, false, typeof(string).TypeHandle),
			new ColumnSchema("Last Name", false, false, false, false, typeof(string).TypeHandle),
		});

		var expected = Invariant($@"DELETE {table}
OUTPUT DELETED.[ID], DELETED.[First Name], DELETED.[Last_Name]
WHERE [First Name] = N'Sarah' AND [Last_Name] = N'Marshal';
");
		var actual = objectSchema.CreateDeleteSQL("[First Name] = N'Sarah' AND [Last_Name] = N'Marshal'", "DELETED.[ID]", "DELETED.[First Name]", "DELETED.[Last_Name]");

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void CreateBatchInsertSQL()
	{
		var dataSource = CreateDataSourceMock(SqlServer);
		var table = new DatabaseObject("db.dbo.Test");
		var objectSchema = new ObjectSchema(dataSource, ObjectType.Table, table, "db", "dbo", "Test", new[]
		{
			new ColumnSchema("ID", false, true, true, true, typeof(int).TypeHandle),
			new ColumnSchema("First Name", false, false, false, false, typeof(string).TypeHandle),
			new ColumnSchema("Last Name", false, false, false, false, typeof(string).TypeHandle),
		});
		var data = new[]
		{
			new Person { ID = 1, FirstName = "FirstName1", LastName = "LastName1", Age = 30 },
			new Person { ID = 2, FirstName = "FirstName2", LastName = "LastName2", Age = 31 },
			new Person { ID = 3, FirstName = "FirstName3", LastName = "LastName3", Age = 32 }
		};

		var expected = Invariant($@"INSERT INTO {table}
([First Name], [Last_Name])
OUTPUT INSERTED.ID, INSERTED.[Last Name]
VALUES (N'FirstName1', N'LastName1')
	, (N'FirstName2', N'LastName2')
	, (N'FirstName3', N'LastName3');
");
		var actual = objectSchema.CreateInsertSQL<Person>(new[] { "First Name", "Last_Name" }, data, "INSERTED.ID", "INSERTED.[Last Name]");

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void CreateInsertSQL()
	{
		var dataSource = CreateDataSourceMock(SqlServer);
		var table = new DatabaseObject("db.dbo.Test");
		var objectSchema = new ObjectSchema(dataSource, ObjectType.Table, table, "db", "dbo", "Test", new[]
		{
			new ColumnSchema("ID", false, true, true, true, typeof(int).TypeHandle),
			new ColumnSchema("First Name", false, false, false, false, typeof(string).TypeHandle),
			new ColumnSchema("Last Name", false, false, false, false, typeof(string).TypeHandle),
		});
		var selectQuery = new SelectQuery
		{
			From = new("[dbo].[NonCustomers]"),
			Having = "MAX([Age]) > 40",
			OrderBy = new[] { "[First Name] ASC", "Last_Name DESC" },
			Select = new[] { "ID", "TRIM([First Name]) AS [First Name]", "UPPER([LastName]) AS LastName", "40 Age", "Amount AS Amount" },
			TableHints = "WITH(NOLOCK)",
			Where = "[First Name] = N'Sarah' AND [Last_Name] = N'Marshal'",
		};

		var expected = Invariant($@"INSERT INTO {table}
([[First Name]]], [[Last_Name]]], [Age], [Amount])
OUTPUT INSERTED.[First Name] AS [First Name], INSERTED.[ID] AS [ID]
SELECT ID, TRIM([First Name]) AS [First Name], UPPER([LastName]) AS LastName, 40 Age, Amount AS Amount
FROM [dbo].[NonCustomers] WITH(NOLOCK)
WHERE [First Name] = N'Sarah' AND [Last_Name] = N'Marshal'
HAVING MAX([Age]) > 40
ORDER BY [First Name] ASC, Last_Name DESC;
");
		var actual = objectSchema.CreateInsertSQL(new[] { "[First Name]", "[Last_Name]", "Age", "Amount" }, selectQuery, "INSERTED.[First Name] AS [First Name]", "INSERTED.[ID] AS [ID]");

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void CreateSelectSQL()
	{
		var dataSource = CreateDataSourceMock(SqlServer);
		var objectSchema = new ObjectSchema(dataSource, ObjectType.Table, new DatabaseObject("db.dbo.Test"), "db", "dbo", "Test", new[]
		{
			new ColumnSchema("ID", false, true, true, true, typeof(int).TypeHandle),
			new ColumnSchema("First Name", false, false, false, false, typeof(string).TypeHandle),
			new ColumnSchema("Last Name", false, false, false, false, typeof(string).TypeHandle),
		});
		var selectQuery = new SelectQuery
		{
			Select = new[] { "ID", "TRIM([First Name]) AS [First Name]", "UPPER([LastName]) AS LastName", "40 Age", "Amount AS Amount" },
			From = new("[dbo].[NonCustomers]"),
			Having = "MAX([Age]) > 40",
			Where = "[First Name] = N'Sarah' AND [Last_Name] = N'Marshal'",
			OrderBy = new[] { "[First Name] ASC", "Last_Name DESC" },
			Fetch = 100,
			Offset = 0,
			TableHints = "WITH(NOLOCK)"
		};

		var expected = Invariant($@"SELECT ID, TRIM([First Name]) AS [First Name], UPPER([LastName]) AS LastName, 40 Age, Amount AS Amount
FROM [dbo].[NonCustomers] WITH(NOLOCK)
WHERE [First Name] = N'Sarah' AND [Last_Name] = N'Marshal'
HAVING MAX([Age]) > 40
ORDER BY [First Name] ASC, Last_Name DESC
FETCH NEXT 100 ROWS ONLY;
");
		var actual = objectSchema.CreateSelectSQL(selectQuery);

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void CreateBatchUpdateSQL()
	{
		var dataSource = CreateDataSourceMock(SqlServer);
		var table = new DatabaseObject("db.dbo.Test");
		var objectSchema = new ObjectSchema(dataSource, ObjectType.Table, table, "db", "dbo", "Test", new[]
		{
			new ColumnSchema("ID", false, true, true, true, typeof(int).TypeHandle),
			new ColumnSchema("First Name", false, false, false, false, typeof(string).TypeHandle),
			new ColumnSchema("Last Name", false, false, false, false, typeof(string).TypeHandle),
		});
		var data = new[]
		{
			new Person { ID = 1, FirstName = "FirstName1", LastName = "LastName1", Age = 30 },
			new Person { ID = 2, FirstName = "FirstName2", LastName = "LastName2", Age = 31 },
			new Person { ID = 3, FirstName = "FirstName3", LastName = "LastName3", Age = 32 }
		};

		var expected = Invariant($@"UPDATE {table} WITH(UPDLOCK)
SET [First Name] = data.[First Name], [Last_Name] = data.[Last_Name]
OUTPUT INSERTED.[First Name] AS [First Name], DELETED.Last_Name AS Last_Name, INSERTED.[ID] AS [ID]
FROM {table} AS _
INNER JOIN
(
VALUES (N'FirstName1', N'LastName1')
	, (N'FirstName2', N'LastName2')
	, (N'FirstName3', N'LastName3')
) AS data ([First Name], [Last_Name])
ON data.[ID] = _.[ID];
");
		var actual = objectSchema.CreateUpdateSQL<Person>(new[] { "First Name", "Last_Name" }, data
			, "INSERTED.[First Name] AS [First Name]", "DELETED.Last_Name AS Last_Name", "INSERTED.[ID] AS [ID]");

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void CreateUpdateSQL()
	{
		var dataSource = CreateDataSourceMock(SqlServer);
		var table = new DatabaseObject("db.dbo.Test");
		var objectSchema = new ObjectSchema(dataSource, ObjectType.Table, table, "db", "dbo", "Test", new[]
		{
			new ColumnSchema("ID", false, true, true, true, typeof(int).TypeHandle),
			new ColumnSchema("First Name", false, false, false, false, typeof(string).TypeHandle),
			new ColumnSchema("Last Name", false, false, false, false, typeof(string).TypeHandle),
		});

		var expected = Invariant($@"UPDATE {table} WITH(UPDLOCK)
SET [First Name] = N'Sarah', Last_Name = N'Marshal', Age = @Param1
OUTPUT INSERTED.[First Name] AS [First Name], DELETED.[Last_Name] AS [Last_Name], INSERTED.ID AS [ID]
WHERE [First Name] = N'Sarah' AND [Last_Name] = N'Marshal';
");
		var actual = objectSchema.CreateUpdateSQL(new[] { "[First Name] = N'Sarah'", "Last_Name = N'Marshal'", "Age = @Param1" }
			, "[First Name] = N'Sarah' AND [Last_Name] = N'Marshal'"
			, "INSERTED.[First Name] AS [First Name]", "DELETED.[Last_Name] AS [Last_Name]", "INSERTED.ID AS [ID]");

		Assert.Equal(expected, actual);
	}

	private static IDataSource CreateDataSourceMock(DataSourceType dataSourceType)
	{
		var dataSourceMock = new Mock<IDataSource>();
		dataSourceMock.Setup(dataSource => dataSource.Type).Returns(dataSourceType);
		dataSourceMock.Setup(dataSource => dataSource.EscapeIdentifier(It.IsAny<string>()))
			.Returns((string identifier) => dataSourceType switch
			{
				PostgreSql => Invariant($"\"{identifier}\""),
				_ => Invariant($"[{identifier.Replace("]", "]]")}]")
			});
		dataSourceMock.Setup(dataSource => dataSource.EscapeLikeValue(It.IsAny<string>()))
			.Returns((string text) => text.Replace("'", "''").Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]"));
		dataSourceMock.Setup(dataSource => dataSource.EscapeValue(It.IsAny<string>()))
			.Returns((string text) => text.Replace("'", "''"));
		return dataSourceMock.Object;
	}
}
