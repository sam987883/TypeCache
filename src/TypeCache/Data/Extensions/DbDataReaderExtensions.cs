// Copyright (c) 2021 Samuel Abraham

using System.Data;
using System.Data.Common;
using System.Text.Json;
using System.Text.Json.Nodes;
using TypeCache.Extensions;
using static System.Reflection.BindingFlags;

namespace TypeCache.Data.Extensions;

public static class DbDataReaderExtensions
{
	/// <summary>
	/// <c>=&gt; (0..@<paramref name="this"/>.VisibleFieldCount).Select(@<paramref name="this"/>.GetName).ToArray();</c>
	/// </summary>
	[DebuggerHidden]
	public static string[] GetColumns(this DbDataReader @this)
		=> (0..@this.VisibleFieldCount).ToEnumerable().Select(@this.GetName).ToArray();

	public static DataTable ReadDataTable(this DbDataReader @this)
	{
		var table = new DataTable();
		table.BeginLoadData();
		table.Load(@this);
		table.EndLoadData();
		return table;
	}

	public static async Task ReadModelsAsync<T>(this DbDataReader @this, IList<T> rows, CancellationToken token = default)
		where T : notnull, new()
	{
		var propertyMap = typeof(T).GetInstanceProperties().ToDictionary(property => property.Name(), property => property);
		var properties = @this.GetColumns().Select(column => propertyMap[column]).ToArray();
		var values = new object[@this.VisibleFieldCount];

		while (await @this.ReadAsync(token))
		{
			var model = (T)typeof(T).Create()!;
			@this.GetValues(values);
			properties.ForEach((property, columnIndex) => property.SetPropertyValue(model, values[columnIndex]));
			rows.Add((T)model);
		}
	}

	public static async Task ReadModelsAsync(this DbDataReader @this, Type modelType, IList<object> rows, CancellationToken token = default)
	{
		var propertyMap = modelType.GetProperties(Instance | Public).ToDictionary(property => property.Name(), property => property);
		var properties = @this.GetColumns().Select(column => propertyMap[column]).ToArray();
		var values = new object[@this.VisibleFieldCount];

		while (await @this.ReadAsync(token))
		{
			var model = modelType.Create()!;
			@this.GetValues(values);
			properties.ForEach((property, columnIndex) => property.SetPropertyValue(model, values[columnIndex]));
			rows.Add(model);
		}
	}

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
				writer.WriteValue(values[i] switch
				{
					DBNull => null,
					var value => value
				}, options);
			});
			writer.WriteEndObject();
		}

		writer.WriteEndArray();
	}
}
