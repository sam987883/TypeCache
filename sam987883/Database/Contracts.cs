// Copyright (c) 2020 Samuel Abraham

using System.Data;

namespace sam987883.Database
{
	public interface ISchemaFactory
	{
		TableSchema LoadTableSchema(IDbConnection connection, string tableSource);
	}

	public interface ISchemaStore
	{
		TableSchema GetTableSchema(IDbConnection connection, string tableSource);
	}
}
