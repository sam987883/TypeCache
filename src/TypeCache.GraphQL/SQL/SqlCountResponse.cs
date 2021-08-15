using GraphQL.Types.Relay.DataObjects;
using TypeCache.Collections;
using TypeCache.GraphQL.Types;

namespace TypeCache.GraphQL.SQL
{
	public class SqlCountResponse
	{
		public long Count { get; set; }

		public string? SQL { get; set; }

		public string? Table { get; set; }
	}
}
