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
		var properties = @this.GetColumns().Select(column => Type<T>.Properties[column]).ToArray();
		var ctor = Type<T>.Constructors.FindDefault()!;
		var columnsLength = @this.VisibleFieldCount;

		while (await @this.ReadAsync(token))
		{
			var model = (T)ctor.Create()!;
			for (var i = 0; i < columnsLength; ++i)
				properties[i].SetValue(model, @this.GetValue(i));
			rows.Add(model);
		}
	}

	public static async Task ReadModelsAsync(this DbDataReader @this, Type modelType, IList<object> rows, CancellationToken token = default)
	{
		var properties = @this.GetColumns().Select(column => modelType.Properties[column]).ToArray();
		var ctor = modelType.Constructors.FindDefault()!;
		var columnsLength = @this.VisibleFieldCount;

		while (await @this.ReadAsync(token))
		{
			var model = ctor.Create()!;
			for (var i = 0; i < columnsLength; ++i)
				properties[i].SetValue(model, @this.GetValue(i));
			rows.Add(model);
		}
	}

	public static async ValueTask<JsonArray> ReadResultsAsJsonAsync(this DbDataReader @this, CancellationToken token = default)
	{
		var jsonNodeOptions = new JsonNodeOptions() { PropertyNameCaseInsensitive = true };
		var jsonArray = new JsonArray(jsonNodeOptions);
		var columns = @this.GetColumns();
		var columnsLength = @this.VisibleFieldCount;

		while (await @this.ReadAsync(token))
		{
			var jsonObject = new JsonObject(jsonNodeOptions);
			for (var i = 0; i < columnsLength; ++i)
				jsonObject.Add(columns[i], JsonValue.Create(@this.GetValue(i)));
			jsonArray.Add(jsonObject);
		}

		return jsonArray;
	}

	public static async ValueTask<JsonObject> ReadResultSetAsJsonAsync(this DbDataReader @this, CancellationToken token = default)
	{
		var json = new JsonObject(new JsonNodeOptions() { PropertyNameCaseInsensitive = true });
		var count = 1;

		json.Add(count.ToString(), await @this.ReadResultsAsJsonAsync(token));

		while (await @this.NextResultAsync(token))
		{
			++count;
			json.Add(count.ToString(), await @this.ReadResultsAsJsonAsync(token));
		}

		return json;
	}

	public static async ValueTask WriteResultsAsJsonAsync(this DbDataReader @this, Utf8JsonWriter writer, JsonSerializerOptions? jsonOptions = null, CancellationToken token = default)
	{
		var columns = @this.GetColumns();
		var columnsLength = @this.VisibleFieldCount;

		writer.WriteStartArray();

		while (await @this.ReadAsync(token))
		{
			writer.WriteStartObject();

			for (var i = 0; i < columnsLength; ++i)
			{
				writer.WritePropertyName(columns[i]);
				var value = @this.GetValue(i);
				writer.WriteValue(value is not DBNull ? value : null, jsonOptions ?? new());
			};

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
