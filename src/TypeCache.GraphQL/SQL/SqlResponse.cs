using TypeCache.Collections;

namespace TypeCache.GraphQL.SQL
{
	public class SqlResponse<T>
	{
		public T[]? Data { get; set; }

		public string? SQL { get; set; }

		public string? Table { get; set; }
	}
}
