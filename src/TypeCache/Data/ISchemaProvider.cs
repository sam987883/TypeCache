// Copyright (c) 2021 Samuel Abraham

using System.Data;

namespace TypeCache.Data
{
	public interface ISchemaProvider
	{
		ObjectSchema GetObjectSchema(IDbConnection connection, string name);
	}
}
