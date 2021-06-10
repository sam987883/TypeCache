// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Reflection
{
	public sealed class FieldMember
		: Member, IEquatable<FieldMember>
	{
		static FieldMember()
		{
			Cache = new LazyDictionary<RuntimeFieldHandle, FieldMember>(handle => new FieldMember(handle.ToFieldInfo()));
		}

		internal static IReadOnlyDictionary<RuntimeFieldHandle, FieldMember> Cache { get; }

		internal FieldMember(FieldInfo fieldInfo)
			: base(fieldInfo, fieldInfo.IsAssembly, fieldInfo.IsPublic)
		{
			this.FieldType = fieldInfo.FieldType.GetTypeMember();
			this.Handle = fieldInfo.FieldHandle;
			this.Static = fieldInfo.IsStatic;
			this.Type = fieldInfo.GetTypeMember();
			this.Getter = fieldInfo.ToGetter();
			this._GetValue = fieldInfo.ToGetValue();

			if (!fieldInfo.IsInitOnly && !fieldInfo.IsLiteral)
			{
				this.Setter = fieldInfo.ToSetter();
				this._SetValue = fieldInfo.ToSetValue();
			}
		}

		private readonly GetValue? _GetValue;

		private readonly SetValue? _SetValue;

		public TypeMember FieldType { get; }

		public RuntimeFieldHandle Handle { get; }

		public Delegate? Getter { get; }

		public Delegate? Setter { get; }

		public bool Static { get; }

		public TypeMember Type { get; }

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
		public bool Equals(FieldMember? other)
			=> this.Handle == other?.Handle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> this.Handle.GetHashCode();
	}
}
