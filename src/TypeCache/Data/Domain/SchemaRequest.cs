// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Data.Domain;

/// <summary>
/// Used for request schema information on a database object.
/// </summary>
public readonly struct SchemaRequest
{
	public SchemaRequest(string dataSource, string name)
	{
		this.DataSource = dataSource;
		this.Name = name;
	}

	/// <summary>
	/// The data source name that contains the connection string and database provider to use.
	/// </summary>
	public string DataSource { get; init; }

	/// <summary>
	/// The name of the database object to get schema information for.<br/>
	/// Database object type can only be one of the following:
	/// <list type="bullet">
	/// <item>Table</item>
	/// <item>View</item>
	/// <item>Function (table-valued only)</item>
	/// <item>Stored Procedure</item>
	/// </list>
	/// </summary>
	public string Name { get; init; }
}
