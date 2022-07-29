// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Attributes;
using TypeCache.Collections.Extensions;
using TypeCache.Data.Domain;
using TypeCache.Data.Extensions;
using TypeCache.Extensions;
using Xunit;
using static System.FormattableString;

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
		Assert.Equal("[First Name]", "First Name".EscapeIdentifier());
		Assert.Equal("[[First Name]]]]]]]", "[First Name]]]".EscapeIdentifier());
		Assert.Equal("[Last[Name]", "Last[Name".EscapeIdentifier());
		Assert.Equal("[Last]]Name]", "Last]Name".EscapeIdentifier());
	}

	[Fact]
	public void EscapeLikeValue()
	{
		Assert.Equal("aaa", "aaa".EscapeLikeValue());
		Assert.Equal("aa[%]a", "aa%a".EscapeLikeValue());
		Assert.Equal("[%]bbb[%]", "%bbb%".EscapeLikeValue());
		Assert.Equal("[[]aaa[_]bbb[%]ccc''", "[aaa_bbb%ccc'".EscapeLikeValue());
		Assert.Equal("[[][[][[][_][_][_][%][%][%]''''''", "[[[___%%%'''".EscapeLikeValue());
	}

	[Fact]
	public void EscapeValue()
	{
		Assert.Equal("aaa", "aaa".EscapeValue());
		Assert.Equal("''a%a%a''", "'a%a%a'".EscapeValue());
		Assert.Equal("''''''", "'''".EscapeValue());
	}

	[Fact]
	public void ToSQL_CountRequest()
	{
		var command = new CountCommand
		{
			Table = "[dbo].[NonCustomers]",
			Where = "[First Name] = N'Sarah' AND [Last_Name] = N'Marshal'"
		};

		var expected = Invariant($@"SELECT COUNT_BIG(*)
FROM [dbo].[NonCustomers] WITH(NOLOCK)
WHERE [First Name] = N'Sarah' AND [Last_Name] = N'Marshal';
");

		Assert.Equal(expected, command.ToSQL());
	}

	[Fact]
	public void ToSQL_DeleteDataRequest()
	{
		var date = DateTime.UtcNow;
		var id = Guid.NewGuid();

		var command = new DeleteDataCommand<Guid>
		{
			Table = "Customers",
			Input = new[] { id, id, id },
			Output = new[] { "INSERTED.[First Name] AS [First Name]", "DELETED.[Last_Name] AS [Last_Name]", "INSERTED.[ID] AS [ID]" },
		};
		TypeOf<DeleteDataCommand<Guid>>.Properties.If(_ => _.Name.Is("PrimaryKeys")).First()!.SetValue(command, new[] { "ID" });

		var expected = Invariant($@"DELETE FROM _
OUTPUT INSERTED.[First Name] AS [First Name]
	, DELETED.[Last_Name] AS [Last_Name]
	, INSERTED.[ID] AS [ID]
FROM Customers _
INNER JOIN
(
VALUES ('{id:D}')
	, ('{id:D}')
	, ('{id:D}')
) AS pk ([ID])
ON pk.[ID] = _.[ID];
");

		Assert.Equal(expected, command.ToSQL());
	}

	[Fact]
	public void ToSQL_DeleteRequest()
	{
		var date = DateTime.UtcNow;
		var id = Guid.NewGuid();

		var command = new DeleteCommand
		{
			Table = "Customers",
			Output = new[] { "INSERTED.[First Name] AS [First Name]", "DELETED.[Last_Name] AS [Last_Name]", "INSERTED.[ID] AS [ID]" },
			Where = "[First Name] = N'Sarah' AND [Last_Name] = N'Marshal'"
		};

		var expected = Invariant($@"DELETE Customers
OUTPUT INSERTED.[First Name] AS [First Name]
	, DELETED.[Last_Name] AS [Last_Name]
	, INSERTED.[ID] AS [ID]
WHERE [First Name] = N'Sarah' AND [Last_Name] = N'Marshal';
");

		Assert.Equal(expected, command.ToSQL());
	}

	[Fact]
	public void ToSQL_InsertDataRequest()
	{
		var date = DateTime.UtcNow;
		var id = Guid.NewGuid();

		var command = new InsertDataCommand<Person>
		{
			Columns = new[] { "First Name", "Last_Name", "ID" },
			Table = "Customers",
			Input = new[]
			{
				new Person { ID = 1, FirstName = "FirstName1", LastName = "LastName1", Age = 30 },
				new Person { ID = 2, FirstName = "FirstName2", LastName = "LastName2", Age = 31 },
				new Person { ID = 3, FirstName = "FirstName3", LastName = "LastName3", Age = 32 }
			},
			Output = new[] { "INSERTED.[First Name] AS [First Name]", "DELETED.[Last_Name] AS [Last_Name]", "INSERTED.[ID] AS [ID]" }
		};

		var expected = Invariant($@"INSERT INTO Customers
([First Name], [Last_Name], [ID])
OUTPUT INSERTED.[First Name] AS [First Name]
	, DELETED.[Last_Name] AS [Last_Name]
	, INSERTED.[ID] AS [ID]
VALUES (N'FirstName1', N'LastName1', 1)
	, (N'FirstName2', N'LastName2', 2)
	, (N'FirstName3', N'LastName3', 3);
");

		Assert.Equal(expected, command.ToSQL());
	}

	[Fact]
	public void ToSQL_InsertRequest()
	{
		var command = new InsertCommand
		{
			From = "[dbo].[NonCustomers]",
			Table = "Customers",
			Columns = new[] { "[ID]", "[First Name]", "[Last_Name]", "Age", "Amount" },
			Having = "MAX([Age]) > 40",
			OrderBy = new[] { "[First Name] ASC", "Last_Name DESC" },
			Output = new[] { "INSERTED.[First Name] AS [First Name]", "DELETED.[Last_Name] AS [Last_Name]", "INSERTED.[ID] AS [ID]" },
			Select = new[] { "ID", "TRIM([First Name]) AS [First Name]", "UPPER([LastName]) AS LastName", "40 Age", "Amount AS Amount" },
			Where = "[First Name] = N'Sarah' AND [Last_Name] = N'Marshal'",
		};

		var expected = Invariant($@"INSERT INTO Customers
([[ID]]], [[First Name]]], [[Last_Name]]], [Age], [Amount])
OUTPUT INSERTED.[First Name] AS [First Name]
	, DELETED.[Last_Name] AS [Last_Name]
	, INSERTED.[ID] AS [ID]
SELECT ID
	, TRIM([First Name]) AS [First Name]
	, UPPER([LastName]) AS LastName
	, 40 Age
	, Amount AS Amount
FROM [dbo].[NonCustomers] WITH(NOLOCK)
WHERE [First Name] = N'Sarah' AND [Last_Name] = N'Marshal'
HAVING MAX([Age]) > 40
ORDER BY [First Name] ASC
	, Last_Name DESC;
");

		Assert.Equal(expected, command.ToSQL());
	}

	[Fact]
	public void ToSQL_SelectRequest()
	{
		var command = new SelectCommand
		{
			Select = new[] { "ID", "TRIM([First Name]) AS [First Name]", "UPPER([LastName]) AS LastName", "40 Age", "Amount AS Amount" },
			From = "[dbo].[NonCustomers]",
			Having = "MAX([Age]) > 40",
			Where = "[First Name] = N'Sarah' AND [Last_Name] = N'Marshal'",
			OrderBy = new[] { "[First Name] ASC", "Last_Name DESC" },
			Pager = new() { After = 0, First = 100 }
		};

		var expected = Invariant($@"SELECT ID
	, TRIM([First Name]) AS [First Name]
	, UPPER([LastName]) AS LastName
	, 40 Age
	, Amount AS Amount
FROM [dbo].[NonCustomers] WITH(NOLOCK)
WHERE [First Name] = N'Sarah' AND [Last_Name] = N'Marshal'
HAVING MAX([Age]) > 40
ORDER BY [First Name] ASC
	, Last_Name DESC
OFFSET 0 ROWS
FETCH NEXT 100 ROWS ONLY;

SELECT @RowCount = COUNT_BIG(1)
FROM [dbo].[NonCustomers] WITH(NOLOCK)
WHERE [First Name] = N'Sarah' AND [Last_Name] = N'Marshal';
");

		Assert.Equal(expected, command.ToSQL());
	}

	[Fact]
	public void ToSQL_UpdateDataRequest()
	{
		var date = DateTime.UtcNow;
		var id = Guid.NewGuid();

		var command = new UpdateDataCommand<Person>
		{
			Columns = new[] { "First Name", "Last_Name", "ID" },
			Table = "Customers",
			Input = new[]
			{
				new Person { ID = 1, FirstName = "FirstName1", LastName = "LastName1", Age = 30 },
				new Person { ID = 2, FirstName = "FirstName2", LastName = "LastName2", Age = 31 },
				new Person { ID = 3, FirstName = "FirstName3", LastName = "LastName3", Age = 32 }
			},
			On = new[] { "ID1", "[ID2]" },
			Output = new[] { "INSERTED.[First Name] AS [First Name]", "DELETED.[Last_Name] AS [Last_Name]", "INSERTED.[ID] AS [ID]" },
			TableHints = "WITH(UPDLOCK)"
		};

		var expected = Invariant($@"UPDATE _ WITH(UPDLOCK)
SET [First Name] = data.[First Name]
	, [Last_Name] = data.[Last_Name]
	, [ID] = data.[ID]
OUTPUT INSERTED.[First Name] AS [First Name]
	, DELETED.[Last_Name] AS [Last_Name]
	, INSERTED.[ID] AS [ID]
FROM Customers _
INNER JOIN
(
VALUES (N'FirstName1', N'LastName1', 1)
	, (N'FirstName2', N'LastName2', 2)
	, (N'FirstName3', N'LastName3', 3)
) AS data ([First Name], [Last_Name], [ID])
ON data.[ID1] = _.[ID1] AND data.[[ID2]]] = _.[[ID2]]];
");

		Assert.Equal(expected, command.ToSQL());
	}

	[Fact]
	public void ToSQL_UpdateRequest()
	{
		var id = Guid.NewGuid();

		var command = new UpdateCommand
		{
			Table = "Customers",
			Columns = new[] { "ID = 123456", "[First Name] = N'Sarah'", "Last_Name = N'Marshal'", "Account = @Param1" },
			Output = new[] { "INSERTED.[First Name] AS [First Name]", "DELETED.[Last_Name] AS [Last_Name]", "INSERTED.[ID] AS [ID]" },
			Where = "[First Name] = N'Sarah' AND [Last_Name] = N'Marshal'"
		};

		var expected = Invariant($@"UPDATE Customers WITH(UPDLOCK)
SET ID = 123456
	, [First Name] = N'Sarah'
	, Last_Name = N'Marshal'
	, Account = @Param1
OUTPUT INSERTED.[First Name] AS [First Name]
	, DELETED.[Last_Name] AS [Last_Name]
	, INSERTED.[ID] AS [ID]
WHERE [First Name] = N'Sarah' AND [Last_Name] = N'Marshal';
");

		Assert.Equal(expected, command.ToSQL());
	}
}
