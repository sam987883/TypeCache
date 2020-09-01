// Copyright (c) 2020 Samuel Abraham

namespace Sam987883.Database
{
	public interface ISchemaFactory : ISchemaProvider
	{
		string SQL { get; }
	}
}
