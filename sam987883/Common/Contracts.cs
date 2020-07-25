// Copyright (c) 2020 Samuel Abraham

using sam987883.Common.Models;
using System.Collections.Generic;

namespace sam987883.Common
{
	public interface IReferenceEqualityComparer<in T> : IEqualityComparer<T>
	{
	}

	public interface IRowSetConverter<T>
		where T : class, new()
	{
		RowSet ToRowSet(T[] items, params string[] columns);
		T[] FromRowSet(RowSet rowSet);
	}
}
