// Copyright (c) 2021 Samuel Abraham

using System.Data.Common;
using System.Threading.Tasks;

namespace TypeCache.Data
{
	public interface ISchemaProvider
	{
		ValueTask<ObjectSchema> GetObjectSchema(DbConnection connection, string name);
	}
}
