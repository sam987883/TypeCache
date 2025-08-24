// Copyright (c) 2021 Samuel Abraham

using System.Data;
using System.Data.Common;
using System.Text.Json;
using System.Text.Json.Nodes;
using TypeCache.Extensions;
using TypeCache.Reflection;

namespace TypeCache.Data.Extensions;

public static class DbDataReaderExtensions
{
	/// <summary>
	/// <c>=&gt; (0..@<paramref name="this"/>.VisibleFieldCount).Select(@<paramref name="this"/>.GetName).ToArray();</c>
	/// </summary>
	[DebuggerHidden]
	public static string[] GetColumns(this DbDataReader @this)
		=> (0..@this.FieldCount).ToEnumerable().Select(@this.GetName).ToArray();

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
		var properties = @this.GetColumns().Select(column => Type<T>.Properties[column]).ToArray();
		var values = new object[properties.Length];

		while (await @this.ReadAsync(token))
		{
			var model = Type<T>.Create()!;
			@this.GetValues(values);
			properties.ForEach((property, columnIndex) => property.SetValue(model, values[columnIndex]));
			rows.Add(model);
		}
	}

	public static async Task ReadModelsAsync(this DbDataReader @this, Type modelType, IList<object> rows, CancellationToken token = default)
	{
		var properties = @this.GetColumns().Select(column => modelType.Properties()[column]).ToArray();
		var values = new object[properties.Length];

		while (await @this.ReadAsync(token))
		{
			var model = modelType.Create()!;
			@this.GetValues(values);
			properties.ForEach((property, columnIndex) => property.SetValue(model, values[columnIndex]));
			rows.Add(model);
		}
	}

	public static async ValueTask<JsonArray> ReadResultsAsJsonAsync(this DbDataReader @this, JsonNodeOptions? jsonOptions = null, CancellationToken token = default)
	{
		var jsonArray = jsonOptions.HasValue ? new JsonArray(jsonOptions.Value) : new();
		var columns = @this.GetColumns();
		var values = new object[columns.Length];
		var range = 0..columns.Length;

		while (await @this.ReadAsync(token))
		{
			var jsonObject = jsonOptions.HasValue ? new JsonObject(jsonOptions.Value) : new();
			@this.GetValues(values);
			range.ForEach(i => jsonObject.Add(columns[i], JsonValue.Create(values[i], jsonOptions)));
			jsonArray.Add(jsonObject);
		}

		return jsonArray;
	}

	public static async ValueTask<JsonObject> ReadResultSetAsJsonAsync(this DbDataReader @this, JsonNodeOptions? jsonOptions = null, CancellationToken token = default)
	{
		var json = jsonOptions.HasValue ? new JsonObject(jsonOptions.Value) : new();
		var count = 1;

		json.Add(count.ToString(), await @this.ReadResultsAsJsonAsync(jsonOptions, token));

		while (await @this.NextResultAsync(token))
		{
			++count;
			json.Add(count.ToString(), await @this.ReadResultsAsJsonAsync(jsonOptions, token));
		}

		return json;
	}

	public static async ValueTask WriteResultsAsJsonAsync(this DbDataReader @this, Utf8JsonWriter writer, JsonSerializerOptions? jsonOptions = null, CancellationToken token = default)
	{
		var columns = @this.GetColumns();
		var range = 0..columns.Length;
		var values = new object[columns.Length];

		writer.WriteStartArray();

		while (await @this.ReadAsync(token))
		{
			@this.GetValues(values);

			writer.WriteStartObject();

			range.ForEach(i =>
			{
				writer.WritePropertyName(columns[i]);
				var value = values[i];
				writer.WriteValue(value is not DBNull ? value : null, jsonOptions ?? new());
			});

			writer.WriteEndObject();
		}

		writer.WriteEndArray();
	}

	public static async ValueTask WriteResultSetAsJsonAsync(this DbDataReader @this, Utf8JsonWriter writer, JsonSerializerOptions? jsonOptions = null, CancellationToken token = default)
	{
		var count = 1;

		writer.WriteStartObject();

		writer.WritePropertyName(count.ToString());
		await @this.WriteResultsAsJsonAsync(writer, jsonOptions, token);

		while (await @this.NextResultAsync(token))
		{
			++count;
			writer.WritePropertyName(count.ToString());
			await @this.WriteResultsAsJsonAsync(writer, jsonOptions, token);
		}

		writer.WriteEndObject();
	}
}
