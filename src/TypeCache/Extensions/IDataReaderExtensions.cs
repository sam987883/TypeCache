// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Data;
using TypeCache.Data;

namespace TypeCache.Extensions
{
	public static class IDataReaderExtensions
	{
		public static string[] GetColumns(this IDataReader @this)
			=> 0.Range(@this.FieldCount).To(@this.GetName).ToArrayOf(@this.FieldCount);

		public static IEnumerable<object[]> ReadRows(this IDataReader @this)
		{
			var columnCount = @this.FieldCount;
			while (@this.Read())
			{
				var values = new object[columnCount];
				@this.GetValues(values);
				yield return values;
			}
		}

		public static RowSet ReadRowSet(this IDataReader @this)
			=> new RowSet
			{
				Columns = @this.GetColumns(),
				Rows = @this.ReadRows().ToList().ToArray()
			};

		public static IEnumerable<RowSet> ReadRowSets(this IDataReader @this)
		{
			do
			{
				yield return @this.ReadRowSet();
			} while (@this.NextResult());
		}
	}
}