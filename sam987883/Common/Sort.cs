// Copyright (c) 2020 Samuel Abraham

using System.Text.Json.Serialization;

namespace sam987883.Common
{
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum Sort
	{
		Ascending,
		Descending
	}
}
