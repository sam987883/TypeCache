using System;
using System.Collections.Generic;
using System.Text.Json;
using TypeCache.Data;
using TypeCache.Data.Requests;
using Xunit;
using static System.FormattableString;

namespace TypeCache.Tests.Data.Converters
{
	public class SqlConverters
	{
		[Fact]
		public void ToJSON_DeleteDataRequest()
		{
			var date = new DateTime(637621814434623833L);
			var guid = Guid.NewGuid();

			var expectedRequest = new DeleteDataRequest
			{
				From = "Customers",
				Input = new()
				{
					Columns = new[] { "ID1", "ID2" },
					Rows = new object[][] { new object[] { 1, 2 }, new object[] { 1, 3 }, new object[] { 2, 1 } }
				},
				Output = new Dictionary<string, string> { { "First Name", "INSERTED" }, { "Last_Name", "DELETED" }, { "ID", "INSERTED" } }
			};

			var expected = Invariant(@$"
{{
	""DataSource"":""Default"",
	""From"":""Customers"",
	""Input"":{{""Columns"":[""ID1"",""ID2""],""Rows"":[[1,2],[1,3],[2,1]]}},
	""Output"":{{""First Name"":""INSERTED"",""Last_Name"":""DELETED"",""ID"":""INSERTED""}}
}}").Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\t", string.Empty);

			Assert.Equal(expected, JsonSerializer.Serialize(expectedRequest));

			var request = JsonSerializer.Deserialize<DeleteDataRequest>(expected);

			Assert.Equal(expectedRequest.DataSource, request.DataSource);
			Assert.Equal(expectedRequest.From, request.From);
			Assert.Equal(expectedRequest.Input.Columns, request.Input.Columns);
			Assert.Equal(expectedRequest.Input.Rows, request.Input.Rows);
			Assert.Equal(expectedRequest.Output, request.Output);
		}

