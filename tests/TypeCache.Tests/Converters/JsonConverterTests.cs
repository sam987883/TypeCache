﻿// Copyright (c) 2021 Samuel Abraham

using System;
using System.Data;
using System.Numerics;
using System.Text.Json;
using TypeCache.Converters;
using Xunit;

namespace TypeCache.Tests.Converters;

public class JsonConverterTests
{
	[Fact]
	public void BigIntegerJsonConverter()
	{
		var value = new BigInteger(999999999999999999999999M);

		var jsonOptions = new JsonSerializerOptions();
		jsonOptions.Converters.Add(new BigIntegerJsonConverter());

		var json = JsonSerializer.Serialize(value);
		Assert.NotEqual("999999999999999999999999", json);

		json = JsonSerializer.Serialize(value, jsonOptions);
		Assert.Equal("999999999999999999999999", json);

		var result = JsonSerializer.Deserialize<BigInteger>(json, jsonOptions);
		Assert.Equal(value, result);
	}

	[Fact]
	public void DataRowsJsonConverter()
	{
		var dataTable = new DataTable("Table1");
		dataTable.Columns.Add("Column1", typeof(bool));
		dataTable.Columns.Add("Column2", typeof(string));
		dataTable.Columns.Add("Column3", typeof(int));
		dataTable.Columns.Add("Column4", typeof(decimal));

		var dataRow1 = dataTable.NewRow();
		dataRow1["Column1"] = true;
		dataRow1["Column2"] = "AAAAAA";
		dataRow1["Column3"] = 24;
		dataRow1["Column4"] = 99999.99M;
		var dataRow2 = dataTable.NewRow();
		dataRow2["Column1"] = false;
		dataRow2["Column2"] = "BBBBBB";
		dataRow2["Column3"] = DBNull.Value;
		dataRow2["Column4"] = 0M;
		var dataRow3 = dataTable.NewRow();
		dataRow3["Column1"] = DBNull.Value;
		dataRow3["Column2"] = DBNull.Value;
		dataRow3["Column3"] = -24;
		dataRow3["Column4"] = -99999.99M;
		dataTable.Rows.Add(dataRow1);
		dataTable.Rows.Add(dataRow2);
		dataTable.Rows.Add(dataRow3);

		var jsonOptions = new JsonSerializerOptions();
		jsonOptions.Converters.Add(new DataRowJsonConverter());

		var json = JsonSerializer.Serialize(dataTable.Rows, jsonOptions);
		Assert.Equal("""
			[{"Column1":true,"Column2":"AAAAAA","Column3":24,"Column4":99999.99},{"Column1":false,"Column2":"BBBBBB","Column3":null,"Column4":0},{"Column1":null,"Column2":null,"Column3":-24,"Column4":-99999.99}]
			""", json);

		json = JsonSerializer.Serialize(null as DataRow, jsonOptions);
		Assert.Equal("null", json);
	}
}
