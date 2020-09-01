// Copyright (c) 2020 Samuel Abraham

using Sam987883.Common.Extensions;
using Sam987883.Common.Models;
using System.Collections.Generic;
using System.Data;

namespace Sam987883.Database.Extensions
{
	public static class IDataReaderExtensions
	{
		public static string[] GetColumns(this IDataReader @this) =>
			0.Range(@this.FieldCount).To(@this.GetName).ToArray(@this.FieldCount);

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

		public static RowSet ReadRowSet(this IDataReader @this) => new RowSet
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