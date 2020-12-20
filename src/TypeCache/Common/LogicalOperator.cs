// Copyright (c) 2021 Samuel Abraham

using System.Text.Json.Serialization;

namespace TypeCache.Common
{
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum LogicalOperator
	{
		And,
		Or
	}
}
