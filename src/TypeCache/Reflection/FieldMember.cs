﻿// Copyright (c) 2021 Samuel Abraham

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
			Cache = new LazyDictionary<(RuntimeFieldHandle, RuntimeTypeHandle), FieldMember>(CreateFieldMember);

			static FieldMember CreateFieldMember((RuntimeFieldHandle FieldHandle, RuntimeTypeHandle TypeHandle) handle)
				=> new FieldMember((FieldInfo)handle.TypeHandle.ToFieldInfo(handle.FieldHandle)!);
		}

		internal static IReadOnlyDictionary<(RuntimeFieldHandle, RuntimeTypeHandle), FieldMember> Cache { get; }

		internal FieldMember(FieldInfo fieldInfo)
			: base(fieldInfo)
		{
			this.FieldType = fieldInfo.FieldType.GetTypeMember();
			this.Handle = fieldInfo.FieldHandle;
			this.Static = fieldInfo.IsStatic;
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

		public new TypeMember Type => base.Type!;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator FieldInfo(FieldMember member)
			=> member.Type!.Handle.ToFieldInfo(member.Handle);

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
	}
}
