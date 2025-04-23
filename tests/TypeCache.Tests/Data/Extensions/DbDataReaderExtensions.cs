// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NSubstitute.Core;
using TypeCache.Data.Extensions;
using Xunit;

namespace TypeCache.Tests.Data.Extensions;

public class DbDataReaderExtensions
{
	class Tester
	{
		[JsonPropertyName("ID")]
		public long Id { get; set; }
		public string Name { get; set; }
		public DateTime? Date { get; set; }
	}

	private readonly DbDataReader _DataReader;
	private readonly DateTime _Now = DateTime.Now;
	private readonly DateTime _UtcNow = DateTime.UtcNow;

	public DbDataReaderExtensions()
	{
		this._DataReader = Substitute.For<DbDataReader>();
		this._DataReader.FieldCount.Returns(3);
		this._DataReader.GetName(0).Returns("ID");
		this._DataReader.GetName(1).Returns("Name");
		this._DataReader.GetName(2).Returns("Date");

		// Used by DataTable.Load(IDataReader)
		this._DataReader.GetFieldType(0).Returns(typeof(long));
		this._DataReader.GetFieldType(1).Returns(typeof(string));
		this._DataReader.GetFieldType(2).Returns(typeof(DateTime));
	}

	private void SetupReader()
	{
		this._DataReader.GetValues(Arg.Any<object[]>()).Returns(callInfo =>
		{
			var values = (object[])callInfo.Args()[0];
			values[0] = 1L;
			values[1] = "Bob Dole";
			values[2] = _Now;
			return 3;
		}, callInfo =>
		{
			var values = (object[])callInfo.Args()[0];
			values[0] = 2L;
			values[1] = "Bob Dole Robot";
			values[2] = _UtcNow;
			return 3;
		}, callInfo =>
		{
			var values = (object[])callInfo.Args()[0];
			values[0] = 3L;
			values[1] = "Bob Dole Clone";
			values[2] = null;
			return 3;
		});
		this._DataReader.ReadAsync().Returns(true, true, true, false);
		this._DataReader.Read().Returns(true, true, true, false);
	}

	[Fact]
	public void GetColumns()
	{
		var expected = new[] { "ID", "Name", "Date" };
		var actual = this._DataReader.GetColumns();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void ReadDataTable()
	{
		this.SetupReader();
		var table = this._DataReader.ReadDataTable();

		Assert.NotNull(table);

		Assert.Equal(3, table.Columns.Count);
		Assert.Equal("ID", table.Columns[0].ColumnName);
		Assert.Equal("Name", table.Columns[1].ColumnName);
		Assert.Equal("Date", table.Columns[2].ColumnName);

		Assert.Equal(3, table.Rows.Count);
		Assert.Equal([1L, "Bob Dole", _Now], table.Rows[0].ItemArray);
		Assert.Equal([2L, "Bob Dole Robot", _UtcNow], table.Rows[1].ItemArray);
		Assert.Equal([3L, "Bob Dole Clone", DBNull.Value], table.Rows[2].ItemArray);
	}

	[Fact]
	public async Task ReadModelsAsync()
	{
		this.SetupReader();
		var rows = new List<Tester>(3);
		await this._DataReader.ReadModelsAsync<Tester>(rows, default);

		Assert.NotNull(rows);
		Assert.Equal(3, rows.Count);

		this.SetupReader();
		var rows2 = new List<object>(3);
		await this._DataReader.ReadModelsAsync(typeof(Tester), rows2, default);

		Assert.NotNull(rows2);
		Assert.Equal(3, rows2.Count);
	}

	[Fact]
	public async Task ReadResultsAsJsonAsync()
	{
		this.SetupReader();
		var expected = JsonSerializer.Serialize(new[]
		{
			new Tester() { Id = 1, Name = "Bob Dole", Date = this._Now },
			new Tester() { Id = 2, Name = "Bob Dole Robot", Date = this._UtcNow },
			new Tester() { Id = 3, Name = "Bob Dole Clone", Date = null }
		});
		var actual = (await this._DataReader.ReadResultsAsJsonAsync()).ToJsonString();

		Assert.NotNull(actual);
		Assert.Equal(expected, actual);
	}
}
