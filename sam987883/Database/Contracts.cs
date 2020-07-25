// Copyright (c) 2020 Samuel Abraham

using sam987883.Common.Models;
using sam987883.Database.Models;
using System.Data;

namespace sam987883.Database
{
	public interface ISchemaFactory
	{
		ObjectSchema LoadObjectSchema(IDbConnection connection, string name);
	}

	public interface ISchemaStore
	{
		ObjectSchema GetObjectSchema(IDbConnection connection, string name);
	}
}
