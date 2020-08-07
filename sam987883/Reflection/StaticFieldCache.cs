// Copyright (c) 2020 Samuel Abraham

using sam987883.Common;
using sam987883.Reflection.Members;
using System.Collections.Generic;
using System.Collections.Immutable;
using static sam987883.Common.Extensions.IEnumerableExtensions;

namespace sam987883.Reflection
{
	internal sealed class StaticFieldCache<T> : IStaticFieldCache<T>
		where T : class
	{
		public StaticFieldCache()
		{
			var valueComparer = new CustomEqualityComparer<IStaticFieldMember>((x, y) => TypeCache.NameComparer.Equals(x.Name, y.Name));
			this.Fields = typeof(T).GetFields(TypeCache.STATIC_BINDING)
				.If(fieldInfo => !fieldInfo.IsLiteral)
				.To(fieldInfo => KeyValuePair.Create(fieldInfo.Name, (IStaticFieldMember)new StaticFieldMember(fieldInfo)))
				.ToImmutable(TypeCache.NameComparer, valueComparer);
		}

		public IImmutableDictionary<string, IStaticFieldMember> Fields { get; }
	}
}
