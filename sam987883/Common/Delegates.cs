using System.Diagnostics.CodeAnalysis;

namespace sam987883.Common
{
	public delegate int CompareFunc<in T>([AllowNull] T x, [AllowNull] T y);
	public delegate bool EqualsFunc<in T>([AllowNull] T x, [AllowNull] T y);
	public delegate void RefAction<T>(ref T item);
	public delegate void RefAction<T, in I>(ref T item, I index);
}
