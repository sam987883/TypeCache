// Copyright (c) 2020 Samuel Abraham

using sam987883.Extensions;
using System.Collections.Generic;
using System.Data;

namespace sam987883.Database.Extensions
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

		public static Output ReadRowSets(this IDataReader @this, string[] outputDeletedColumns, string[] outputInsertedColumns)
		{
			var rows = @this.ReadRows().ToList().ToArray();
			var output = new Output();
			if (outputDeletedColumns.Any())
				output.Deleted = new RowSet
				{
					Columns = outputDeletedColumns,
					Rows = rows.To(row => row.Subarray(0, outputDeletedColumns.Length)).ToArray(outputDeletedColumns.Length)
				};
			if (outputInsertedColumns.Any())
				output.Deleted = new RowSet
				{
					Columns = outputDeletedColumns,
					Rows = rows.To(row => row.Subarray(0, outputDeletedColumns.Length)).ToArray(outputDeletedColumns.Length)
				};
			return output;
		}

		public static IEnumerable<RowSet> ReadRowSets(this IDataReader @this)
		{
			do
			{
				yield return @this.ReadRowSet();
			} while (@this.NextResult());
		}
	}
}