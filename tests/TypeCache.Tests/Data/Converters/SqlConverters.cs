using System;
using System.Collections.Generic;
using System.Text.Json;
using TypeCache.Data;
using TypeCache.Data.Extensions;
using Xunit;
using static System.FormattableString;

namespace TypeCache.Tests.Data.Converters
{
	public class SqlConverters
	{
		[Fact]
		public void ToJSON_BatchRequest()
		{
			var date = new DateTime(637621814434623833L);
			var guid = Guid.NewGuid();

			var expectedRequest = new BatchRequest
			{
				Delete = true,
				Input = new RowSet
				{
					 Columns = new[] { "ID", "Col 2", "Column3]" },
					 Rows = new[]
					{
						new object[] {1, "aaa", date, guid },
						new object[] {2, "bbb", date, guid },
						new object[] {3, "ccc", date, guid },
					}
				},
				Insert = new[] { "Col 2", "Column3]" },
				On = new[] { "ID" },
				Output = new Dictionary<string, string> { { "ID", "INSERTED.[ID]" }, { "Col 2", "INSERTED" }, { "Column3]", "INSERTED" } },
				Table = "Customers",
				Update = new[] { "Col 2", "Column3]" }
			};

			var expected = Invariant(@$"
{{
	""Delete"":true,
	""Input"":{{""Columns"":[""ID"",""Col 2"",""Column3]""],""Rows"":[[1,""aaa"",""{date:O}"",""{guid:D}""],[2,""bbb"",""{date:O}"",""{guid:D}""],[3,""ccc"",""{date:O}"",""{guid:D}""]]}},
	""Insert"":[""Col 2"",""Column3]""],
	""On"":[""ID""],
	""Output"":{{""ID"":""INSERTED.[ID]"",""Col 2"":""INSERTED"",""Column3]"":""INSERTED""}},
	""Table"":""Customers"",
	""Update"":[""Col 2"",""Column3]""]
}}").Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\t", string.Empty);

			Assert.Equal(expected, JsonSerializer.Serialize(expectedRequest));

			var request = JsonSerializer.Deserialize<BatchRequest>(expected);

			Assert.Equal(expectedRequest.Delete, request.Delete);
			Assert.Equal(expectedRequest.Input.Columns, request.Input.Columns);
			Assert.Equal(expectedRequest.Input.Rows, request.Input.Rows);
			Assert.Equal(expectedRequest.Insert, request.Insert);
			Assert.Equal(expectedRequest.On, request.On);
			Assert.Equal(expectedRequest.Output, request.Output);
			Assert.Equal(expectedRequest.Table, request.Table);
			Assert.Equal(expectedRequest.Update, request.Update);
		}

		[Fact]
		public void ToJSON_DeleteRequest()
		{
			var date = new DateTime(637621814434623833L);
			var guid = Guid.NewGuid();

			var expectedRequest = new DeleteRequest
			{
				From = "Customers",
				Output = new Dictionary<string, string> { { "First Name", "INSERTED" }, { "Last_Name", "DELETED" }, { "ID", "INSERTED" } },
				Where = "[First Name] = N'Sarah' AND [Last_Name] = N'Marshal'",
				Parameters = new Dictionary<string, object> { { "Param1", false }, { "Param 2", date }, { "Param_3", "String Value" }, { "Param4", guid } }
			};

			var expected = Invariant(@$"
{{
	""From"":""Customers"",
	""Output"":{{""First Name"":""INSERTED"",""Last_Name"":""DELETED"",""ID"":""INSERTED""}},
	""Parameters"":{{""Param1"":false,""Param 2"":""{date:O}"",""Param_3"":""String Value"",""Param4"":""{guid:D}""}},
	""Where"":""[First Name] = N\u0027Sarah\u0027 AND [Last_Name] = N\u0027Marshal\u0027""
}}").Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\t", string.Empty);

			Assert.Equal(expected, JsonSerializer.Serialize(expectedRequest));

			var request = JsonSerializer.Deserialize<DeleteRequest>(expected);

			Assert.Equal(expectedRequest.From, request.From);
			Assert.Equal(expectedRequest.Output, request.Output);
			Assert.Equal(expectedRequest.Where, request.Where);
			Assert.Equal(expectedRequest.Parameters, request.Parameters);
		}

		[Fact]
		public void ToJSON_InsertRequest()
		{
			var date = new DateTime(637621814434623833L);
			var guid = Guid.NewGuid();

			var expectedRequest = new InsertRequest
			{
				Into = "Customers",
				Insert = new[] { "[ID]", "[First Name]", "[Last_Name]", "[Age]", "[Amount]" },
				Output = new Dictionary<string, string>(3) { { "First Name", "INSERTED" }, { "Last_Name", "DELETED" }, { "ID", "INSERTED" } },
				From = "[dbo].[NonCustomers]",
				Having = "MAX([Age]) > 40",
				OrderBy = new Dictionary<string, Sort> { { "First Name", Sort.Descending }, { "Last_Name", Sort.Ascending } },
				Parameters = new Dictionary<string, object> { { "Param1", 333.66M }, { "Param 2", date }, { "Param_3", "String Value" }, { "Param4", guid } },
				Select = new Dictionary<string, string>(5) { { "ID", "ID" }, { "First Name", "TRIM([First Name])" }, { "LastName", "UPPER([LastName])" }, { "Age", "40" }, { "Amount", "Amount" } },
				Where = "[First Name] = N'Sarah' AND [Last_Name] = N'Marshal'"
			};

			var expected = Invariant(@$"
{{
	""Into"":""Customers"",
	""Insert"":[""[ID]"",""[First Name]"",""[Last_Name]"",""[Age]"",""[Amount]""],
	""Output"":{{""First Name"":""INSERTED"",""Last_Name"":""DELETED"",""ID"":""INSERTED""}},
	""From"":""[dbo].[NonCustomers]"",
	""Having"":""MAX([Age]) \u003E 40"",
	""OrderBy"":[{{""Descending"":""First Name""}},{{""Ascending"":""Last_Name""}}],
	""Parameters"":{{""Param1"":333.66,""Param 2"":""{date:O}"",""Param_3"":""String Value"",""Param4"":""{guid:D}""}},
	""Select"":{{""ID"":""ID"",""First Name"":""TRIM([First Name])"",""LastName"":""UPPER([LastName])"",""Age"":""40"",""Amount"":""Amount""}},
	""Where"":""[First Name] = N\u0027Sarah\u0027 AND [Last_Name] = N\u0027Marshal\u0027""
}}").Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\t", string.Empty);

			Assert.Equal(expected, JsonSerializer.Serialize(expectedRequest));

			var request = JsonSerializer.Deserialize<InsertRequest>(expected);

			Assert.Equal(expectedRequest.From, request.From);
			Assert.Equal(expectedRequest.Having, request.Having);
			Assert.Equal(expectedRequest.Into, request.Into);
			Assert.Equal(expectedRequest.Insert, request.Insert);
			Assert.Equal(expectedRequest.OrderBy, request.OrderBy);
			Assert.Equal(expectedRequest.Output, request.Output);
			Assert.Equal(expectedRequest.Parameters, request.Parameters);
			Assert.Equal(expectedRequest.Select, request.Select);
			Assert.Equal(expectedRequest.Where, request.Where);
		}

		[Fact]
		public void ToJSON_SelectRequest()
		{
			var date = new DateTime(637621814434623833L);
			var guid = Guid.NewGuid();

			var expectedRequest = new SelectRequest
			{
				From = "[dbo].[NonCustomers]",
				Having = "MAX([Age]) > 40",
				OrderBy = new Dictionary<string, Sort> { { "First Name", Sort.Descending }, { "Last_Name", Sort.Ascending } },
				Parameters = new Dictionary<string, object> { { "Param1", 333.66M }, { "Param 2", date }, { "Param_3", "String Value" }, { "Param4", guid } },
				Select = new Dictionary<string, string>(5) { { "ID", "ID" }, { "First Name", "TRIM([First Name])" }, { "LastName", "UPPER([LastName])" }, { "Age", "40" }, { "Amount", "Amount" } },
				Where = "[First Name] = N'Sarah' AND [Last_Name] = N'Marshal'"
			};

			var expected = Invariant(@$"
{{
	""From"":""[dbo].[NonCustomers]"",
	""Having"":""MAX([Age]) \u003E 40"",
	""OrderBy"":[{{""Descending"":""First Name""}},{{""Ascending"":""Last_Name""}}],
	""Parameters"":{{""Param1"":333.66,""Param 2"":""{date:O}"",""Param_3"":""String Value"",""Param4"":""{guid:D}""}},
	""Select"":{{""ID"":""ID"",""First Name"":""TRIM([First Name])"",""LastName"":""UPPER([LastName])"",""Age"":""40"",""Amount"":""Amount""}},
	""Where"":""[First Name] = N\u0027Sarah\u0027 AND [Last_Name] = N\u0027Marshal\u0027""
}}").Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\t", string.Empty);

			Assert.Equal(expected, JsonSerializer.Serialize(expectedRequest));

			var request = JsonSerializer.Deserialize<SelectRequest>(expected);

			Assert.Equal(expectedRequest.From, request.From);
			Assert.Equal(expectedRequest.Having, request.Having);
			Assert.Equal(expectedRequest.OrderBy, request.OrderBy);
			Assert.Equal(expectedRequest.Parameters, request.Parameters);
			Assert.Equal(expectedRequest.Select, request.Select);
			Assert.Equal(expectedRequest.Where, request.Where);
		}

		[Fact]
		public void ToJSON_UpdateRequest()
		{
			var date = new DateTime(637621814434623833L);
			var guid = Guid.NewGuid();

			var expectedRequest = new UpdateRequest
			{
				Table = "[dbo].[NonCustomers]",
				Set = new Dictionary<string, object> { { "ID", 123456 }, { "First Name", "Sarah" }, { "Last_Name", "Marshal" }, { "Account", guid } },
				Output = new Dictionary<string, string> { { "First Name", "INSERTED" }, { "Last_Name", "DELETED" }, { "ID", "INSERTED" } },
				Where = "[First Name] = N'Sarah' AND [Last_Name] = N'Marshal'",
				Parameters = new Dictionary<string, object> { { "Param1", 44 }, { "Param 2", date }, { "Param_3", "String Value" }, { "Param4", guid } },
			};

			var expected = Invariant(@$"
{{
	""Output"":{{""First Name"":""INSERTED"",""Last_Name"":""DELETED"",""ID"":""INSERTED""}},
	""Parameters"":{{""Param1"":44,""Param 2"":""{date:O}"",""Param_3"":""String Value"",""Param4"":""{guid:D}""}},
	""Table"":""[dbo].[NonCustomers]"",
	""Set"":{{""ID"":123456,""First Name"":""Sarah"",""Last_Name"":""Marshal"",""Account"":""{guid:D}""}},
	""Where"":""[First Name] = N\u0027Sarah\u0027 AND [Last_Name] = N\u0027Marshal\u0027""
}}").Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\t", string.Empty);

			Assert.Equal(expected, JsonSerializer.Serialize(expectedRequest));

			var request = JsonSerializer.Deserialize<UpdateRequest>(expected);

			Assert.Equal(expectedRequest.Table, request.Table);
			Assert.Equal(expectedRequest.Set, request.Set);
			Assert.Equal(expectedRequest.Output, request.Output);
			Assert.Equal(expectedRequest.Where, request.Where);
			Assert.Equal(expectedRequest.Parameters, request.Parameters);
		}
	}
}
