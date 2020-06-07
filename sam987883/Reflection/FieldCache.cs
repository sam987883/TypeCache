// Copyright (c) 2020 Samuel Abraham

using sam987883.Common;
using sam987883.Reflection.Members;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using static sam987883.Extensions.IEnumerableExtensions;

namespace sam987883.Reflection
{
	internal sealed class FieldCache<T> : IFieldCache<T>
	{
		public FieldCache()
		{
			var valueComparer = new CustomEqualityComparer<IFieldMember<T>>((x, y) => TypeCache.NameComparer.Equals(x.Name, y.Name));
			this.Fields = typeof(T).GetFields(TypeCache.INSTANCE_BINDING)
				.If(fieldInfo => !fieldInfo.IsLiteral)
				.To(fieldInfo => KeyValuePair.Create(fieldInfo.Name, (IFieldMember<T>)new FieldMember<T>(fieldInfo)))
				.ToImmutable(TypeCache.NameComparer, valueComparer);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IFieldAccessor<T> CreateAccessor(T instance) =>
			new FieldAccessor<T>(this, instance);

		public IImmutableDictionary<string, IFieldMember<T>> Fields { get; }

		public void Map(T from, T to) =>
			this.Fields.Values
				.If(field => field.GetValue != null && field.SetValue != null)
				.Do(field => field[to] = field[from]);
	}
}
