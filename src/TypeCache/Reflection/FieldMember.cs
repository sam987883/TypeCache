// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Reflection
{
	public readonly struct FieldMember : IMember, IEquatable<FieldMember>
	{
		static FieldMember()
		{
			Cache = new LazyDictionary<(RuntimeFieldHandle, RuntimeTypeHandle), FieldMember>(CreateFieldMember);

			static FieldMember CreateFieldMember((RuntimeFieldHandle FieldHandle, RuntimeTypeHandle TypeHandle) handle)
				=> new FieldMember((FieldInfo)handle.TypeHandle.ToFieldInfo(handle.FieldHandle)!);
		}

		internal static IReadOnlyDictionary<(RuntimeFieldHandle, RuntimeTypeHandle), FieldMember> Cache { get; }

		internal FieldMember(FieldInfo fieldInfo)
		{
			this.Attributes = fieldInfo.GetCustomAttributes<Attribute>()?.ToImmutableArray() ?? ImmutableArray<Attribute>.Empty;
			this.Name = this.Attributes.First<NameAttribute>()?.Name ?? fieldInfo.Name;
			this.FieldType = fieldInfo.FieldType.GetTypeMember();
			this.Internal = fieldInfo.IsAssembly;
			this.Handle = fieldInfo.FieldHandle;
			this.Public = fieldInfo.IsPublic;
			this.Static = fieldInfo.IsStatic;
			this.Type = fieldInfo.GetTypeMember();

			this.Getter = fieldInfo.ToGetter();
			this._GetValue = fieldInfo.ToGetValue();

			var canSet = !fieldInfo.IsInitOnly && !fieldInfo.IsLiteral;
			this.Setter = canSet ? fieldInfo.ToSetter() : null;
			this._SetValue = canSet ? fieldInfo.ToSetValue() : null;
		}

		private readonly GetValue? _GetValue;

		private readonly SetValue? _SetValue;

		public TypeMember Type { get; }

		public IImmutableList<Attribute> Attributes { get; }

		public string Name { get; }

		public TypeMember FieldType { get; }

		public RuntimeFieldHandle Handle { get; }

		public Delegate? Getter { get; }

		public Delegate? Setter { get; }

		public bool Internal { get; }

		public bool Public { get; }

		public bool Static { get; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator FieldInfo(FieldMember member)
			=> member.Type.Handle.ToFieldInfo(member.Handle);

		/// <param name="instance">Pass null if the field is static.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public object? GetValue(object? instance)
			=> this._GetValue?.Invoke(instance);

		/// <param name="instance">Pass null if the field is static.</param>
		/// <param name="value">The value to set the property to.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetValue(object? instance, object? value)
			=> this._SetValue?.Invoke(instance, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(FieldMember other)
			=> this.Handle == other.Handle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> this.Handle.GetHashCode();
	}
}
