// Copyright (c) 2021 Samuel Abraham

using System.Text.Json.Serialization;

namespace TypeCache.Data;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DatabaseObjectType
{
	Table = 1,
	View,
	Function,
	StoredProcedure,
	TableType
}
