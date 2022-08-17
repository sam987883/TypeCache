// Copyright (c) 2021 Samuel Abraham

using System.Text.Json.Serialization;

namespace TypeCache.Data.Schema;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ObjectType
{
	Table = 1,
	View,
	Function,
	StoredProcedure,
	TableType
}
