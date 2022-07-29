// Copyright (c) 2021 Samuel Abraham

using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace TypeCache.Data.Domain;

/// <summary>
/// JSON: <code>{ "Procedure": "[Database1]..[Procedure1]", "Parameters": { "Parameter1": "Value1", "Parameter2": null, "Parameter3": true } }</code>
/// SQL: <code>EXECUTE [Database1]..[Procedure1] (@Parameter1 = N'Value1', @Parameter2 = NULL, @Parameter3 = 1);</code>
/// </summary>
public class StoredProcedureCommand : Command
{
	public StoredProcedureCommand(string procedure)
	{
		this.Procedure = procedure;
	}

	/// <summary>
	/// <list type="bullet">
	/// <item><term><c><see cref="System.Data.CommandType.StoredProcedure"/></c></term> <description>Stored Procedure commands only.</description></item>
	/// </list>
	/// </summary>
	public override CommandType CommandType { get; } = CommandType.StoredProcedure;

	/// <summary>
	/// Stored procedure name.
	/// </summary>
	public string Procedure { get; set; } = string.Empty;

	/// <summary>
	/// Read the results of the stored procedure.
	/// </summary>
	public Func<DbDataReader, CancellationToken, ValueTask<object>>? ReadData { get; set; }

	/// <inheritdoc/>
	public override string ToSQL()
		=> this.Procedure;
}
