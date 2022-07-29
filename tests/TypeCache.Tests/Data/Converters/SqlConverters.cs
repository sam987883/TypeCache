// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Text.Json;
using TypeCache.Attributes;
using TypeCache.Data.Domain;
using Xunit;
using static System.FormattableString;

namespace TypeCache.Tests.Data.Converters;

public class SqlConverters
{
	public class Person
	{
		[Name("First Name")]
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public int Age { get; set; }
	}

	[Fact]
	public void ToJSON_CountRequest()
	{
		var date = new DateTime(637621814434623833L);
		var guid = Guid.NewGuid();

		var expectedRequest = new CountCommand
		{
			DataSource = "LocalInstance",
			Table = "[dbo].[NonCustomers]",
			InputParameters = new Dictionary<string, object> { { "Param1", 333.66M }, { "Param 2", date }, { "Param_3", "String Value" }, { "Param4", guid } },
			Where = "[First Name] = N'Sarah' AND [Last_Name] = N'Marshal'"
		};

		var expected = Invariant(@$"
{{
	""{nameof(CountCommand.Distinct)}"":"""",
	""{nameof(CountCommand.Table)}"":""[dbo].[NonCustomers]"",
	""{nameof(CountCommand.TableHints)}"":""WITH(NOLOCK)"",
	""{nameof(CountCommand.Where)}"":""[First Name] = N\u0027Sarah\u0027 AND [Last_Name] = N\u0027Marshal\u0027"",
	""{nameof(CountCommand.DataSource)}"":""LocalInstance"",
	""{nameof(CountCommand.InputParameters)}"":{{""Param1"":333.66,""Param 2"":""{date:O}"",""Param_3"":""String Value"",""Param4"":""{guid:D}""}},
	""{nameof(CountCommand.OutputParameters)}"":null
}}").Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\t", string.Empty);

		Assert.Equal(expected, JsonSerializer.Serialize(expectedRequest));

		var request = JsonSerializer.Deserialize<CountCommand>(expected);

		Assert.Equal(expectedRequest.DataSource, request.DataSource);
		Assert.Equal(expectedRequest.Table, request.Table);
		Assert.Equal(expectedRequest.InputParameters, request.InputParameters);
		Assert.Equal(expectedRequest.Where, request.Where);
	}

	[Fact]
	public void ToJSON_DeleteDataRequest()
	{
		var date = new DateTime(637621814434623833L);
		var guid = Guid.NewGuid();
		var ids = new[] { 6, 5, 4, 3, 2, 1 };

		var expectedRequest = new DeleteDataCommand<int>
		{
			Table = "Customers",
			Input = ids,
			Output = new[] { "INSERTED.[First Name]", "DELETED.[Last_Name]", "INSERTED.ID" }
		};

		var expected = Invariant(@$"
{{
	""{nameof(DeleteDataCommand<int>.Input)}"":[6,5,4,3,2,1],
	""{nameof(DeleteDataCommand<int>.Output)}"":[""INSERTED.[First Name]"",""DELETED.[Last_Name]"",""INSERTED.ID""],
	""{nameof(DeleteDataCommand<int>.Table)}"":""Customers"",
	""{nameof(DeleteDataCommand<int>.DataSource)}"":""Default"",
	""{nameof(DeleteDataCommand<int>.InputParameters)}"":null,
	""{nameof(DeleteDataCommand<int>.OutputParameters)}"":null
}}").Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\t", string.Empty);

		Assert.Equal(expected, JsonSerializer.Serialize(expectedRequest));

		var request = JsonSerializer.Deserialize<DeleteDataCommand<int>>(expected);

		Assert.Equal(expectedRequest.DataSource, request.DataSource);
		Assert.Equal(expectedRequest.Table, request.Table);
		Assert.Equal(ids, request.Input);
		Assert.Equal(expectedRequest.Output, request.Output);
	}

	[Fact]
	public void ToJSON_DeleteRequest()
	{
		var date = new DateTime(637621814434623833L);
		var guid = Guid.NewGuid();

		var expectedRequest = new DeleteCommand
		{
			DataSource = "LocalInstance",
			Table = "Customers",
			Output = new[] { "INSERTED.[First Name]", "DELETED.[Last_Name]", "INSERTED.ID" },
			Where = "[First Name] = N'Sarah' AND [Last_Name] = N'Marshal'",
			InputParameters = new Dictionary<string, object> { { "Param1", false }, { "Param 2", date }, { "Param_3", "String Value" }, { "Param4", guid } }
		};

		var expected = Invariant(@$"
{{
	""{nameof(DeleteCommand.Output)}"":[""INSERTED.[First Name]"",""DELETED.[Last_Name]"",""INSERTED.ID""],
	""{nameof(DeleteCommand.Table)}"":""Customers"",
	""{nameof(DeleteCommand.Where)}"":""[First Name] = N\u0027Sarah\u0027 AND [Last_Name] = N\u0027Marshal\u0027"",
	""{nameof(DeleteCommand.DataSource)}"":""LocalInstance"",
	""{nameof(DeleteCommand.InputParameters)}"":{{""Param1"":false,""Param 2"":""{date:O}"",""Param_3"":""String Value"",""Param4"":""{guid:D}""}},
	""{nameof(DeleteCommand.OutputParameters)}"":null
}}").Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\t", string.Empty);

		Assert.Equal(expected, JsonSerializer.Serialize(expectedRequest));

		var request = JsonSerializer.Deserialize<DeleteCommand>(expected);

		Assert.Equal(expectedRequest.DataSource, request.DataSource);
		Assert.Equal(expectedRequest.Table, request.Table);
		Assert.Equal(expectedRequest.Output, request.Output);
		Assert.Equal(expectedRequest.Where, request.Where);
		Assert.Equal(expectedRequest.InputParameters, request.InputParameters);
	}

	[Fact]
	public void ToJSON_InsertDataRequest()
	{
		var date = new DateTime(637621814434623833L);
		var guid = Guid.NewGuid();

		var expectedRequest = new InsertDataCommand<Person>
		{
			Columns = new[] { "First Name", "Last_Name", "ID" },
			Input = new[]
			{
				new Person { FirstName = "FirstName1", LastName = "LastName1", Age = 31 },
				new Person { FirstName = "FirstName2", LastName = "LastName2", Age = 41 },
				new Person { FirstName = "FirstName3", LastName = "LastName3", Age = 22 }
			},
			Table = "Customers",
			Output = new[] { "INSERTED.[First Name]", "DELETED.[Last_Name]", "INSERTED.ID" }
		};

		var expected = Invariant(@$"
{{
	""{nameof(InsertDataCommand<Person>.Columns)}"":[""First Name"",""Last_Name"",""ID""],
	""{nameof(InsertDataCommand<Person>.Input)}"":[{{""FirstName"":""FirstName1"",""LastName"":""LastName1"",""Age"":31}},{{""FirstName"":""FirstName2"",""LastName"":""LastName2"",""Age"":41}},{{""FirstName"":""FirstName3"",""LastName"":""LastName3"",""Age"":22}}],
	""{nameof(InsertDataCommand<Person>.Output)}"":[""INSERTED.[First Name]"",""DELETED.[Last_Name]"",""INSERTED.ID""],
	""{nameof(InsertDataCommand<Person>.Table)}"":""Customers"",
	""{nameof(InsertDataCommand<Person>.DataSource)}"":""Default"",
	""{nameof(InsertDataCommand<Person>.InputParameters)}"":null,
	""{nameof(InsertDataCommand<Person>.OutputParameters)}"":null
}}").Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\t", string.Empty);

		Assert.Equal(expected, JsonSerializer.Serialize(expectedRequest));

		var request = JsonSerializer.Deserialize<InsertDataCommand<Person>>(expected);

		Assert.Equal(expectedRequest.Columns, request.Columns);
		Assert.Equal(expectedRequest.DataSource, request.DataSource);
		Assert.Equal(JsonSerializer.Serialize(expectedRequest.Input), JsonSerializer.Serialize(request.Input), StringComparer.Ordinal);
		Assert.Equal(expectedRequest.Table, request.Table);
		Assert.Equal(expectedRequest.Output, request.Output);
	}

	[Fact]
	public void ToJSON_InsertRequest()
	{
		var date = new DateTime(637621814434623833L);
		var guid = Guid.NewGuid();

		var expectedRequest = new InsertCommand
		{
			DataSource = "LocalInstance",
			Table = "Customers",
			Columns = new[] { "[ID]", "[First Name]", "[Last_Name]", "[Age]", "[Amount]" },
			Output = new[] { "INSERTED.[First Name]", "DELETED.[Last_Name]", "INSERTED.ID" },
			From = "[dbo].[NonCustomers]",
			Having = "MAX([Age]) > 40",
			OrderBy = new[] { "[First Name] ASC", "Last_Name DESC" },
			InputParameters = new Dictionary<string, object> { { "Param1", 333.66M }, { "Param 2", date }, { "Param_3", "String Value" }, { "Param4", guid } },
			Select = new[] { "ID", "TRIM([First Name]) AS [First Name]", "UPPER([LastName]) AS LastName", "40 Age", "Amount AS Amount" },
			TableHints = "WITH(NOLOCK)",
			Where = "[First Name] = N'Sarah' AND [Last_Name] = N'Marshal'"
		};

		var expected = Invariant(@$"
{{
	""{nameof(InsertCommand.Columns)}"":[""[ID]"",""[First Name]"",""[Last_Name]"",""[Age]"",""[Amount]""],
	""{nameof(InsertCommand.Output)}"":[""INSERTED.[First Name]"",""DELETED.[Last_Name]"",""INSERTED.ID""],
	""{nameof(InsertCommand.Table)}"":""Customers"",
	""{nameof(InsertCommand.Distinct)}"":false,
	""{nameof(InsertCommand.From)}"":""[dbo].[NonCustomers]"",
	""{nameof(InsertCommand.GroupBy)}"":null,
	""{nameof(InsertCommand.Having)}"":""MAX([Age]) \u003E 40"",
	""{nameof(InsertCommand.OrderBy)}"":[""[First Name] ASC"",""Last_Name DESC""],
	""{nameof(InsertCommand.Pager)}"":null,
	""{nameof(InsertCommand.Percent)}"":false,
	""{nameof(InsertCommand.Select)}"":[""ID"",""TRIM([First Name]) AS [First Name]"",""UPPER([LastName]) AS LastName"",""40 Age"",""Amount AS Amount""],
	""{nameof(InsertCommand.TableHints)}"":""WITH(NOLOCK)"",
	""{nameof(InsertCommand.Top)}"":0,
	""{nameof(InsertCommand.Where)}"":""[First Name] = N\u0027Sarah\u0027 AND [Last_Name] = N\u0027Marshal\u0027"",
	""{nameof(InsertCommand.WithTies)}"":false,
	""{nameof(InsertCommand.DataSource)}"":""LocalInstance"",
	""{nameof(InsertCommand.InputParameters)}"":{{""Param1"":333.66,""Param 2"":""{date:O}"",""Param_3"":""String Value"",""Param4"":""{guid:D}""}},
	""{nameof(InsertCommand.OutputParameters)}"":null
}}").Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\t", string.Empty);

		Assert.Equal(expected, JsonSerializer.Serialize(expectedRequest));

		var request = JsonSerializer.Deserialize<InsertCommand>(expected);

		Assert.Equal(expectedRequest.DataSource, request.DataSource);
		Assert.Equal(expectedRequest.From, request.From);
		Assert.Equal(expectedRequest.Having, request.Having);
		Assert.Equal(expectedRequest.Table, request.Table);
		Assert.Equal(expectedRequest.Columns, request.Columns);
		Assert.Equal(expectedRequest.OrderBy, request.OrderBy);
		Assert.Equal(expectedRequest.Output, request.Output);
		Assert.Equal(expectedRequest.InputParameters, request.InputParameters);
		Assert.Equal(expectedRequest.Select, request.Select);
		Assert.Equal(expectedRequest.Where, request.Where);
	}

	[Fact]
	public void ToJSON_SelectRequest()
	{
		var date = new DateTime(637621814434623833L);
		var guid = Guid.NewGuid();

		var expectedRequest = new SelectCommand
		{
			DataSource = "LocalInstance",
			From = "[dbo].[NonCustomers]",
			Having = "MAX([Age]) > 40",
			OrderBy = new[] { "[First Name] ASC", "Last_Name DESC" },
			InputParameters = new Dictionary<string, object> { { "Param1", 333.66M }, { "Param 2", date }, { "Param_3", "String Value" }, { "Param4", guid } },
			Pager = new() { After = 0, First = 100 },
			Select = new[] { "ID", "TRIM([First Name]) AS [First Name]", "UPPER([LastName]) AS LastName", "40 Age", "Amount AS Amount" },
			TableHints = "WITH(NOLOCK)",
			Where = "[First Name] = N'Sarah' AND [Last_Name] = N'Marshal'"
		};

		var expected = Invariant(@$"
{{
	""{nameof(SelectCommand.Distinct)}"":false,
	""{nameof(SelectCommand.From)}"":""[dbo].[NonCustomers]"",
	""{nameof(SelectCommand.GroupBy)}"":null,
	""{nameof(SelectCommand.Having)}"":""MAX([Age]) \u003E 40"",
	""{nameof(SelectCommand.OrderBy)}"":[""[First Name] ASC"",""Last_Name DESC""],
	""{nameof(SelectCommand.Pager)}"":{{""First"":100,""After"":0}},
	""{nameof(SelectCommand.Percent)}"":false,
	""{nameof(SelectCommand.Select)}"":[""ID"",""TRIM([First Name]) AS [First Name]"",""UPPER([LastName]) AS LastName"",""40 Age"",""Amount AS Amount""],
	""{nameof(SelectCommand.TableHints)}"":""WITH(NOLOCK)"",
	""{nameof(SelectCommand.Top)}"":0,
	""{nameof(SelectCommand.Where)}"":""[First Name] = N\u0027Sarah\u0027 AND [Last_Name] = N\u0027Marshal\u0027"",
	""{nameof(SelectCommand.WithTies)}"":false,
	""{nameof(SelectCommand.DataSource)}"":""LocalInstance"",
	""{nameof(SelectCommand.InputParameters)}"":{{""Param1"":333.66,""Param 2"":""{date:O}"",""Param_3"":""String Value"",""Param4"":""{guid:D}""}},
	""{nameof(SelectCommand.OutputParameters)}"":null
}}").Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\t", string.Empty);

		Assert.Equal(expected, JsonSerializer.Serialize(expectedRequest));

		var request = JsonSerializer.Deserialize<SelectCommand>(expected);

		Assert.Equal(expectedRequest.DataSource, request.DataSource);
		Assert.Equal(expectedRequest.From, request.From);
		Assert.Equal(expectedRequest.Having, request.Having);
		Assert.Equal(expectedRequest.OrderBy, request.OrderBy);
		Assert.Equal(expectedRequest.InputParameters, request.InputParameters);
		Assert.Equal(expectedRequest.Select, request.Select);
		Assert.Equal(expectedRequest.Where, request.Where);
	}

	[Fact]
	public void ToJSON_UpdateDataRequest()
	{
		var date = new DateTime(637621814434623833L);
		var guid = Guid.NewGuid();

		var expectedRequest = new UpdateDataCommand<Person>
		{
			Columns = new[] { "ID1", "ID2", "First Name", "Last_Name", "ID" },
			DataSource = "LOCAL",
			Input = new[]
			{
				new Person { FirstName = "FirstName1", LastName = "LastName1", Age = 31 },
				new Person { FirstName = "FirstName2", LastName = "LastName2", Age = 41 },
				new Person { FirstName = "FirstName3", LastName = "LastName3", Age = 22 }
			},
			On = new[] { "ID" },
			Output = new[] { "INSERTED.[First Name]", "DELETED.[Last_Name]", "INSERTED.ID" },
			Table = "Customers",
			TableHints = "WITH(UPDLOCK)"
		};

		var expected = Invariant(@$"
{{
	""{nameof(UpdateDataCommand<Person>.Columns)}"":[""ID1"",""ID2"",""First Name"",""Last_Name"",""ID""],
	""{nameof(UpdateDataCommand<Person>.Input)}"":[{{""FirstName"":""FirstName1"",""LastName"":""LastName1"",""Age"":31}},{{""FirstName"":""FirstName2"",""LastName"":""LastName2"",""Age"":41}},{{""FirstName"":""FirstName3"",""LastName"":""LastName3"",""Age"":22}}],
	""{nameof(UpdateDataCommand<Person>.On)}"":[""ID""],
	""{nameof(UpdateDataCommand<Person>.Output)}"":[""INSERTED.[First Name]"",""DELETED.[Last_Name]"",""INSERTED.ID""],
	""{nameof(UpdateDataCommand<Person>.Table)}"":""Customers"",
	""{nameof(UpdateDataCommand<Person>.TableHints)}"":""WITH(UPDLOCK)"",
	""{nameof(UpdateDataCommand<Person>.DataSource)}"":""LOCAL"",
	""{nameof(UpdateDataCommand<Person>.InputParameters)}"":null,
	""{nameof(UpdateDataCommand<Person>.OutputParameters)}"":null
}}").Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\t", string.Empty);

		Assert.Equal(expected, JsonSerializer.Serialize(expectedRequest));

		var request = JsonSerializer.Deserialize<UpdateDataCommand<Person>>(expected);

		Assert.Equal(expectedRequest.DataSource, request.DataSource);
		Assert.Equal(JsonSerializer.Serialize(expectedRequest.Input), JsonSerializer.Serialize(request.Input), StringComparer.Ordinal);
		Assert.Equal(expectedRequest.Output, request.Output);
		Assert.Equal(expectedRequest.Columns, request.Columns);
		Assert.Equal(expectedRequest.Table, request.Table);
	}

	[Fact]
	public void ToJSON_UpdateRequest()
	{
		var date = new DateTime(637621814434623833L);
		var guid = Guid.NewGuid();

		var expectedRequest = new UpdateCommand
		{
			DataSource = "LocalInstance",
			Table = "[dbo].[NonCustomers]",
			Columns = new[] { "ID = 123456", "[First Name] = N'Sarah'", "Last_Name = N'Marshal'", "Account = @Param1" },
			Output = new[] { "INSERTED.[First Name]", "DELETED.[Last_Name]", "INSERTED.ID" },
			TableHints = "WITH(UPDLOCK)",
			Where = "[First Name] = N'Sarah' AND [Last_Name] = N'Marshal'",
			InputParameters = new Dictionary<string, object> { { "Param1", 44 }, { "Param 2", date }, { "Param_3", "String Value" }, { "Param4", guid } }
		};

		var expected = Invariant(@$"
{{
	""{nameof(UpdateCommand.Columns)}"":[""ID = 123456"",""[First Name] = N\u0027Sarah\u0027"",""Last_Name = N\u0027Marshal\u0027"",""Account = @Param1""],
	""{nameof(UpdateCommand.Output)}"":[""INSERTED.[First Name]"",""DELETED.[Last_Name]"",""INSERTED.ID""],
	""{nameof(UpdateCommand.Table)}"":""[dbo].[NonCustomers]"",
	""{nameof(UpdateCommand.TableHints)}"":""WITH(UPDLOCK)"",
	""{nameof(UpdateCommand.Where)}"":""[First Name] = N\u0027Sarah\u0027 AND [Last_Name] = N\u0027Marshal\u0027"",
	""{nameof(UpdateCommand.DataSource)}"":""LocalInstance"",
	""{nameof(UpdateCommand.InputParameters)}"":{{""Param1"":44,""Param 2"":""{date:O}"",""Param_3"":""String Value"",""Param4"":""{guid:D}""}},
	""{nameof(UpdateCommand.OutputParameters)}"":null
}}").Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\t", string.Empty);

		Assert.Equal(expected, JsonSerializer.Serialize(expectedRequest));

		var request = JsonSerializer.Deserialize<UpdateCommand>(expected);

		Assert.Equal(expectedRequest.DataSource, request.DataSource);
		Assert.Equal(expectedRequest.Table, request.Table);
		Assert.Equal(expectedRequest.Columns, request.Columns);
		Assert.Equal(expectedRequest.Output, request.Output);
		Assert.Equal(expectedRequest.Where, request.Where);
		Assert.Equal(expectedRequest.InputParameters, request.InputParameters);
	}
}
