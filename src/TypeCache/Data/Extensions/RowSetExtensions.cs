// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.Data.Extensions;

public static class RowSetExtensions
{
	/// <summary>
	/// Maps RowSet to records by their matching constructor.<br/>
	/// Matches constructor parameter names to RowSet column names.
	/// </summary>
	public static T[] MapRecords<T>(this RowSet @this)
	{
		var constructor = TypeOf<T>.Constructors.First(_ => _!.Parameters.Map(parameter => parameter.Name).IsSet(@this.Columns));
		constructor.AssertNotNull();
		var columnIndexes = constructor!.Parameters.Map(parameter => @this.Columns.ToIndex(parameter.Name)).Gather().ToArray();

		var items = new T[@this.Rows.Length];
		@this.Rows.Do((row, rowIndex) =>
		{
			var item = TypeOf<T>.Create(columnIndexes.Map(columnIndex => row[columnIndex]).Map(value => value != DBNull.Value ? value : null)!);
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
		(PropertyMember Property, int ColumnIndex)[] properties = TypeOf<T>.Properties.Get(@this.Columns)
			.If(property => property.Setter is not null)
			.Map(property => (property, @this.Columns.ToIndex(property.Name).First()!))
			.ToArray();

		var items = new T[@this.Rows.Length];
		@this.Rows.Do((row, rowIndex) =>
		{
			var item = TypeOf<T>.Create();
			properties.Map(_ => (_.Property, Value: row[_.ColumnIndex])).Do(_ => _.Property.SetValue(item, _.Value != DBNull.Value ? _.Value : null));
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
		var getters = properties.Values.If(property => property!.Getter is not null).Map(property => property!.Name);

		var rowSet = new RowSet
		{
			Columns = columns.Any() ? columns.Match(getters, STRING_COMPARISON).ToArray() : getters.ToArray(),
			Rows = new object?[@this.Length][]
		};
		@this.Do((item, rowIndex) =>
		{
			var row = new object?[rowSet.Columns.Length];
			rowSet.Columns.Do((column, columnIndex) => row[columnIndex] = properties[column].GetValue(item) ?? DBNull.Value);
			rowSet.Rows[rowIndex] = row;
		});
		return rowSet;
	}
}
