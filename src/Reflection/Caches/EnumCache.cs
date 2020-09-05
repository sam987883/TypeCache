// Copyright (c) 2020 Samuel Abraham

using Sam987883.Reflection.Extensions;
using Sam987883.Reflection.Members;
using System;
using System.Collections.Immutable;
using System.Reflection;
using static Sam987883.Common.Extensions.EnumExtensions;
using static Sam987883.Common.Extensions.IEnumerableExtensions;

namespace Sam987883.Reflection.Caches
{
	internal sealed class EnumCache<T> : Member, IEnumCache<T>
		where T : struct, Enum
	{
		private EnumCache() : base(typeof(T))
		{
			var type = typeof(T);
			var underlyingType = type.GetEnumUnderlyingType();

			this.Flags = type.GetCustomAttribute<FlagsAttribute>() != null;
			this.UnderlyingType = underlyingType.ToNativeType();
			this.UnderlyingTypeHandle = underlyingType.TypeHandle;

			var values = (T[])type.GetEnumValues();
			var arrayBuilder = ImmutableArray.CreateBuilder<IEnumMember<T>>(values.Length);
			arrayBuilder.AddRange(values.To(value => new EnumMember<T>(type.GetField(value.Name(), TypeCache.STATIC_BINDING), value)));
			this.Fields = arrayBuilder.ToImmutable();
		}

		public IImmutableList<IEnumMember<T>> Fields { get; }

		public bool Flags { get; }

		public NativeType UnderlyingType { get; }

		public RuntimeTypeHandle UnderlyingTypeHandle { get; }
	}
}
