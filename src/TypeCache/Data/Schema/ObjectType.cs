// Copyright (c) 2021 Samuel Abraham

using System.Text.Json.Serialization;

namespace TypeCache.Data.Schema
{
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum ObjectType
	{
		Table,
		View,
		Function,
		StoredProcedure
	}
}
