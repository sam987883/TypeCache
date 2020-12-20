// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;

namespace TypeCache.Reflection
{
	public interface IEnumComparer<T> : IComparer<T> where T : struct, Enum
	{
	}
}
