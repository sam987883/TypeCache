// Copyright (c) 2021 Samuel Abraham

using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection;

public static class Unsafe
{
	private static class Converter<FROM, TO>
		where FROM : unmanaged
		where TO : unmanaged
	{
		public delegate TO Convert(FROM value);

		static Converter()
		{
			ParameterExpression value = nameof(value).Parameter<FROM>();
			To = value.Cast<TO>().Lambda<Convert>(value).Compile();
		}

		public static Convert To { get; }
	}

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static TO Convert<FROM, TO>(FROM value)
		where FROM : unmanaged
		where TO : unmanaged
		=> Converter<FROM, TO>.To(value);
}
