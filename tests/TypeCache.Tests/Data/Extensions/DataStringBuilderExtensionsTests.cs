// Copyright (c) 2021 Samuel Abraham

using System.Data;
using System.Text;
using Microsoft.Extensions.Primitives;
using TypeCache.Data;
using TypeCache.Data.Extensions;
using Xunit;

namespace TypeCache.Tests.Data.Extensions;

public class DataStringBuilderExtensionsTests
{
	[Fact]
	public void AppendOutputSQL_SqlServer()
	{
		var sb = new StringBuilder();
		var columns = new StringValues(new[] { "ID", "Name" });

		sb.AppendOutputSQL(DataSourceType.SqlServer, columns);

		Assert.Contains("OUTPUT", sb.ToString());
	}

	[Fact]
	public void AppendOutputSQL_PostgreSql()
	{
		var sb = new StringBuilder();
		var columns = new StringValues(new[] { "ID", "Name" });

		sb.AppendOutputSQL(DataSourceType.PostgreSql, columns);

		Assert.Contains("OUTPUT", sb.ToString());
	}

	[Fact]
	public void AppendOutputSQL_Oracle()
	{
		var sb = new StringBuilder();
		var columns = new StringValues(new[] { "ID", "Name" });

		sb.AppendOutputSQL(DataSourceType.Oracle, columns);

		Assert.Contains("RETURNING", sb.ToString());
	}

	[Fact]
	public void AppendOutputSQL_Empty()
	{
		var sb = new StringBuilder();
		var columns = new StringValues(Array.Empty<string>());

		sb.AppendOutputSQL(DataSourceType.SqlServer, columns);

		Assert.Empty(sb.ToString());
	}

	[Fact]
	public void AppendStatementEndSQL()
	{
		var sb = new StringBuilder("SELECT * FROM Users\r\n");

		sb.AppendStatementEndSQL();

		Assert.EndsWith(";", sb.ToString().TrimEnd());
	}
}
