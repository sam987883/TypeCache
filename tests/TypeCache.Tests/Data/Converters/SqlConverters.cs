// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Text.Json;
using TypeCache.Data.Requests;
using Xunit;
using static System.FormattableString;

namespace TypeCache.Tests.Data.Converters;

public class SqlConverters
{
	[Fact]
	public void ToJSON_CountRequest()
	{
		var date = new DateTime(637621814434623833L);
		var guid = Guid.NewGuid();

		var expectedRequest = new CountRequest
		{
			DataSource = "LocalInstance",
			From = "[dbo].[NonCustomers]",
			Parameters = new Dictionary<string, object> { { "Param1", 333.66M }, { "Param 2", date }, { "Param_3", "String Value" }, { "Param4", guid } },
			Where = "[First Name] = N'Sarah' AND [Last_Name] = N'Marshal'"
		};

		var expected = Invariant(@$"
{{
	""DataSource"":""LocalInstance"",
	""Distinct"":"""",
	""From"":""[dbo].[NonCustomers]"",
	""Parameters"":{{""Param1"":333.66,""Param 2"":""{date:O}"",""Param_3"":""String Value"",""Param4"":""{guid:D}""}},
	""TableHints"":""WITH(NOLOCK)"",
	""Where"":""[First Name] = N\u0027Sarah\u0027 AND [Last_Name] = N\u0027Marshal\u0027""
}}").Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\t", string.Empty);

		Assert.Equal(expected, JsonSerializer.Serialize(expectedRequest));

		var request = JsonSerializer.Deserialize<CountRequest>(expected);

		Assert.Equal(expectedRequest.DataSource, request.DataSource);
		Assert.Equal(expectedRequest.From, request.From);
		Assert.Equal(expectedRequest.Parameters, request.Parameters);
		Assert.Equal(expectedRequest.Where, request.Where);
	}

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
			Output = new[] { "INSERTED.[First Name]", "DELETED.[Last_Name]", "INSERTED.ID" }
		};

		var expected = Invariant(@$"
{{
	""DataSource"":""Default"",
	""From"":""Customers"",
	""Input"":{{""Columns"":[""ID1"",""ID2""],""Count"":0,""Rows"":[[1,2],[1,3],[2,1]]}},
	""Output"":[""INSERTED.[First Name]"",""DELETED.[Last_Name]"",""INSERTED.ID""]
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
			Output = new[] { "INSERTED.[First Name]", "DELETED.[Last_Name]", "INSERTED.ID" },
			Where = "[First Name] = N'Sarah' AND [Last_Name] = N'Marshal'",
			Parameters = new Dictionary<string, object> { { "Param1", false }, { "Param 2", date }, { "Param_3", "String Value" }, { "Param4", guid } }
		};

		var expected = Invariant(@$"
{{
	""DataSource"":""LocalInstance"",
	""From"":""Customers"",
	""Output"":[""INSERTED.[First Name]"",""DELETED.[Last_Name]"",""INSERTED.ID""],
	""Parameters"":{{""Param1"":false,""Param 2"":""{date:O}"",""Param_3"":""String Value"",""Param4"":""{guid:D}""}},
	""Where"":""[First Name] = N\u0027Sarah\u0027 AND [Last_Name] = N\u0027Marshal\u0027""
}}").Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\t", string.Empty);

		Assert.Equal(expected, JsonSerializer.Serialize(expectedRequest));

		var request = JsonSerializer.Deserialize<DeleteRequest>(expected);

		Assert.Equal(expectedRequest.DataSource, request.DataSource);
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
			Output = new[] { "INSERTED.[First Name]", "DELETED.[Last_Name]", "INSERTED.ID" }
		};

		var expected = Invariant(@$"
{{
	""DataSource"":""Default"",
	""Input"":{{""Columns"":[""First Name"",""Last_Name"",""ID""],""Count"":0,""Rows"":[[""FirstName1"",""LastName1"",1],[""FirstName2"",""LastName2"",2],[""FirstName3"",""LastName3"",3]]}},
	""Into"":""Customers"",
	""Output"":[""INSERTED.[First Name]"",""DELETED.[Last_Name]"",""INSERTED.ID""]
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
			Output = new[] { "INSERTED.[First Name]", "DELETED.[Last_Name]", "INSERTED.ID" },
			From = "[dbo].[NonCustomers]",
			Having = "MAX([Age]) > 40",
			OrderBy = new[] { "[First Name] ASC", "Last_Name DESC" },
			Parameters = new Dictionary<string, object> { { "Param1", 333.66M }, { "Param 2", date }, { "Param_3", "String Value" }, { "Param4", guid } },
			Select = new[] { "ID", "TRIM([First Name]) AS [First Name]", "UPPER([LastName]) AS LastName", "40 Age", "Amount AS Amount" },
			TableHints = "WITH(NOLOCK)",
			Where = "[First Name] = N'Sarah' AND [Last_Name] = N'Marshal'"
		};

		var expected = Invariant(@$"
{{
	""Insert"":[""[ID]"",""[First Name]"",""[Last_Name]"",""[Age]"",""[Amount]""],
	""Into"":""Customers"",
	""Output"":[""INSERTED.[First Name]"",""DELETED.[Last_Name]"",""INSERTED.ID""],
	""DataSource"":""LocalInstance"",
	""Distinct"":false,
	""From"":""[dbo].[NonCustomers]"",
	""GroupBy"":null,
	""Having"":""MAX([Age]) \u003E 40"",
	""OrderBy"":[""[First Name] ASC"",""Last_Name DESC""],
	""Pager"":null,
	""Parameters"":{{""Param1"":333.66,""Param 2"":""{date:O}"",""Param_3"":""String Value"",""Param4"":""{guid:D}""}},
	""Percent"":false,
	""Select"":[""ID"",""TRIM([First Name]) AS [First Name]"",""UPPER([LastName]) AS LastName"",""40 Age"",""Amount AS Amount""],
	""TableHints"":""WITH(NOLOCK)"",
	""Top"":0,
	""Where"":""[First Name] = N\u0027Sarah\u0027 AND [Last_Name] = N\u0027Marshal\u0027"",
	""WithTies"":false
}}").Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\t", string.Empty);

		Assert.Equal(expected, JsonSerializer.Serialize(expectedRequest));

		var request = JsonSerializer.Deserialize<InsertRequest>(expected);

		Assert.Equal(expectedRequest.DataSource, request.DataSource);
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
			OrderBy = new[] { "[First Name] ASC", "Last_Name DESC" },
			Parameters = new Dictionary<string, object> { { "Param1", 333.66M }, { "Param 2", date }, { "Param_3", "String Value" }, { "Param4", guid } },
			Pager = new() { After = 0, First = 100 },
			Select = new[] { "ID", "TRIM([First Name]) AS [First Name]", "UPPER([LastName]) AS LastName", "40 Age", "Amount AS Amount" },
			TableHints = "WITH(NOLOCK)",
			Where = "[First Name] = N'Sarah' AND [Last_Name] = N'Marshal'"
		};

		var expected = Invariant(@$"
{{
	""DataSource"":""LocalInstance"",
	""Distinct"":false,
	""From"":""[dbo].[NonCustomers]"",
	""GroupBy"":null,
	""Having"":""MAX([Age]) \u003E 40"",
	""OrderBy"":[""[First Name] ASC"",""Last_Name DESC""],
	""Pager"":{{""First"":100,""After"":0}},
	""Parameters"":{{""Param1"":333.66,""Param 2"":""{date:O}"",""Param_3"":""String Value"",""Param4"":""{guid:D}""}},
	""Percent"":false,
	""Select"":[""ID"",""TRIM([First Name]) AS [First Name]"",""UPPER([LastName]) AS LastName"",""40 Age"",""Amount AS Amount""],
	""TableHints"":""WITH(NOLOCK)"",
	""Top"":0,
	""Where"":""[First Name] = N\u0027Sarah\u0027 AND [Last_Name] = N\u0027Marshal\u0027"",
	""WithTies"":false
}}").Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\t", string.Empty);

		Assert.Equal(expected, JsonSerializer.Serialize(expectedRequest));

		var request = JsonSerializer.Deserialize<SelectRequest>(expected);

		Assert.Equal(expectedRequest.DataSource, request.DataSource);
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
			On = new[] { "ID" },
			Output = new[] { "INSERTED.[First Name]", "DELETED.[Last_Name]", "INSERTED.ID" },
			Table = "Customers",
			TableHints = "WITH(UPDLOCK)"
		};

		var expected = Invariant(@$"
{{
	""DataSource"":""LOCAL"",
	""Input"":{{""Columns"":[""ID1"",""ID2"",""First Name"",""Last_Name"",""ID""],""Count"":0,""Rows"":[[1,2,""FirstName1"",""LastName1"",1],[1,3,""FirstName2"",""LastName2"",2],[2,1,""FirstName3"",""LastName3"",3]]}},
	""On"":[""ID""],
	""Output"":[""INSERTED.[First Name]"",""DELETED.[Last_Name]"",""INSERTED.ID""],
	""Table"":""Customers"",
	""TableHints"":""WITH(UPDLOCK)""
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
			Set = new[] { "ID = 123456", "[First Name] = N'Sarah'", "Last_Name = N'Marshal'", "Account = @Param1" },
			Output = new[] { "INSERTED.[First Name]", "DELETED.[Last_Name]", "INSERTED.ID" },
			TableHints = "WITH(UPDLOCK)",
			Where = "[First Name] = N'Sarah' AND [Last_Name] = N'Marshal'",
			Parameters = new Dictionary<string, object> { { "Param1", 44 }, { "Param 2", date }, { "Param_3", "String Value" }, { "Param4", guid } }
		};

		var expected = Invariant(@$"
{{
	""DataSource"":""LocalInstance"",
	""Output"":[""INSERTED.[First Name]"",""DELETED.[Last_Name]"",""INSERTED.ID""],
	""Parameters"":{{""Param1"":44,""Param 2"":""{date:O}"",""Param_3"":""String Value"",""Param4"":""{guid:D}""}},
	""Set"":[""ID = 123456"",""[First Name] = N\u0027Sarah\u0027"",""Last_Name = N\u0027Marshal\u0027"",""Account = @Param1""],
	""Table"":""[dbo].[NonCustomers]"",
	""TableHints"":""WITH(UPDLOCK)"",
	""Where"":""[First Name] = N\u0027Sarah\u0027 AND [Last_Name] = N\u0027Marshal\u0027""
}}").Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\t", string.Empty);

		Assert.Equal(expected, JsonSerializer.Serialize(expectedRequest));

		var request = JsonSerializer.Deserialize<UpdateRequest>(expected);

		Assert.Equal(expectedRequest.DataSource, request.DataSource);
		Assert.Equal(expectedRequest.Table, request.Table);
		Assert.Equal(expectedRequest.Set, request.Set);
		Assert.Equal(expectedRequest.Output, request.Output);
		Assert.Equal(expectedRequest.Where, request.Where);
		Assert.Equal(expectedRequest.Parameters, request.Parameters);
	}
}
