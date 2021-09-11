// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection
{
	public readonly struct PropertyMember
		: IMember, IEquatable<PropertyMember>
	{
		internal PropertyMember(PropertyInfo propertyInfo)
		{
			this.Type = propertyInfo.GetTypeMember();
			this.Attributes = propertyInfo.GetCustomAttributes<Attribute>()?.ToImmutableArray() ?? ImmutableArray<Attribute>.Empty;
			this.Name = this.Attributes.First<NameAttribute>()?.Name ?? propertyInfo.Name;
			this.PropertyType = propertyInfo.PropertyType.GetTypeMember();
			this.Indexer = propertyInfo.GetIndexParameters().Any();
			this.Getter = propertyInfo.GetMethod?.MethodHandle.GetMethodMember(this.Type.Handle);
			this.Setter = propertyInfo.SetMethod?.MethodHandle.GetMethodMember(this.Type.Handle);

			var accessor = propertyInfo.GetAccessors(true).First()!;
			this.Internal = accessor.IsAssembly;
			this.Public = accessor.IsPublic;

			this._Handle = accessor.MethodHandle;
		}

		private readonly RuntimeMethodHandle _Handle;

		public TypeMember Type { get; }

		public IImmutableList<Attribute> Attributes { get; }

		public string Name { get; }

		public bool Indexer { get; }

		public TypeMember PropertyType { get; }

		public MethodMember? Getter { get; }

		public MethodMember? Setter { get; }

		public bool Internal { get; }

		public bool Public { get; }

		/// <param name="instance">Pass null if the property getter is static.</param>
		/// <param name="indexers">Ignore if property is not an indexer.</param>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public object? GetValue(object? instance, params object?[]? indexers)
			=> this.Getter?.Invoke(instance, indexers);

		/// <param name="instance">Pass null if the property setter is static.</param>
		/// <param name="value">The value to set the property to.</param>
		/// <param name="indexers">Ignore if property is not an indexer.</param>
		public void SetValue(object? instance, object? value, params object?[]? indexers)
		{
			if (this.Indexer)
				this.Setter?.Invoke(instance, indexers.And(value).ToArray());
			else
				this.Setter?.Invoke(instance, value);
		}

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public bool Equals(PropertyMember other)
			=> this._Handle == other._Handle;

		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public override int GetHashCode()
			=> this._Handle.GetHashCode();
	}
}
