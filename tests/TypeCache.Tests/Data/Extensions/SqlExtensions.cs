using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using TypeCache.Data;
using TypeCache.Data.Extensions;
using Xunit;

namespace TypeCache.Tests.Data.Extensions
{
	public class SqlExtensions
	{
		public SqlExtensions()
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
		}

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
		public void ToSQL_BatchRequest()
		{
			var date = DateTime.UtcNow;

			var request = new BatchRequest
			{
				Delete = false,
				Input = new RowSet(new[] { "ID", "Col 2", "Column3]" }, new[]
				{
					new object[] {1, "aaa", date },
					new object[] {2, "bbb", date },
					new object[] {3, "ccc", date },
				}),
				Insert = new[] { "Col 2", "Column3]" },
				On = new[] { "ID" },
				Table = "Customers",
			};

			request.Update = null;

			var expected = @$"INSERT INTO Customers ([Col 2], [Column3]]])
VALUES (1, N'aaa', {date.ToSQL()})
	, (2, N'bbb', {date.ToSQL()})
	, (3, N'ccc', {date.ToSQL()});
";

			Assert.Equal(expected, request.ToSQL());

			request.Output = new Dictionary<string, string> { { "ID", "INSERTED.[ID]" }, { "Col 2", "INSERTED" }, { "Column3]", "INSERTED" } };

			expected = @$"INSERT INTO Customers ([Col 2], [Column3]]])
OUTPUT INSERTED.[ID] AS [ID]
	, INSERTED.[Col 2] AS [Col 2]
	, INSERTED.[Column3]]] AS [Column3]]]
VALUES (1, N'aaa', {date.ToSQL()})
	, (2, N'bbb', {date.ToSQL()})
	, (3, N'ccc', {date.ToSQL()});
";

			Assert.Equal(expected, request.ToSQL());

			request.Delete = true;

			expected = @$"MERGE Customers AS t WITH(UPDLOCK)
USING
(
	VALUES (1, N'aaa', {date.ToSQL()})
	, (2, N'bbb', {date.ToSQL()})
	, (3, N'ccc', {date.ToSQL()})
) AS s ([ID], [Col 2], [Column3]]])
ON s.[ID] = t.[ID]
WHEN NOT MATCHED BY SOURCE THEN
	DELETE
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([Col 2], [Column3]]])
	VALUES
	(
	s.[Col 2]
	, s.[Column3]]]
	)
OUTPUT INSERTED.[ID] AS [ID]
	, INSERTED.[Col 2] AS [Col 2]
	, INSERTED.[Column3]]] AS [Column3]]];
";

			Assert.Equal(expected, request.ToSQL());

			request.Delete = false;
			request.Update = new[] { "Col 2" };

			expected = @$"MERGE Customers AS t WITH(UPDLOCK)
USING
(
	VALUES (1, N'aaa', {date.ToSQL()})
	, (2, N'bbb', {date.ToSQL()})
	, (3, N'ccc', {date.ToSQL()})
) AS s ([ID], [Col 2], [Column3]]])
ON s.[ID] = t.[ID]
WHEN MATCHED THEN
	UPDATE SET [Col 2] = s.[Col 2]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([Col 2], [Column3]]])
	VALUES
	(
	s.[Col 2]
	, s.[Column3]]]
	)
OUTPUT INSERTED.[ID] AS [ID]
	, INSERTED.[Col 2] AS [Col 2]
	, INSERTED.[Column3]]] AS [Column3]]];
";

			Assert.Equal(expected, request.ToSQL());

			request.Insert = null;
			request.Update = new[] { "Col 2", "Column3]" };

			expected = @$"MERGE Customers AS t WITH(UPDLOCK)
USING
(
	VALUES (1, N'aaa', {date.ToSQL()})
	, (2, N'bbb', {date.ToSQL()})
	, (3, N'ccc', {date.ToSQL()})
) AS s ([ID], [Col 2], [Column3]]])
ON s.[ID] = t.[ID]
WHEN MATCHED THEN
	UPDATE SET [Col 2] = s.[Col 2]
	, [Column3]]] = s.[Column3]]]
OUTPUT INSERTED.[ID] AS [ID]
	, INSERTED.[Col 2] AS [Col 2]
	, INSERTED.[Column3]]] AS [Column3]]];
";

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

			var expected = $@"DELETE FROM Customers
OUTPUT INSERTED.[First Name] AS [First Name]
	, DELETED.[Last_Name] AS [Last_Name]
	, INSERTED.[ID] AS [ID]
WHERE [First Name] = N'Sarah' AND [Last_Name] = N'Marshal';
";

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
				OrderBy = new Dictionary<string, Sort> { { "First Name", Sort.Descending }, { "Last_Name", Sort.Ascending } }
			};

			var expected = $@"INSERT INTO Customers ([ID], [First Name], [Last_Name], [Age], [Amount])
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
ORDER BY [First Name] DESC, [Last_Name] ASC;
";

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
				OrderBy = new Dictionary<string, Sort> { { "First Name", Sort.Descending }, { "Last_Name", Sort.Ascending } }
			};

			var expected = $@"SELECT [ID]
	, TRIM([First Name]) AS [First Name]
	, UPPER([LastName]) AS [LastName]
	, 40 AS [Age]
	, [Amount]
FROM [dbo].[NonCustomers] WITH(NOLOCK)
WHERE [First Name] = N'Sarah' AND [Last_Name] = N'Marshal'
HAVING MAX([Age]) > 40
ORDER BY [First Name] DESC, [Last_Name] ASC;
";

			Assert.Equal(expected, request.ToSQL());
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

			var expected = $@"UPDATE Customers WITH(UPDLOCK)
SET [ID] = 123456
	, [First Name] = N'Sarah'
	, [Last_Name] = N'Marshal'
	, [Account] = '{id:D}'
OUTPUT INSERTED.[First Name] AS [First Name]
	, DELETED.[Last_Name] AS [Last_Name]
	, INSERTED.[ID] AS [ID]
WHERE [First Name] = N'Sarah' AND [Last_Name] = N'Marshal';
";

			Assert.Equal(expected, request.ToSQL());
		}
	}
}
