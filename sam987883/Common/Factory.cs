// Copyright (c) 2020 Samuel Abraham

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using static sam987883.Common.Extensions.ExpressionExtensions;

namespace sam987883.Common
{
	public static class Factory
	{
		static class Instance<T>
			where T : class, new()
		{
			public static Func<T> Create { get; } = typeof(T).New().Lambda<Func<T>>().Compile();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Create<T>() where T : class, new() =>
			Instance<T>.Create();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static StringWriter CreateStringWriter(Encoding encoding) =>
			new CustomStringWriter(encoding);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static StringWriter CreateStringWriter(StringBuilder builder, Encoding encoding) =>
			new CustomStringWriter(builder, encoding);
	}
}