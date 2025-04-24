// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class JsonExtensions
{
	[Fact]
	public void GetArrayElements()
	{
		var arrayJson = JsonSerializer.Deserialize<JsonElement>(@$"[null,true,3,{long.MaxValue},{ulong.MaxValue},""{DateTime.UtcNow.ToShortDateString()}"",""{Guid.NewGuid():D}"",""Word""]");
		var array = new[] { "A", "b", "C" };
		var jsonElement = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(array));
		var jsonElements = jsonElement.GetArrayElements();

		Assert.Equal(8, arrayJson.GetArrayElements().Length);
		Assert.Equal(array[0], jsonElements[0].GetString());
		Assert.Equal(array[1], jsonElements[1].GetString());
		Assert.Equal(array[2], jsonElements[2].GetString());
	}

	[Fact]
	public void GetArrayValues()
	{
		var arrayJson = JsonSerializer.Deserialize<JsonElement>(@$"[null,true,3,{long.MaxValue},{ulong.MaxValue},""{DateTime.UtcNow.ToShortDateString()}"",""{Guid.NewGuid():D}"",""Word""]");
		var array = new[] { "A", "b", "C" };
		var jsonElement = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(array));
		var values = jsonElement.GetArrayValues();

		Assert.Equal(8, arrayJson.GetArrayValues().Length);
		Assert.Equal(array[0], values[0]);
		Assert.Equal(array[1], values[1]);
		Assert.Equal(array[2], values[2]);
	}

	[Fact]
	public void GetNodes()
	{
		const string json = """
			{
				"Aaa": "Bbb",
				"Bbb":
				{
					"Ccc": 111,
					"Ddd": false,
					"Eee": [1, 2, 3, 4, 5, 6]
				},
				"ObjA":
				{
					"Obj1":
					{
						"Ccc": 111,
						"Ddd": true,
						"Eee": [1, 2, 3, 4, 5, 6]
					}
				},
				"ObjB":
				{
					"Obj2":
					{
						"Ccc": 111,
						"Ddd": false,
						"Eee": [1, 2, 3, 4, 5, 6]
					}
				},
				"Fff":
				[
					[
						99999.66,
						{
							"Ggg": 222,
							"Hhh": true,
							"Iii":
							{
								"Jjj": "Lll"
							}
						}
					]
				]
			}
			""";

		var root = JsonSerializer.Deserialize<JsonNode>(json);

		var nodes = root.GetNodes("$.Fff.*.*.Iii.Jjj");
		Assert.Equal("Lll", nodes.Single().AsValue().GetValue<string>());

		var range = "4".ToRange();
		nodes = root.GetNodes("$.Bbb.Eee[4]");
		Assert.Equal(5, nodes.Single().AsValue().GetValue<int>());

		nodes = root.GetNodes("$.Aaa.Bbb.Ccc");
		Assert.Empty(nodes);

		nodes = root.GetNodes("$['Aaa']['Bbb']['Ccc']");
		Assert.Empty(nodes);

		nodes = root.GetNodes("$.Fff.*.*.Hhh");
		Assert.True(nodes.Single().AsValue().GetValue<bool>());

		nodes = root.GetNodes("$['Fff']['*']['*']['Hhh']");
		Assert.True(nodes.Single().AsValue().GetValue<bool>());

		nodes = root.GetNodes("$.Fff[0][1].Hhh");
		Assert.True(nodes.Single().AsValue().GetValue<bool>());

		nodes = root.GetNodes("$.Fff[0][1].Ggg");
		Assert.Equal(222, nodes.Single().AsValue().GetValue<int>());

		nodes = root.GetNodes("$.Fff[0][0]");
		Assert.Equal(99999.66M, nodes.Single().AsValue().GetValue<decimal>());

		nodes = root.GetNodes("$.*.Obj1.Ddd");
		Assert.True(nodes.Single().AsValue().GetValue<bool>());

		nodes = root.GetNodes("$.*.*.Ddd");
		Assert.True(nodes.First().AsValue().GetValue<bool>());
		Assert.False(nodes.Last().AsValue().GetValue<bool>());

		nodes = root.GetNodes("$.Fff[99999999999999]");
		Assert.Empty(nodes);

		nodes = root.GetNodes(null);
		Assert.Empty(nodes);

		nodes = root.GetNodes("   ");
		Assert.Empty(nodes);

		nodes = root.GetNodes("Aaa.Bbb.Ccc");
		Assert.Empty(nodes);

		nodes = root.GetNodes("['Fff']['Hhh']");
		Assert.Empty(nodes);

		root = JsonSerializer.Deserialize<JsonNode>("[1, 2, 3, 4, 5, 6, 7, 8]");
		nodes = root.GetNodes("$[4]");
		Assert.Equal(5, nodes.Single().AsValue().GetValue<int>());

		nodes = root.GetNodes("$.[7]");
		Assert.Equal(8, nodes.Single().AsValue().GetValue<int>());
	}

	[Fact]
	public void GetObjectElements()
	{
		var objectJson = JsonSerializer.Deserialize<JsonElement>(@"{""a"": 1, ""B"": 2, ""c"": 3, ""D"": 4, ""e"": 5, ""F"": 6}");
		var jsonElements = objectJson.GetObjectElements();

		Assert.Equal(6, jsonElements.Count);
		Assert.Equal(1, jsonElements["a"].GetInt32());
		Assert.Equal(2, jsonElements["B"].GetInt32());
		Assert.Equal(3, jsonElements["c"].GetInt32());
		Assert.Equal(4, jsonElements["D"].GetInt32());
		Assert.Equal(5, jsonElements["e"].GetInt32());
		Assert.Equal(6, jsonElements["F"].GetInt32());
	}

	[Fact]
	public void GetObjectValues()
	{
		var objectJson = JsonSerializer.Deserialize<JsonElement>(@"{""a"": 1, ""B"": 2, ""c"": 3, ""D"": 4, ""e"": 5, ""F"": 6}");
		var values = objectJson.GetObjectValues();

		Assert.Equal(6, values.Count);
		Assert.Equal(1, (int)values["a"]);
		Assert.Equal(2, (int)values["B"]);
		Assert.Equal(3, (int)values["c"]);
		Assert.Equal(4, (int)values["D"]);
		Assert.Equal(5, (int)values["e"]);
		Assert.Equal(6, (int)values["F"]);
	}

	[Fact]
	public void GetValue()
	{
		Assert.Equal(true, JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(true)).GetValue());
		Assert.Equal(false, JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(false)).GetValue());
		Assert.Equal(1.234M, JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(1.234M)).GetValue());
		Assert.Equal(long.MinValue, JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(long.MinValue)).GetValue());
		Assert.Equal(int.MaxValue, JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(int.MaxValue)).GetValue());

		var nowOffset = DateTimeOffset.Now.ToString();
		Assert.Equal(nowOffset, JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(nowOffset)).GetValue());

		var now = DateTime.Now;
		Assert.Equal(now, JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(now)).GetValue());

		var guid = Guid.NewGuid();
		Assert.Equal(guid, JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(guid)).GetValue());
		Assert.Equal("AbC123", JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize("AbC123")).GetValue());
		Assert.Equal("123456", JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize("123456")).GetValue());
		Assert.Equal(new[] { 1, 2, 3 }, JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize<int[]>([1, 2, 3])).GetValue());

		var dictionary = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(new { A = 1, B = "2", C = false })).GetValue();
		Assert.IsType<Dictionary<string, object>>(dictionary);
		Assert.Equal(3, ((Dictionary<string, object>)dictionary).Count);
	}
}
