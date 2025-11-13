// Copyright (c) 2021 Samuel Abraham

using System.Runtime.CompilerServices;

namespace TypeCache.Extensions;

public static class TupleExtensions
{
	extension(ITuple @this)
	{
		public object?[] ToArray()
		{
			var array = new object?[@this.Length];
			for (var i = 0; i < @this.Length; ++i)
				array[i] = @this[i];

			return array;
		}
	}
}
