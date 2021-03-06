﻿// Copyright (c) 2021 Samuel Abraham

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Reflection
{
	public sealed class PropertyMember
		: Member, IEquatable<PropertyMember>
	{
		internal PropertyMember(PropertyInfo propertyInfo)
			: base(propertyInfo)
		{
			this.PropertyType = propertyInfo.PropertyType.GetTypeMember();
			this.Indexer = propertyInfo.GetIndexParameters().Any();
			this.Getter = propertyInfo.GetMethod?.MethodHandle.GetMethodMember(this.Type.Handle);
			this.Setter = propertyInfo.SetMethod?.MethodHandle.GetMethodMember(this.Type.Handle);

			this._Handle = this.Getter?.Handle ?? this.Setter!.Handle;
		}

		private readonly RuntimeMethodHandle _Handle;

		public bool Indexer { get; }

		public TypeMember PropertyType { get; }

		public MethodMember? Getter { get; }

		public MethodMember? Setter { get; }

		public new TypeMember Type => base.Type!;

		/// <param name="instance">Pass null if the property getter is static.</param>
		/// <param name="indexers">Ignore if property is not an indexer.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(PropertyMember? other)
			=> this._Handle == other?._Handle;
	}
}
