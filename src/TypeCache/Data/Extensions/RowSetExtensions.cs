// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Data.Extensions
{
	public static class RowSetExtensions
	{
		public static T[] Map<T>(this RowSet @this, bool compareCase = false)
			where T : new()
		{
			var comparer = compareCase ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;

			var items = new T[@this.Rows.Length];
			var properties = TypeOf<T>.Properties.GetValues(@this.Columns)
				.If(property => property!.Setter != null)
				.ToArray();

			@this.Rows.Do((row, rowIndex) =>
			{
				var item = TypeOf<T>.Create();
				properties.Do((property, columnIndex) => property.Setter?.Invoke(item!, row[columnIndex]));
				items[rowIndex] = item;
			});

			return items;
		}

		public static RowSet Map<T>(this T[] @this, string[] columns, bool compareCase = false)
			where T : class, new()
		{
			var comparer = compareCase ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;
			var getters = TypeOf<T>.Properties.Values.If(property => property!.Getter != null).To(property => property!.Name);
			var rowSet = new RowSet
			{
				Columns = columns.Any() ? columns.Match(getters, comparer).ToArray() : getters.ToArray(),
				Rows = new object?[@this.Length][]
			};

			@this.Do((item, rowIndex) =>
			{
				var row = new object?[rowSet.Columns.Length];
				rowSet.Columns.Do((column, columnIndex) => row[columnIndex] = TypeOf<T>.Properties[column].GetValue(item));
				rowSet.Rows[rowIndex] = row;
			});

			return rowSet;
		}
	}
}
