// Copyright (c) 2021 Samuel Abraham

using System.Runtime.CompilerServices;

namespace TypeCache.Extensions;

public static class TupleExtensions
{
	public static object?[] ToArray(this ITuple @this)
	{
		var array = new object?[@this.Length];
		for (var i = 0; i < @this.Length; ++i)
			array[i] = @this[i];

		return array;
	}
}
