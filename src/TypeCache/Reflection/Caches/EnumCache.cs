// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Reflection;
using TypeCache.Common;
using TypeCache.Extensions;
using TypeCache.Reflection.Members;

namespace TypeCache.Reflection.Caches
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
