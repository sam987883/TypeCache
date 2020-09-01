// Copyright (c) 2020 Samuel Abraham

using Sam987883.Common.Models;

namespace Sam987883.Reflection
{
	public interface IRowSetConverter<T>
		where T : class, new()
	{
		RowSet ToRowSet(T[] items, params string[] columns);
		T[] FromRowSet(RowSet rowSet);
	}
}
