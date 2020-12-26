// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Text.Json;

namespace TypeCache.Extensions
{
	public static class JsonElementExtensions
	{
		public static IEnumerable<JsonElement> GetArrayValues(this JsonElement @this)
		{
			@this.ValueKind.Assert(nameof(GetArrayValues), JsonValueKind.Array);

			using var enumerator = @this.EnumerateArray();
			while (enumerator.MoveNext())
			{
				yield return enumerator.Current;
			}
		}

		public static IDictionary<string, JsonElement> GetObjectValues(this JsonElement @this)
		{
			@this.ValueKind.Assert(nameof(GetObjectValues), JsonValueKind.Object);

			var properties = new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);
			using var enumerator = @this.EnumerateObject();
			while (enumerator.MoveNext())
			{
				properties.Add(enumerator.Current.Name, enumerator.Current.Value);
			}
			return properties;
		}
	}
}
