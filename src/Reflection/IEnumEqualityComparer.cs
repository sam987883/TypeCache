// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections.Generic;

namespace Sam987883.Reflection
{
	public interface IEnumEqualityComparer<T> : IEqualityComparer<T> where T : struct, Enum
	{
	}
}
