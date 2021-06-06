using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeCache.GraphQL.Extensions
{
	public class ConnectionSelections
	{
		public bool TotalCount { get; set; }
		public bool HasNextPage { get; set; }
		public bool HasPreviousPage { get; set; }
		public bool Cursor { get; set; }
		public bool StartCursor { get; set; }
		public bool EndCursor { get; set; }
		public string[] EdgeNodeFields { get; set; } = Array.Empty<string>();
		public string[] ItemFields { get; set; } = Array.Empty<string>();
	}
}
