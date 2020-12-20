// Copyright (c) 2021 Samuel Abraham

using System.Text.Json.Serialization;

namespace TypeCache.Common
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
}
