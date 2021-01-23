// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Data;

namespace TypeCache.Extensions
{
	public static class RowSetExtensions
	{
		public static T[] Map<T>(this RowSet @this, bool compareCase = false)
			where T : class, new()
		{
			var comparer = compareCase ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;

			var items = new T[@this.Rows.Length];
			var properties = Class<T>.Properties.GetValues(@this.Columns)
				.If(property => property.Setter != null)
				.ToArray();

			@this.Rows.Do((row, rowIndex) =>
			{
				var item = Class<T>.Create();
				properties.Do((property, columnIndex) => property[item] = row[columnIndex]);
				items[rowIndex] = item;
			});

			return items;
		}
	}
}
