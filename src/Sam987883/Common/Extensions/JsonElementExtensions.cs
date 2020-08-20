using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Sam987883.Common.Extensions
{
	public static class JsonElementExtensions
	{
		public static IEnumerable<JsonElement> GetArrayValues(this JsonElement json)
		{
			using var enumerator = json.EnumerateArray();
			while (enumerator.MoveNext())
			{
				yield return enumerator.Current;
			}
		}

		public static IDictionary<string, JsonElement> GetObjectValues(this JsonElement json)
		{
			var properties = new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);
			using var enumerator = json.EnumerateObject();
			while (enumerator.MoveNext())
			{
				properties.Add(enumerator.Current.Name, enumerator.Current.Value);
			}
			return properties;
		}
	}
}
