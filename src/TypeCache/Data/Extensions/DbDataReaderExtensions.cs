// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Data.Extensions;

public static class DbDataReaderExtensions
{
	public static string[] GetColumns(this DbDataReader @this)
		=> (0..@this.VisibleFieldCount).Map(@this.GetName).ToArray();

	public static async Task<T[]> ReadRowsAsync<T>(this DbDataReader @this, int rowCount, CancellationToken token = default)
	{
		var type = TypeOf<T>.Member;
		var columnCount = @this.VisibleFieldCount;
		var rows = new T[rowCount];
		var i = -1;
		if (type.SystemType == SystemType.Tuple)
		{
			var values = new object[columnCount];
			var tupleType = typeof(Tuple).GetTypeMember();
			var genericTypes = type.GenericTypes.As<Type>().ToArray();
			while (await @this.ReadAsync(token))
			{
				@this.GetValues(values);
				rows[++i] = (T)tupleType.InvokeGenericMethod(nameof(Tuple.Create), genericTypes!, values)!;
			}
		}
		else if (type.SystemType == SystemType.ValueTuple)
		{
			var values = new object[columnCount];
			var valueTupleType = typeof(ValueTuple).GetTypeMember();
			var genericTypes = type.GenericTypes.As<Type>().ToArray();
			while (await @this.ReadAsync(token))
			{
				@this.GetValues(values);
				rows[++i] = (T)valueTupleType.InvokeGenericMethod(nameof(ValueTuple.Create), genericTypes!, values)!;
			}
		}
		else if (type.SystemType == SystemType.Array && type.ElementType!.SystemType == SystemType.Object)
		{
			while (await @this.ReadAsync(token))
			{
				var values = new object[columnCount];
				@this.GetValues(values);
				rows[++i] = (T)(object)values;
			}
		}
		else if (type.Kind == Kind.Enum
			|| type.SystemType.IsPrimitive()
			|| type.SystemType == SystemType.Half
			|| type.SystemType == SystemType.Decimal
			|| type.SystemType == SystemType.Guid
			|| type.SystemType == SystemType.DateOnly
			|| type.SystemType == SystemType.DateTime
			|| type.SystemType == SystemType.DateTimeOffset
			|| type.SystemType == SystemType.TimeOnly
			|| type.SystemType == SystemType.TimeSpan
			|| type.SystemType == SystemType.String
			|| (type.SystemType == SystemType.Array && type.ElementType!.SystemType == SystemType.Byte))
		{
			while (await @this.ReadAsync(token))
			{
				rows[++i] = await @this.GetFieldValueAsync<T>(0, token);
			}
		}
		else if (type.SystemType == SystemType.Dictionary || type.SystemType == SystemType.IDictionary)
		{
			var columns = @this.GetColumns();
			var values = new object[columnCount];
			while (await @this.ReadAsync(token))
			{
				@this.GetValues(values);
				var dictionary = new Dictionary<string, object>(columnCount, StringComparer.OrdinalIgnoreCase);
				columns.Do((column, c) => dictionary.Add(column, values[c]));
				rows[++i] = (T)(object)dictionary;
			}
		}
		else
		{
			var propertyMap = TypeOf<T>.Member.Properties.ToDictionary(property => property.Name, property => property);
			var properties = @this.GetColumns().Map(column => propertyMap[column]).ToArray();
			var values = new object[columnCount];
			while (await @this.ReadAsync(token))
			{
				var model = TypeOf<T>.Create()!;
				@this.GetValues(values);
				properties.Do((property, c) => property.SetValue(model, values[c]));
				rows[++i] = model;
			}
		}
		return rows;
	}
}
