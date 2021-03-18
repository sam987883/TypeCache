// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Data.Extensions
{
	public static class RowSetExtensions
	{
		/// <summary>
		/// Maps RowSet to records by their matching constructor.<br/>
		/// Matches constructor parameter names to RowSet column names.
		/// </summary>
		public static T[] MapRecords<T>(this RowSet @this)
		{
			var constructor = TypeOf<T>.Constructors.First(_ => _!.Parameters.To(parameter => parameter.Name).IsSet(@this.Columns, StringComparer.OrdinalIgnoreCase));
			constructor.AssertNotNull(nameof(constructor));
			var columnIndexes = constructor!.Parameters.To(parameter => @this.Columns.ToIndex(parameter.Name, false)).Gather().ToArray();

			var items = new T[@this.Rows.Length];
			@this.Rows.Do((row, rowIndex) =>
			{
				var item = TypeOf<T>.Create(columnIndexes.To(columnIndex => row[columnIndex]).ToArray(columnIndexes.Length)!);
				items[rowIndex] = item;
			});
			return items;
		}

		/// <summary>
		/// Maps RowSet to models by their properties.<br/>
		/// Matches model property names to RowSet column names.
		/// </summary>
		public static T[] MapModels<T>(this RowSet @this)
			where T : new()
		{
			var properties = TypeOf<T>.Properties.GetValues(@this.Columns)
				.If(property => property!.Setter is not null)
				.ToArray();

			var items = new T[@this.Rows.Length];
			@this.Rows.Do((row, rowIndex) =>
			{
				var item = TypeOf<T>.Create();
				properties.Do((property, columnIndex) => property?.Setter?.Invoke!(item!, row[columnIndex]));
				items[rowIndex] = item;
			});
			return items;
		}

		/// <summary>
		/// Maps models to RowSet by their properties.<br/>
		/// Matches model's readable property names to columns specified.
		/// </summary>
		public static RowSet MapRowSet<T>(this T[] @this, string[] columns)
		{
			var properties = TypeOf<T>.Properties;
			var getters = properties.Values.If(property => property!.GetValue is not null).To(property => property!.Name);

			var rowSet = new RowSet(columns.Any() ? columns.Match(getters, StringComparer.OrdinalIgnoreCase).ToArray() : getters.ToArray(), new object?[@this.Length][]);
			@this.Do((item, rowIndex) =>
			{
				var row = new object?[rowSet.Columns.Length];
				rowSet.Columns.Do((column, columnIndex) => row[columnIndex] = properties[column].GetValue!(item!));
				rowSet.Rows[rowIndex] = row;
			});
			return rowSet;
		}
	}
}
