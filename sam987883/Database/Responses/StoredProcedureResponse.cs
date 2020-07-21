// Copyright (c) 2020 Samuel Abraham

using System.Collections.Generic;

namespace sam987883.Database.Responses
{
	public class StoredProcedureResponse
	{
		public IEnumerable<RowSet> Output { get; set; } = new RowSet[0];

		public IDictionary<string, object?>? Parameters { get; set; } = null;
	}
}