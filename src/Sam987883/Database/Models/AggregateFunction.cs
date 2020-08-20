// Copyright (c) 2020 Samuel Abraham

namespace Sam987883.Database.Models
{
	/// <summary>
	/// Database aggregate function.
	/// </summary>
	public enum AggregateFunction
	{
		None = 0,

		/// <summary>
		/// AVG
		/// </summary>
		Average,

		/// <summary>
		/// CHECKSUM_AGG
		/// </summary>
		CheckSum,

		/// <summary>
		/// COUNT, COUNT_BIG
		/// </summary>
		Count,

		/// <summary>
		/// MAX
		/// </summary>
		Maximum,

		/// <summary>
		/// MIN
		/// </summary>
		Minimum,

		/// <summary>
		/// DENSE_RANK
		/// </summary>
		Rank,

		/// <summary>
		/// ROW_NUMBER
		/// </summary>
		RowNumber,

		/// <summary>
		/// STDEV
		/// </summary>
		StandardDeviation,

		/// <summary>
		/// STDEVP
		/// </summary>
		PopulationStandardDeviation,

		/// <summary>
		/// SUM
		/// </summary>
		Summation,

		/// <summary>
		/// VAR
		/// </summary>
		Variance,

		/// <summary>
		/// VARP
		/// </summary>
		PopulationVariance
	}
}
