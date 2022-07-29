// Copyright (c) 2021 Samuel Abraham

using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace TypeCache.Data.Domain;

/// <inheritdoc/>
public class SqlCommand : Command
{
	/// <summary>
	/// Read the results of the stored procedure.
	/// </summary>
	public Func<DbDataReader, CancellationToken, ValueTask<object>>? ReadData { get; set; }

	/// <summary>
	/// Raw SQL.
	/// </summary>
	public string SQL { get; set; } = string.Empty;

	/// <inheritdoc/>
	public override string ToSQL()
		=> this.SQL;
}