		[Fact]
		public void ToJSON_DeleteRequest()
		{
			var date = new DateTime(637621814434623833L);
			var guid = Guid.NewGuid();

			var expectedRequest = new DeleteRequest
			{
				DataSource = "LocalInstance",
				From = "Customers",
				Output = new Dictionary<string, string> { { "First Name", "INSERTED" }, { "Last_Name", "DELETED" }, { "ID", "INSERTED" } },
				Where = "[First Name] = N'Sarah' AND [Last_Name] = N'Marshal'",
				Parameters = new Dictionary<string, object> { { "Param1", false }, { "Param 2", date }, { "Param_3", "String Value" }, { "Param4", guid } }
			};

			var expected = Invariant(@$"
{{
	""DataSource"":""LocalInstance"",
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
		public void ToJSON_InsertDataRequest()
		{
			var date = new DateTime(637621814434623833L);
			var guid = Guid.NewGuid();

			var expectedRequest = new InsertDataRequest
			{
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
				Into = "Customers",
				Output = new Dictionary<string, string> { { "First Name", "INSERTED" }, { "Last_Name", "DELETED" }, { "ID", "INSERTED" } }
			};

			var expected = Invariant(@$"
{{
	""DataSource"":""Default"",
	""Input"":{{""Columns"":[""First Name"",""Last_Name"",""ID""],""Rows"":[[""FirstName1"",""LastName1"",1],[""FirstName2"",""LastName2"",2],[""FirstName3"",""LastName3"",3]]}},
	""Into"":""Customers"",
	""Output"":{{""First Name"":""INSERTED"",""Last_Name"":""DELETED"",""ID"":""INSERTED""}}
}}").Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\t", string.Empty);

			Assert.Equal(expected, JsonSerializer.Serialize(expectedRequest));

			var request = JsonSerializer.Deserialize<InsertDataRequest>(expected);

			Assert.Equal(expectedRequest.DataSource, request.DataSource);
			Assert.Equal(expectedRequest.Input.Columns, request.Input.Columns);
			Assert.Equal(expectedRequest.Input.Rows, request.Input.Rows);
			Assert.Equal(expectedRequest.Into, request.Into);
			Assert.Equal(expectedRequest.Output, request.Output);
		}

		[Fact]
		public void ToJSON_InsertRequest()
		{
			var date = new DateTime(637621814434623833L);
			var guid = Guid.NewGuid();

			var expectedRequest = new InsertRequest
			{
				DataSource = "LocalInstance",
				Into = "Customers",
				Insert = new[] { "[ID]", "[First Name]", "[Last_Name]", "[Age]", "[Amount]" },
				Output = new Dictionary<string, string>(3) { { "First Name", "INSERTED" }, { "Last_Name", "DELETED" }, { "ID", "INSERTED" } },
				From = "[dbo].[NonCustomers]",
				Having = "MAX([Age]) > 40",
				OrderBy = new[] { ("First Name", Sort.Descending), ("Last_Name", Sort.Ascending) },
				Parameters = new Dictionary<string, object> { { "Param1", 333.66M }, { "Param 2", date }, { "Param_3", "String Value" }, { "Param4", guid } },
				Select = new Dictionary<string, string>(5) { { "ID", "ID" }, { "First Name", "TRIM([First Name])" }, { "LastName", "UPPER([LastName])" }, { "Age", "40" }, { "Amount", "Amount" } },
				Where = "[First Name] = N'Sarah' AND [Last_Name] = N'Marshal'"
			};

			var expected = Invariant(@$"
{{
	""Into"":""Customers"",
	""Insert"":[""[ID]"",""[First Name]"",""[Last_Name]"",""[Age]"",""[Amount]""],
	""Output"":{{""First Name"":""INSERTED"",""Last_Name"":""DELETED"",""ID"":""INSERTED""}},
	""DataSource"":""LocalInstance"",
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
				DataSource = "LocalInstance",
				From = "[dbo].[NonCustomers]",
				Having = "MAX([Age]) > 40",
				OrderBy = new[] { ("First Name", Sort.Descending), ("Last_Name", Sort.Ascending) },
				Parameters = new Dictionary<string, object> { { "Param1", 333.66M }, { "Param 2", date }, { "Param_3", "String Value" }, { "Param4", guid } },
				Select = new Dictionary<string, string>(5) { { "ID", "ID" }, { "First Name", "TRIM([First Name])" }, { "LastName", "UPPER([LastName])" }, { "Age", "40" }, { "Amount", "Amount" } },
				Where = "[First Name] = N'Sarah' AND [Last_Name] = N'Marshal'"
			};

			var expected = Invariant(@$"
{{
	""DataSource"":""LocalInstance"",
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
		public void ToJSON_UpdateDataRequest()
		{
			var date = new DateTime(637621814434623833L);
			var guid = Guid.NewGuid();

			var expectedRequest = new UpdateDataRequest
			{
				DataSource = "LOCAL",
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
				Output = new Dictionary<string, string> { { "First Name", "INSERTED" }, { "Last_Name", "DELETED" }, { "ID", "INSERTED" } },
				Table = "Customers",
			};

			var expected = Invariant(@$"
{{
	""DataSource"":""LOCAL"",
	""Input"":{{""Columns"":[""ID1"",""ID2"",""First Name"",""Last_Name"",""ID""],""Rows"":[[1,2,""FirstName1"",""LastName1"",1],[1,3,""FirstName2"",""LastName2"",2],[2,1,""FirstName3"",""LastName3"",3]]}},
	""Output"":{{""First Name"":""INSERTED"",""Last_Name"":""DELETED"",""ID"":""INSERTED""}},
	""Table"":""Customers""
}}").Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\t", string.Empty);

			Assert.Equal(expected, JsonSerializer.Serialize(expectedRequest));

			var request = JsonSerializer.Deserialize<UpdateDataRequest>(expected);

			Assert.Equal(expectedRequest.DataSource, request.DataSource);
			Assert.Equal(expectedRequest.Input.Columns, request.Input.Columns);
			Assert.Equal(expectedRequest.Input.Rows, request.Input.Rows);
			Assert.Equal(expectedRequest.Output, request.Output);
			Assert.Equal(expectedRequest.Table, request.Table);
		}

		[Fact]
		public void ToJSON_UpdateRequest()
		{
			var date = new DateTime(637621814434623833L);
			var guid = Guid.NewGuid();

			var expectedRequest = new UpdateRequest
			{
				DataSource = "LocalInstance",
				Table = "[dbo].[NonCustomers]",
				Set = new Dictionary<string, object> { { "ID", 123456 }, { "First Name", "Sarah" }, { "Last_Name", "Marshal" }, { "Account", guid } },
				Output = new Dictionary<string, string> { { "First Name", "INSERTED" }, { "Last_Name", "DELETED" }, { "ID", "INSERTED" } },
				Where = "[First Name] = N'Sarah' AND [Last_Name] = N'Marshal'",
				Parameters = new Dictionary<string, object> { { "Param1", 44 }, { "Param 2", date }, { "Param_3", "String Value" }, { "Param4", guid } },
			};

			var expected = Invariant(@$"
{{
	""DataSource"":""LocalInstance"",
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
