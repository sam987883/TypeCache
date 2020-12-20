// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using TypeCache.Common;
using TypeCache.Extensions;
using TypeCache.Reflection.Accessors;
using TypeCache.Reflection.Members;

namespace TypeCache.Reflection.Caches
{
	internal sealed class FieldCache<T> : IFieldCache<T>
	{
		public FieldCache()
		{
			var valueComparer = new CustomEqualityComparer<IFieldMember>((x, y) => TypeCache.NameComparer.Equals(x.Name, y.Name));
			this.Fields = typeof(T).GetFields(TypeCache.INSTANCE_BINDING)
				.If(fieldInfo => !fieldInfo.IsLiteral)
				.To(fieldInfo => KeyValuePair.Create(fieldInfo.Name, (IFieldMember)new FieldMember(fieldInfo)))
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

		public IImmutableDictionary<string, IFieldMember> Fields { get; }

		public IImmutableList<string> GetNames { get; }

		public IImmutableList<string> SetNames { get; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IMemberAccessor CreateAccessor(object instance)
			=> new FieldAccessor((IFieldCache<object>)(IFieldCache<T>)this, instance);

		public void Map(object from, object to)
			=> this.Fields.Values
				.If(field => field.GetValue != null && field.SetValue != null)
				.Do(field => field[to] = field[from]);

		public void Map(IDictionary<string, object> from, object to, IEqualityComparer<string>? comparer = null)
		{
			comparer ??= TypeCache.NameComparer;

			from.If(pair => this.SetNames.Has(pair.Key, comparer)).Do(pair => this.Fields[pair.Key][to] = pair.Value);
		}

		public void Map(object from, IDictionary<string, object?> to, IEqualityComparer<string>? comparer = null)
		{
			comparer ??= TypeCache.NameComparer;

			to.Keys.If(name => this.GetNames.Has(name, comparer)).Do(name => to[name] = this.Fields[name][from]);
		}
	}
}
