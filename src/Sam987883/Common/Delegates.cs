using System.Diagnostics.CodeAnalysis;

namespace Sam987883.Common
{
	public delegate int CompareFunc<in T>([AllowNull] T x, [AllowNull] T y);
	public delegate bool EqualsFunc<in T>([AllowNull] T x, [AllowNull] T y);
}
