// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections.Generic;

namespace Sam987883.Reflection
{
	public interface IEnumComparer<T> : IComparer<T> where T : struct, Enum
	{
	}
}
