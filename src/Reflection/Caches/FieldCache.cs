// Copyright (c) 2020 Samuel Abraham

using Sam987883.Common;
using Sam987883.Reflection.Members;
using System.Collections.Generic;
using System.Collections.Immutable;
using static Sam987883.Common.Extensions.IEnumerableExtensions;

namespace Sam987883.Reflection.Caches
{
	internal sealed class FieldCache<T> : IFieldCache<T>
		where T : class
	{
		public FieldCache()
		{
			var valueComparer = new CustomEqualityComparer<IFieldMember<T>>((x, y) => TypeCache.NameComparer.Equals(x.Name, y.Name));
			this.Fields = typeof(T).GetFields(TypeCache.INSTANCE_BINDING)
				.If(fieldInfo => !fieldInfo.IsLiteral)
				.To(fieldInfo => KeyValuePair.Create(fieldInfo.Name, (IFieldMember<T>)new FieldMember<T>(fieldInfo)))
				.ToImmutable(TypeCache.NameComparer, valueComparer);
			this.GetNames = this.Fields
				.If(_ => _.Value.Getter != null)
				.To(_ => _.Value.Name)
				.ToImmutableArray();
			this.SetNames = this.Fields
				.If(_ => _.Value.Setter != null)
				.To(_ => _.Value.Name)
				.ToImmutableArray();
		}

		public IImmutableDictionary<string, IFieldMember<T>> Fields { get; }

		public IImmutableList<string> GetNames { get; }

		public IImmutableList<string> SetNames { get; }

		public void Map(T from, T to) =>
			this.Fields.Values
				.If(field => field.GetValue != null && field.SetValue != null)
				.Do(field => field[to] = field[from]);
	}
}
