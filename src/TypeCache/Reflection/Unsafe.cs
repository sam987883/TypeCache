// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection
{
	public static class Unsafe
	{
		private static class Converter<FROM, TO>
		{
			public delegate ref TO Convert(ref FROM value);

			static Converter()
			{
				ParameterExpression value = nameof(value).Parameter<FROM>();
				To = value.Cast<TO>().Lambda<Convert>(value).Compile();
			}

			public static Convert To { get; }
		}

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public static ref TO Convert<FROM, TO>(ref FROM value) =>
			ref Converter<FROM, TO>.To(ref value);
	}
}
