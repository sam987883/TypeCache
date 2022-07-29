// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Threading;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Data.Extensions;

public static class DbDataReaderExtensions
{
	public static string[] GetColumns(this DbDataReader @this)
		=> (0..@this.VisibleFieldCount).Map(@this.GetName).ToArray();

	public static async IAsyncEnumerable<T> ReadRowsAsync<T>(this DbDataReader @this, [EnumeratorCancellation] CancellationToken token = default)
	{
		var type = TypeOf<T>.Member;
		var count = @this.VisibleFieldCount;
		if (type.SystemType == SystemType.Tuple)
		{
			var values = new object[count];
			var tupleType = typeof(Tuple).GetTypeMember();
			var genericTypes = type.GenericTypes.As<Type>().ToArray();
			while (await @this.ReadAsync(token))
			{
				@this.GetValues(values);
				yield return (T)tupleType.InvokeGenericStaticMethod(nameof(Tuple.Create), genericTypes!, values)!;
			}
		}
		else if (type.SystemType == SystemType.ValueTuple)
		{
			var values = new object[count];
			var valueTupleType = typeof(ValueTuple).GetTypeMember();
			var genericTypes = type.GenericTypes.As<Type>().ToArray();
			while (await @this.ReadAsync(token))
			{
				@this.GetValues(values);
				yield return (T)valueTupleType.InvokeGenericStaticMethod(nameof(ValueTuple.Create), genericTypes!, values)!;
			}
		}
		else if (type.SystemType == SystemType.Array && type.ElementType!.SystemType == SystemType.Object)
		{
			while (await @this.ReadAsync(token))
			{
				var values = new object[count];
				@this.GetValues(values);
				yield return (T)(object)values;
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
			yield return await @this.GetFieldValueAsync<T>(0, token);
		else if (type.SystemType == SystemType.Dictionary || type.SystemType == SystemType.IDictionary)
		{
			var columns = @this.GetColumns();
			var values = new object[count];
			while (await @this.ReadAsync(token))
			{
				@this.GetValues(values);
				var dictionary = new Dictionary<string, object>(count, StringComparer.OrdinalIgnoreCase);
				var i = -1;
				while (++i < count)
					dictionary.Add(columns[i], values[i]);
				yield return (T)(object)dictionary;
			}
		}
		else
		{
			var propertyMap = TypeOf<T>.Member.Properties.ToDictionary(property => property.Name, property => property);
			var properties = @this.GetColumns().Map(column => propertyMap[column]).ToArray();
			var values = new object[count];
			while (await @this.ReadAsync(token))
			{
				var model = TypeOf<T>.Create()!;
				@this.GetValues(values);
				properties.Do((property, i) => property.SetValue(model, values[i]));
				yield return model;
			}
		}
	}
}
