// Copyright (c) 2020 Samuel Abraham

using sam987883.Database.Schemas;
using System.Data;

namespace sam987883.Database
{
	public interface IRowSetConverter<T>
		where T : class, new()
	{
		RowSet ToRowSet(T[] items, params string[] columns);
		T[] FromRowSet(RowSet rowSet);
	}

	public interface ISchemaFactory
	{
		ObjectSchema LoadObjectSchema(IDbConnection connection, string name);
	}

	public interface ISchemaStore
	{
		ObjectSchema GetObjectSchema(IDbConnection connection, string name);
	}
}
