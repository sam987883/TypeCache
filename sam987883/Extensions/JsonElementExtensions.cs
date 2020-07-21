using System.Collections.Generic;
using System.Text.Json;

namespace sam987883.Extensions
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
	}
}
