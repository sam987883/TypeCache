// Copyright (c) 2021 Samuel Abraham

using System.Data;
using System.Data.Common;
using System.Text.Json;
using System.Text.Json.Nodes;
using TypeCache.Extensions;

namespace TypeCache.Data.Extensions;

public static class DbDataReaderExtensions
{
	/// <summary>
	/// <c>=&gt; (0..@<paramref name="this"/>.VisibleFieldCount).Select(@<paramref name="this"/>.GetName).ToArray();</c>
	/// </summary>
	public static string[] GetColumns(this DbDataReader @this)
		=> (0..@this.VisibleFieldCount).Select(@this.GetName).ToArray();

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
	/// <see langword="    var"/> propertyMap = <see cref="TypeOf{T}.Properties"/>.ToDictionary(property =&gt; property.Name, property =&gt; property);<br/>
	/// <see langword="    var"/> properties = @<paramref name="this"/>.GetColumns().Select(column =&gt; propertyMap[column]).ToArray();<br/>
	/// <see langword="    var"/> values = <see langword="new"/> <see cref="object"/>[@<paramref name="this"/>.VisibleFieldCount];<br/>
	/// <see langword="    while"/> (<see langword="await"/> @<paramref name="this"/>.ReadAsync(<paramref name="token"/>))<br/>
	/// <see langword="    "/>{<br/>
	/// <see langword="        var"/> model = <see cref="TypeOf{T}"/>.Create();<br/>
	/// <see langword="        "/>@<paramref name="this"/>.GetValues(values);<br/>
	/// <see langword="        "/>properties.ForEach((property, columnIndex) =&gt; property.SetValue(model, values[columnIndex]));<br/>
	/// <see langword="        "/>rows.Add(model);<br/>
	/// <see langword="    "/>}<br/>
	/// <see langword="    return"/> rows;<br/>
	/// }
	/// </code>
	/// </summary>
	public static async Task ReadModelsAsync<T>(this DbDataReader @this, IList<T> rows, CancellationToken token = default)
		where T : new()
	{
		var propertyMap = TypeOf<T>.Properties.ToDictionary(property => property.Name, property => property);
		var properties = @this.GetColumns().Select(column => propertyMap[column]).ToArray();
		var values = new object[@this.VisibleFieldCount];
		while (await @this.ReadAsync(token))
		{
			var model = TypeOf<T>.Create()!;
			@this.GetValues(values);
			properties.ForEach((property, columnIndex) => property.SetValue(model, values[columnIndex]));
			rows.Add(model);
		}
	}

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    var"/> typeMember = <paramref name="modelType"/>.GetTypeMember();<br/>
	/// <see langword="    var"/> propertyMap = typeMember.Properties.ToDictionary(property =&gt; property.Name, property =&gt; property);<br/>
	/// <see langword="    var"/> properties = @<paramref name="this"/>.GetColumns().Select(column =&gt; propertyMap[column]).ToArray();<br/>
	/// <see langword="    var"/> values = <see langword="new"/> <see cref="object"/>[@<paramref name="this"/>.VisibleFieldCount];<br/>
	/// <see langword="    while"/> (<see langword="await"/> @<paramref name="this"/>.ReadAsync(<paramref name="token"/>))<br/>
	/// <see langword="    "/>{<br/>
	/// <see langword="        var"/> model = typeMember.Create();<br/>
	/// <see langword="        "/>@<paramref name="this"/>.GetValues(values);<br/>
	/// <see langword="        "/>properties.ForEach((property, columnIndex) =&gt; property.SetValue(model, values[columnIndex]));<br/>
	/// <see langword="        "/>rows.Add(model);<br/>
	/// <see langword="    "/>}<br/>
	/// <see langword="    return"/> rows;<br/>
	/// }
	/// </code>
	/// </summary>
	public static async Task ReadModelsAsync(this DbDataReader @this, Type modelType, IList<object> rows, CancellationToken token = default)
	{
		var typeMember = modelType.GetTypeMember();
		var propertyMap = typeMember.Properties.ToDictionary(property => property.Name, property => property);
		var properties = @this.GetColumns().Select(column => propertyMap[column]).ToArray();
		var values = new object[@this.VisibleFieldCount];
		while (await @this.ReadAsync(token))
		{
			var model = typeMember.Create()!;
			@this.GetValues(values);
			properties.ForEach((property, columnIndex) => property.SetValue(model, values[columnIndex]));
			rows.Add(model);
		}
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
	/// <see langword="        "/>range.ForEach(i =&gt; jsonObject.Add(columns[i], JsonValue.Create(values[i], options)));<br/>
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
			range.ForEach(i => jsonObject.Add(columns[i], JsonValue.Create(values[i], options)));
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
			range.ForEach(i =>
			{
				writer.WritePropertyName(columns[i]);
				writer.WriteValue(values[i] switch { DBNull => null, object value => value }, options);
			});
			writer.WriteEndObject();
		}
		writer.WriteEndArray();
	}
}
