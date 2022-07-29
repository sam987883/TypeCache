// Copyright (c) 2021 Samuel Abraham

using TypeCache.Collections;

namespace TypeCache.Data.Domain;

/// <summary>
/// <code>
/// {<br/>
///	<see langword="    "/>"Columns": [ "Column1", "Column2", "Column3", ... ],<br/>
///	<see langword="    "/>"Count": 6000,<br/>
///	<see langword="    "/>"Rows": [ Model or JObject or Dictionary&lt;<see cref="string"/>, <see cref="object"/>&gt; or ValueTuple ]<br/>
/// }
/// </code>
/// </summary>
public class RowSetResponse<T>
{
	public RowSetResponse()
	{
		this.Columns = Array<string>.Empty;
		this.Count = 0;
		this.Rows = Array<T>.Empty;
	}

	public string[] Columns { get; set; }

	public long Count { get; set; }

	public T[] Rows { get; set; }

	public T this[int row] => this.Rows[row];
}
