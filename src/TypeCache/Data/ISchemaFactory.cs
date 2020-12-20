// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Data
{
	public interface ISchemaFactory : ISchemaProvider
	{
		string SQL { get; }
	}
}
