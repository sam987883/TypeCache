// Copyright (c) 2020 Samuel Abraham

using System.Text.Json.Serialization;

namespace Sam987883.Common.Models
{

	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum Sort
	{
		/// <summary>
		/// ASC
		/// </summary>
		Ascending,
		/// <summary>
		/// DESC
		/// </summary>
		Descending
	}
}
