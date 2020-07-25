// Copyright (c) 2020 Samuel Abraham

using sam987883.Reflection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using static sam987883.Common.Extensions.ExpressionExtensions;

namespace sam987883.Common
{
	public static class Factory
	{
		static class Anonymous<T>
			where T : class
		{
			public static IFieldCache<T> FieldCache { get; } = new FieldCache<T>();
		}

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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> Empty<T>()
		{
			yield break;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IFieldAccessor<T> CreateFieldAccessor<T>(T anonymousInstance)
			where T : class => Anonymous<T>.FieldCache.CreateAccessor(anonymousInstance);
	}
}