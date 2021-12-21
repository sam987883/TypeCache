// Copyright (c) 2021 Samuel Abraham

using System;
using System.Text.Json;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class JsonExtensions
{
	[Fact]
	public void GetArrayElements()
	{
		var arrayJson = JsonSerializer.Deserialize<JsonElement>(@$"[null,true,3,{long.MaxValue},{ulong.MaxValue},""{DateTime.UtcNow.ToShortDateString()}"",""{Guid.NewGuid().ToString("D")}"",""Word""]");

		Assert.Equal(8, arrayJson.GetArrayValues().Length);
	}

	[Fact]
	public void GetArrayValues()
	{
		var arrayJson = JsonSerializer.Deserialize<JsonElement>(@$"[null,true,3,{long.MaxValue},{ulong.MaxValue},""{DateTime.UtcNow.ToShortDateString()}"",""{Guid.NewGuid().ToString("D")}"",""Word""]");

		Assert.Equal(8, arrayJson.GetArrayElements().Length);
	}

	[Fact]
	public void GetObjectElements()
	{
		var objectJson = JsonSerializer.Deserialize<JsonElement>(@"{""a"": 1, ""B"": 2, ""c"": 3, ""D"": 4, ""e"": 5, ""F"": 6}");

		Assert.Equal(6, objectJson.GetObjectElements().Count);
	}

	[Fact]
	public void GetObjectValues()
	{
		var objectJson = JsonSerializer.Deserialize<JsonElement>(@"{""a"": 1, ""B"": 2, ""c"": 3, ""D"": 4, ""e"": 5, ""F"": 6}");

		Assert.Equal(6, objectJson.GetObjectValues().Count);
	}
}
