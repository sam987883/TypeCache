// Copyright (c) 2021 Samuel Abraham

using System.Data;
using System.Data.Common;
using TypeCache.Extensions;

namespace TypeCache.Data;

/// <inheritdoc/>
public sealed class SqlCommand
{
	internal SqlCommand(IDataSource dataSource, string sql)
	{
		dataSource.AssertNotNull();
		sql.AssertNotBlank();

		this.DataSource = dataSource;
		this.SQL = sql;
	}

	/// <summary>
	/// Associated <see cref="IDataSource"/>.
	/// </summary>
	public IDataSource DataSource { get; }

	public IDictionary<string, DbType> OutputParameters { get; } = new Dictionary<string, DbType>(0, StringComparer.OrdinalIgnoreCase);

	public IDictionary<string, object?> Parameters { get; } = new Dictionary<string, object?>(0, StringComparer.OrdinalIgnoreCase);

	/// <inheritdoc cref="DbDataReader.RecordsAffected"/>
	public int RecordsAffected { get; set; }

	/// <summary>
	/// Raw SQL.
	/// </summary>
	public string SQL { get; }

	/// <inheritdoc cref="DbCommand.CommandTimeout"/>
	public TimeSpan? Timeout { get; set; }

	/// <inheritdoc cref="DbCommand.CommandType"/>
	public CommandType Type { get; set; } = CommandType.Text;

	/// <summary>
	/// <b>SQL</b>
	/// </summary>
	public override string ToString()
		=> this.SQL!;
}
