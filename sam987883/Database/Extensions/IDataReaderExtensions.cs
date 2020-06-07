// Copyright (c) 2020 Samuel Abraham

using sam987883.Common;
using sam987883.Extensions;
using sam987883.Reflection;
using System.Collections.Generic;
using System.Data;

namespace sam987883.Database.Extensions
{
	public static class IDataReaderExtensions
	{
		public static IEnumerable<(T? Deleted, T? Inserted)> Read<T>(this IDataReader @this, IPropertyCache<T> propertyCache, string[] outputDeletedColumns, string[] outputInsertedColumns) where T : class, new()
		{
			var values = new object[@this.FieldCount];

			if (outputDeletedColumns.Any() && outputInsertedColumns.Any())
			{
				var deletedProperties = propertyCache.Properties.GetValues(outputDeletedColumns);
				var insertedProperties = propertyCache.Properties.GetValues(outputInsertedColumns);
				var offset = outputDeletedColumns.Length;
				while (@this.Read())
				{
					@this.GetValues(values);
					var outputDeletedRow = Factory.Create<T>();
					deletedProperties.Do((property, index) => property[outputDeletedRow] = values[index]);
					var outputInsertedRow = Factory.Create<T>();
					insertedProperties.Do((property, index) => property[outputInsertedRow] = values[index + offset]);
					yield return (outputDeletedRow, outputInsertedRow);
				}
			}
			else if (outputDeletedColumns.Any())
			{
				var deletedProperties = propertyCache.Properties.GetValues(outputDeletedColumns);
				while (@this.Read())
				{
					@this.GetValues(values);
					var outputDeletedRow = Factory.Create<T>();
					deletedProperties.Do((property, index) => property[outputDeletedRow] = values[index]);
					yield return (outputDeletedRow, default);
				}
			}
			else if (outputInsertedColumns.Any())
			{
				var insertedProperties = propertyCache.Properties.GetValues(outputInsertedColumns);
				while (@this.Read())
				{
					@this.GetValues(values);
					var outputInsertedRow = Factory.Create<T>();
					insertedProperties.Do((property, index) => property[outputInsertedRow] = values[index]);
					yield return (default, outputInsertedRow);
				}
			}
		}

		public static IEnumerable<T> Read<T>(this IDataReader @this, IPropertyCache<T> propertyCache) where T : class, new()
		{
			var fieldNames = 0.Range(@this.FieldCount).To(@this.GetName).ToArray(@this.FieldCount);
			var fields = propertyCache.Properties.GetValues(fieldNames);
			var values = new object[@this.FieldCount];

			while (@this.Read())
			{
				@this.GetValues(values);
				var row = Factory.Create<T>();
				fields.Do((property, index) => property[row] = values[index]);
				yield return row;
			}
		}

		public static string[] GetNames(this IDataReader @this) =>
			0.Range(@this.FieldCount).To(@this.GetName).ToArray(@this.FieldCount);

		public static IEnumerable<object[]> ReadValues(this IDataReader @this)
		{
			var columnCount = @this.FieldCount;
			while (@this.Read())
			{
				var values = new object[columnCount];
				@this.GetValues(values);
				yield return values;
			}
		}
	}
}