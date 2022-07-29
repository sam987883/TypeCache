// Copyright (c) 2021 Samuel Abraham

using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using static TypeCache.Default;

namespace TypeCache.Data.Domain;

/// <inheritdoc/>
public class ExecuteCommands
{
	/// <summary>
	/// The data source name that contains the connection string and database provider to use.
	/// </summary>
	public string DataSource { get; set; } = DATASOURCE;

	/// <summary>
	/// Read the results of the stored procedure.
	/// </summary>
	public Func<DbConnection, CancellationToken, ValueTask<object>>? Execute { get; set; }

	/// <summary>
	/// Specifies whether transaction flow across thread continuations is enabled for <see cref="TransactionScope"/>.
	/// </summary>
	public TransactionScopeAsyncFlowOption TransactionScopeAsyncFlowOption { get; } = TransactionScopeAsyncFlowOption.Enabled;

	/// <summary>
	/// Contains additional information that specifies transaction behaviors.
	/// </summary>
	public TransactionOptions TransactionOptions { get; } = new()
	{
		IsolationLevel = IsolationLevel.Unspecified,
		Timeout = TransactionManager.DefaultTimeout
	};

	/// <summary>
	/// Provides additional options for creating a transaction scope.
	/// </summary>
	public TransactionScopeOption TransactionScopeOption { get; } = TransactionScopeOption.Required;
}
