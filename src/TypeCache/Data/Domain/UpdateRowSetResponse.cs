// Copyright (c) 2021 Samuel Abraham

using TypeCache.Collections;

namespace TypeCache.Data.Domain;

/// <summary>
/// <code>
/// {<br/>
///	<see langword="    "/>"Columns": [ "Column1", "Column2", "Column3", ... ],<br/>
///	<see langword="    "/>"Count": 6000,<br/>
///	<see langword="    "/>"Deleted": [ [ "Data", 123, null ], [ ... ], ... ],<br/>
///	<see langword="    "/>"Inserted": [ [ "Data", 456, '01-01-2020' ], [ ... ], ... ]
/// }
/// </code>
/// </summary>
public struct UpdateRowSetResponse<T>
{
	public UpdateRowSetResponse()
	{
		this.Columns = Array<string>.Empty;
		this.Count = 0;
		this.Deleted = Array<T>.Empty;
		this.Inserted = Array<T>.Empty;
	}

	public string[] Columns { get; set; }

	public long Count { get; set; }

	public T[] Deleted { get; set; }

	public T[] Inserted { get; set; }
}
