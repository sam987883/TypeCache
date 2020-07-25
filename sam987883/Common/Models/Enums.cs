// Copyright (c) 2020 Samuel Abraham

using System.Text.Json.Serialization;

namespace sam987883.Common.Models
{
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum ComparisonOperator
	{
		/// <summary>
		/// =
		/// </summary>
		Equal,
		/// <summary>
		/// &lt;&gt;
		/// </summary>
		NotEqual,
		/// <summary>
		/// &gt;
		/// </summary>
		MoreThan,
		/// <summary>
		/// &gt;=
		/// </summary>
		MoreThanOrEqual,
		/// <summary>
		/// &lt;
		/// </summary>
		LessThan,
		/// <summary>
		/// &lt;=
		/// </summary>
		LessThanOrEqual,
		/// <summary>
		/// LIKE N'Value%'
		/// </summary>
		StartWith,
		/// <summary>
		/// NOT LIKE N'Value%'
		/// </summary>
		NotStartWith,
		/// <summary>
		/// LIKE N'%Value'
		/// </summary>
		EndWith,
		/// <summary>
		/// NOT LIKE N'%Value'
		/// </summary>
		NotEndWith,
		/// <summary>
		/// LIKE N'%Value%'
		/// </summary>
		Like,
		/// <summary>
		/// NOT LIKE N'%Value%'
		/// </summary>
		NotLike
	}

	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum LogicalOperator
	{
		And,
		Or
	}

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
