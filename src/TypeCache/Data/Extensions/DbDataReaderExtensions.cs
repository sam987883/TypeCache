// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Data.Extensions;

public static class DbDataReaderExtensions
{
	/// <summary>
	/// <c>=&gt; (0..@<paramref name="this"/>.VisibleFieldCount).Map(@<paramref name="this"/>.GetName).ToArray();</c>
	/// </summary>
	public static string[] GetColumns(this DbDataReader @this)
		=> (0..@this.VisibleFieldCount).Map(@this.GetName).ToArray();

	/// <summary>
	/// <inheritdoc cref="DataTable.Load(IDataReader)"/>
	/// <code>
	/// {<br/>
	/// <see langword="    var"/> table = <see langword="new"/> <see cref="DataTable"/>();<br/>
	/// <see langword="    "/>table.BeginLoadData();<br/>
	/// <see langword="    "/>table.Load(<see langword="await"/> @<paramref name="this"/>.ExecuteReaderAsync(<paramref name="token"/>));<br/>
	/// <see langword="    "/>table.EndLoadData();<br/>
	/// <see langword="    return"/> table;<br/>
	/// }
	/// </code>
	/// </summary>
	public static DataTable ReadDataTable(this DbDataReader @this)
	{
		var table = new DataTable();
		table.BeginLoadData();
		table.Load(@this);
		table.EndLoadData();
		return table;
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    var"/> rows = <see langword="new"/> <see cref="List{T}"/>(<paramref name="initialCapacity"/>);<br/>
	/// <see langword="    "/>propertyMap = <see cref="TypeOf{T}.Properties"/>.ToDictionary(property =&gt; property.Name, property =&gt; property);<br/>
	/// <see langword="    "/>properties = @<paramref name="this"/>.GetColumns().Map(column =&gt; propertyMap[column]);<br/>
	/// <see langword="    "/>values = <see langword="new"/> <see cref="object"/>[@<paramref name="this"/>.VisibleFieldCount];<br/>
	/// <see langword="    while"/> (<see langword="await"/> @<paramref name="this"/>.ReadAsync(<paramref name="token"/>))<br/>
	/// <see langword="    "/>{<br/>
	/// <see langword="        var"/> model = <see cref="TypeOf{T}"/>.Create()!;<br/>
	/// <see langword="        "/>@<paramref name="this"/>.GetValues(values);<br/>
	/// <see langword="        "/>properties.Do((property, columnIndex) =&gt; property.SetValue(model, values[columnIndex]));<br/>
	/// <see langword="        "/>rows.Add(model);<br/>
	/// <see langword="    "/>}<br/>
	/// <see langword="    return"/> rows;<br/>
	/// }
	/// </code>
	/// </summary>
	public static async Task<IList<T>> ReadModelsAsync<T>(this DbDataReader @this, int initialCapacity = 0, CancellationToken token = default)
		where T : new()
	{
		var rows = new List<T>(initialCapacity);
		var propertyMap = TypeOf<T>.Properties.ToDictionary(property => property.Name, property => property);
		var properties = @this.GetColumns().Map(column => propertyMap[column]);
		var values = new object[@this.VisibleFieldCount];
		while (await @this.ReadAsync(token))
		{
			var model = TypeOf<T>.Create()!;
			@this.GetValues(values);
			properties.Do((property, columnIndex) => property.SetValue(model, values[columnIndex]));
			rows.Add(model);
		}
		return rows;
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    var"/> jsonArray = <see langword="new"/> <see cref="JsonArray"/>();<br/>
	/// <see langword="    var"/> options = <see langword="new"/> <see cref="JsonNodeOptions"/><br/>
	/// <see langword="    "/>{<br/>
	/// <see langword="        "/>PropertyNameCaseInsensitive = <see langword="true"/><br/>
	/// <see langword="    "/>};<br/>
	/// <see langword="    var"/> columns = @<paramref name="this"/>.GetColumns();<br/>
	/// <see langword="    var"/> values = <see langword="new"/> <see cref="object"/>[columns.Length];<br/>
	/// <see langword="    var"/> range = 0..columns.Length;<br/>
	/// <see langword="    while"/> (<see langword="await"/> @<paramref name="this"/>.ReadAsync(<paramref name="token"/>))<br/>
	/// <see langword="    "/>{<br/>
	/// <see langword="        var"/> jsonObject = <see langword="new"/> <see cref="JsonObject"/>(options);<br/>
	/// <see langword="        "/>@<paramref name="this"/>.GetValues(values);<br/>
	/// <see langword="        "/>range.Do(i =&gt; jsonObject.Add(columns[i], JsonValue.Create(values[i], options)));<br/>
	/// <see langword="        "/>jsonArray.Add(jsonObject);<br/>
	/// <see langword="    "/>}<br/>
	/// <see langword="    return"/> jsonArray;<br/>
	/// }
	/// </code>
	/// </summary>
	public static async ValueTask<JsonArray> ReadJsonArrayAsync(this DbDataReader @this, CancellationToken token = default)
	{
		var jsonArray = new JsonArray();
		var options = new JsonNodeOptions
		{
			PropertyNameCaseInsensitive = true
		};
		var columns = @this.GetColumns();
		var values = new object[columns.Length];
		var range = 0..columns.Length;
		while (await @this.ReadAsync(token))
		{
			var jsonObject = new JsonObject(options);
			@this.GetValues(values);
			range.Do(i => jsonObject.Add(columns[i], JsonValue.Create(values[i], options)));
			jsonArray.Add(jsonObject);
		}
		return jsonArray;
	}

	public static async ValueTask ReadJsonAsync(this DbDataReader @this, Utf8JsonWriter writer, CancellationToken token = default)
	{
		var columns = @this.GetColumns();
		var range = 0..columns.Length;
		var values = new object[columns.Length];
		var options = new JsonSerializerOptions();
		writer.WriteStartArray();
		while (await @this.ReadAsync(token))
		{
			writer.WriteStartObject();
			@this.GetValues(values);
			range.Do(i =>
			{
				writer.WritePropertyName(columns[i]);
				writer.WriteValue(values[i] switch { DBNull => null, object value => value }, options);
			});
			writer.WriteEndObject();
		}
		writer.WriteEndArray();
	}

	public static async Task<T[]> ReadRowsAsync<T>(this DbDataReader @this, int initialCapacity = 0, CancellationToken token = default)
	{
		var type = TypeOf<T>.Member;
		var columnCount = @this.VisibleFieldCount;
		var rows = new List<T>(initialCapacity);
		if (type.SystemType == SystemType.Tuple)
		{
			var values = new object[columnCount];
			var tupleType = typeof(Tuple).GetTypeMember();
			var genericTypes = type.GenericTypes.As<Type>().ToArray();
			while (await @this.ReadAsync(token))
			{
				@this.GetValues(values);
				rows.Add((T)tupleType.InvokeGenericMethod(nameof(Tuple.Create), genericTypes!, values)!);
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
				rows.Add((T)valueTupleType.InvokeGenericMethod(nameof(ValueTuple.Create), genericTypes!, values)!);
			}
		}
		else if (type.SystemType == SystemType.Array && type.ElementType!.SystemType == SystemType.Object)
		{
			while (await @this.ReadAsync(token))
			{
				var values = new object[columnCount];
				@this.GetValues(values);
				rows.Add((T)(object)values);
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
				rows.Add(await @this.GetFieldValueAsync<T>(0, token));
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
				rows.Add((T)(object)dictionary);
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
				rows.Add(model);
			}
		}
		return rows.ToArray();
	}
}
