using System;

namespace sam987883.Reflection
{
	public static class TypeOf<T>
	{
		public static RuntimeTypeHandle TypeHandle { get; } = typeof(T).TypeHandle;
	}
}
