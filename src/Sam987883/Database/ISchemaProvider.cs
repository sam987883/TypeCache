// Copyright (c) 2020 Samuel Abraham

using Sam987883.Database.Models;
using System.Data;

namespace Sam987883.Database
{
	public interface ISchemaProvider
	{
		ObjectSchema GetObjectSchema(IDbConnection connection, string name);
	}
}
