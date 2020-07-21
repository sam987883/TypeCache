// Copyright (c) 2020 Samuel Abraham

using System.Collections.Generic;

namespace sam987883.Database.Requests
{
	public class StoredProcedureRequest
	{
		public string Procedure { get; set; } = string.Empty;

		public IDictionary<string, object?>? Parameters { get; set; } = null;
	}
}