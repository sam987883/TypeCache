// Copyright (c) 2021 Samuel Abraham

using System.Data;
using System.Data.Common;
using System.Transactions;
using TypeCache.Extensions;
using TransactionIsolationLevel = System.Transactions.IsolationLevel;

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

	/// <summary>
	/// The initial capacity used for the <see cref="IList{T}"/> returned from a <see cref="DbCommand"/>.
	/// </summary>
	public int InitialCapacity { get; set; }

	/// <inheritdoc cref="TransactionIsolationLevel"/>
	public TransactionIsolationLevel IsolationLevel { get; set; } = TransactionIsolationLevel.Unspecified;

	/// <summary>
	/// <code>
	/// DECLARE @ParameterName4 AS INTEGER;<br/>
	/// DECLARE @ParameterName5 AS NVARCHAR(MAX);<br/>
	/// DECLARE @ParameterName6 AS DATETIMEOFFSET;
	/// </code>
	/// </summary>
	public IDictionary<string, DbType> OutputParameters { get; } = new Dictionary<string, DbType>(0, StringComparer.OrdinalIgnoreCase);

	/// <summary>
	/// <code>
	/// SET @ParameterName1 = N'ParameterValue1';<br/>
	/// SET @ParameterName2 = NULL;<br/>
	/// SET @ParameterName3 = 123;
	/// </code>
	/// </summary>
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

	/// <inheritdoc cref="TransactionScopeOption"/>
	public TransactionScopeOption TransactionScope { get; set; } = TransactionScopeOption.Required;

	public TransactionScope? StartTransaction()
	{
		if (this.IsolationLevel is TransactionIsolationLevel.Unspecified)
			return null;

		var options = new TransactionOptions
		{
			IsolationLevel = this.IsolationLevel
		};
		if (this.Timeout.HasValue)
			options.Timeout = this.Timeout.Value;

		return new TransactionScope(this.TransactionScope, options, TransactionScopeAsyncFlowOption.Enabled);
	}

	/// <summary>
	/// <b>SQL</b>
	/// </summary>
	public override string ToString()
		=> this.SQL!;
}
