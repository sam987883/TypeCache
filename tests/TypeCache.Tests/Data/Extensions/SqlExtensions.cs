using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using TypeCache.Collections;
using TypeCache.Data;
using TypeCache.Data.Extensions;
using TypeCache.Data.Requests;
using TypeCache.Data.Schema;
using Xunit;
using static System.FormattableString;

namespace TypeCache.Tests.Data.Extensions
{
	public class SqlExtensions
	{
		[Fact]
		public void EscapeIdentifier()
		{
			Assert.Equal("[First Name]", "First Name".EscapeIdentifier());
			Assert.Equal("[First Name]]]]]", "[First Name]]]".EscapeIdentifier());
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
		public void ToSQL_DeleteDataRequest()
		{
			var date = DateTime.UtcNow;
			var id = Guid.NewGuid();

			var request = new DeleteDataRequest
			{
				From = "Customers",
				Input = new()
				{
					Columns = new[] { "ID1", "ID2" },
					Rows = new object[][] { new object[] { 1, 2 }, new object[] { 1, 3 }, new object[] { 2, 1 } }
				},
				Output = new Dictionary<string, string> { { "First Name", "INSERTED" }, { "Last_Name", "DELETED" }, { "ID", "INSERTED" } }
			};

			var expected = Invariant($@"DELETE x
OUTPUT INSERTED.[First Name] AS [First Name]
	, DELETED.[Last_Name] AS [Last_Name]
	, INSERTED.[ID] AS [ID]
FROM Customers AS x
INNER JOIN
(
VALUES (1, 2)
	, (1, 3)
	, (2, 1)
) AS i ([ID1], [ID2])
ON i.[ID1] = x.[ID1] AND i.[ID2] = x.[ID2];
");

			Assert.Equal(expected, request.ToSQL());
		}

		[Fact]
		public void ToSQL_DeleteRequest()
		{
			var date = DateTime.UtcNow;
			var id = Guid.NewGuid();

			var request = new DeleteRequest
			{
				From = "Customers",
				Output = new Dictionary<string, string> { { "First Name", "INSERTED" }, { "Last_Name", "DELETED" }, { "ID", "INSERTED" } },
				Where = "[First Name] = N'Sarah' AND [Last_Name] = N'Marshal'"
			};

			var expected = Invariant($@"DELETE FROM Customers
OUTPUT INSERTED.[First Name] AS [First Name]
	, DELETED.[Last_Name] AS [Last_Name]
	, INSERTED.[ID] AS [ID]
WHERE [First Name] = N'Sarah' AND [Last_Name] = N'Marshal';
");

			Assert.Equal(expected, request.ToSQL());
		}

		[Fact]
		public void ToSQL_InsertDataRequest()
		{
			var date = DateTime.UtcNow;
			var id = Guid.NewGuid();

			var request = new InsertDataRequest
			{
				Into = "Customers",
				Input = new()
				{
					Columns = new[] { "First Name", "Last_Name", "ID" },
					Rows = new object[][]
					{
						new object[] { "FirstName1", "LastName1", 1 },
						new object[] { "FirstName2", "LastName2", 2 },
						new object[] { "FirstName3", "LastName3", 3 }
					}
				},
				Output = new Dictionary<string, string> { { "First Name", "INSERTED" }, { "Last_Name", "DELETED" }, { "ID", "INSERTED" } }
			};

			var expected = Invariant($@"INSERT INTO Customers ([First Name], [Last_Name], [ID])
OUTPUT INSERTED.[First Name] AS [First Name]
	, DELETED.[Last_Name] AS [Last_Name]
	, INSERTED.[ID] AS [ID]
VALUES (N'FirstName1', N'LastName1', 1)
	, (N'FirstName2', N'LastName2', 2)
	, (N'FirstName3', N'LastName3', 3);
");

			Assert.Equal(expected, request.ToSQL());
		}

		[Fact]
		public void ToSQL_InsertRequest()
		{
			var request = new InsertRequest
			{
				Into = "Customers",
				Insert = new[] { "[ID]", "[First Name]", "[Last_Name]", "[Age]", "[Amount]" },
				Select = new Dictionary<string, string>(5) { { "ID", "ID" }, { "First Name", "TRIM([First Name])" }, { "LastName", "UPPER([LastName])" }, { "Age", "40" }, { "Amount", "Amount" } },
				From = "[dbo].[NonCustomers]",
				Output = new Dictionary<string, string>(3) { { "First Name", "INSERTED" }, { "Last_Name", "DELETED" }, { "ID", "INSERTED" } },
				Having = "MAX([Age]) > 40",
				Where = "[First Name] = N'Sarah' AND [Last_Name] = N'Marshal'",
				OrderBy = new[] { ("First Name", Sort.Descending), ("Last_Name", Sort.Ascending), ("1", Sort.Descending) }
			};

			var expected = Invariant($@"INSERT INTO Customers ([ID], [First Name], [Last_Name], [Age], [Amount])
OUTPUT INSERTED.[First Name] AS [First Name]
	, DELETED.[Last_Name] AS [Last_Name]
	, INSERTED.[ID] AS [ID]
SELECT [ID]
	, TRIM([First Name]) AS [First Name]
	, UPPER([LastName]) AS [LastName]
	, 40 AS [Age]
	, [Amount]
FROM [dbo].[NonCustomers] WITH(NOLOCK)
WHERE [First Name] = N'Sarah' AND [Last_Name] = N'Marshal'
HAVING MAX([Age]) > 40
ORDER BY [First Name] DESC, [Last_Name] ASC, 1 DESC;
");

			Assert.Equal(expected, request.ToSQL());
		}

		[Fact]
		public void ToSQL_SelectRequest()
		{
			var request = new SelectRequest
			{
				Select = new Dictionary<string, string>(5) { { "ID", "ID" }, { "First Name", "TRIM([First Name])" }, { "LastName", "UPPER([LastName])" }, { "Age", "40" }, { "Amount", "Amount" } },
				From = "[dbo].[NonCustomers]",
				Having = "MAX([Age]) > 40",
				Where = "[First Name] = N'Sarah' AND [Last_Name] = N'Marshal'",
				OrderBy = new[] { ("First Name", Sort.Descending), ("Last_Name", Sort.Ascending) }
			};

			var expected = Invariant($@"SELECT [ID]
	, TRIM([First Name]) AS [First Name]
	, UPPER([LastName]) AS [LastName]
	, 40 AS [Age]
	, [Amount]
FROM [dbo].[NonCustomers] WITH(NOLOCK)
WHERE [First Name] = N'Sarah' AND [Last_Name] = N'Marshal'
HAVING MAX([Age]) > 40
ORDER BY [First Name] DESC, [Last_Name] ASC;
");

			Assert.Equal(expected, request.ToSQL());
		}

		[Fact]
		public void ToSQL_UpdateDataRequest()
		{
			var date = DateTime.UtcNow;
			var id = Guid.NewGuid();

			var schemaRowSet = new RowSet
			{
				Columns = new[] { nameof(ObjectSchema.Id), nameof(ObjectSchema.Type), nameof(ObjectSchema.DatabaseName), nameof(ObjectSchema.SchemaName), nameof(ObjectSchema.ObjectName) },
				Rows = new object[][] { new object[] { 1, ObjectType.Table, "LocalDb", "dbo", "Customers" } }
			};
			var schema = new ObjectSchema("Default", schemaRowSet, new ColumnSchema[]
			{
				new() { Name = "ID1", PrimaryKey = true },
				new() { Name = "ID2", PrimaryKey = true },
				new() { Name = "First Name" },
				new() { Name = "Last_Name" },
				new() { Name = "ID" },
			}, Array<ParameterSchema>.Empty);
			var request = new UpdateDataRequest
			{
				Table = "Customers",
				Input = new()
				{
					Columns = new[] { "ID1", "ID2", "First Name", "Last_Name", "ID" },
					Rows = new object[][]
					{
						new object[] { 1, 2, "FirstName1", "LastName1", 1 },
						new object[] { 1, 3, "FirstName2", "LastName2", 2 },
						new object[] { 2, 1, "FirstName3", "LastName3", 3 }
					}
				},
				Output = new Dictionary<string, string> { { "First Name", "INSERTED" }, { "Last_Name", "DELETED" }, { "ID", "INSERTED" } }
			};

			var expected = Invariant($@"UPDATE x WITH(UPDLOCK)
SET [First Name] = i.[First Name]
	, [Last_Name] = i.[Last_Name]
	, [ID] = i.[ID]
OUTPUT INSERTED.[First Name] AS [First Name]
	, DELETED.[Last_Name] AS [Last_Name]
	, INSERTED.[ID] AS [ID]
FROM Customers AS x
INNER JOIN
(
VALUES (1, 2, N'FirstName1', N'LastName1', 1)
	, (1, 3, N'FirstName2', N'LastName2', 2)
	, (2, 1, N'FirstName3', N'LastName3', 3)
) AS i ([ID1], [ID2], [First Name], [Last_Name], [ID])
ON i.[ID1] = x.[ID1] AND i.[ID2] = x.[ID2];
");

			Assert.Equal(expected, request.ToSQL(schema));
		}

		[Fact]
		public void ToSQL_UpdateRequest()
		{
			var id = Guid.NewGuid();

			var request = new UpdateRequest
			{
				Table = "Customers",
				Set = new Dictionary<string, object> { { "ID", 123456 }, { "First Name", "Sarah" }, { "Last_Name", "Marshal" }, { "Account", id } },
				Output = new Dictionary<string, string> { { "First Name", "INSERTED" }, { "Last_Name", "DELETED" }, { "ID", "INSERTED" } },
				Where = "[First Name] = N'Sarah' AND [Last_Name] = N'Marshal'"
			};

			var expected = Invariant($@"UPDATE Customers WITH(UPDLOCK)
SET [ID] = 123456
	, [First Name] = N'Sarah'
	, [Last_Name] = N'Marshal'
	, [Account] = '{id:D}'
OUTPUT INSERTED.[First Name] AS [First Name]
	, DELETED.[Last_Name] AS [Last_Name]
	, INSERTED.[ID] AS [ID]
WHERE [First Name] = N'Sarah' AND [Last_Name] = N'Marshal';
");

			Assert.Equal(expected, request.ToSQL());
		}
	}
}
