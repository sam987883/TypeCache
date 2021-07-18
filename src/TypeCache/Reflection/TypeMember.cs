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
	public readonly struct TypeMember : IMember, IEquatable<TypeMember>
	{
		static TypeMember()
		{
			Cache = new LazyDictionary<RuntimeTypeHandle, TypeMember>(typeHandle => new TypeMember(typeHandle.ToType()));
		}

		internal static IReadOnlyDictionary<RuntimeTypeHandle, TypeMember> Cache { get; }

		internal TypeMember(Type type)
		{
			var kind = type.GetKind();
			var systemType = type.GetSystemType();

			this.Attributes = type.GetCustomAttributes<Attribute>()?.ToImmutableArray() ?? ImmutableArray<Attribute>.Empty;
			this.Name = this.Attributes.First<NameAttribute>()?.Name ?? type.Name;
			this.Handle = type.TypeHandle;
			this.Kind = kind;
			this.SystemType = systemType;
			this.Ref = type.IsByRef || type.IsByRefLike;
			this.Internal = !type.IsVisible;
			this.Public = type.IsPublic;

			this._BaseType = new Lazy<TypeMember>(() => kind switch
			{
				Kind.Enum => typeof(Enum).GetTypeMember(),
				Kind.Struct or Kind.Pointer => typeof(ValueType).GetTypeMember(),
				_ when type.BaseType is not null => type.BaseType.GetTypeMember(),
				_ => typeof(object).GetTypeMember()
			}, false);
			this._EnclosedType = new Lazy<TypeMember?>(() => systemType switch
			{
				_ when type.HasElementType => type.GetElementType()?.GetTypeMember(),
				SystemType.Dictionary or SystemType.ImmutableDictionary or SystemType.ImmutableSortedDictionary or SystemType.SortedDictionary
					=> typeof(KeyValuePair<,>).MakeGenericType(type.GenericTypeArguments).GetTypeMember(),
				_ when type.GenericTypeArguments.Length == 1 => type.GenericTypeArguments[0].GetTypeMember(),
				_ => null
			}, false);
			this._GenericTypes = new Lazy<IImmutableList<TypeMember>>(() => type.GenericTypeArguments.To(_ => _.GetTypeMember()).ToImmutableArray(), false);
			this._InterfaceTypes = new Lazy<IImmutableList<TypeMember>>(() => type.GetInterfaces().To(_ => _.GetTypeMember()).ToImmutableArray(), false);

			this._Constructors = new Lazy<IImmutableList<ConstructorMember>>(() => type.TypeHandle.CreateConstructorMembers().ToImmutableArray(), false);
			this._Events = new Lazy<IImmutableDictionary<string, EventMember>>(() => type.TypeHandle.CreateEventMembers().ToImmutableDictionary(), false);
			this._Fields = this.Kind switch
			{
				Kind.Delegate => new Lazy<IImmutableDictionary<string, FieldMember>>(() => ImmutableDictionary<string, FieldMember>.Empty, false),
				_ => new Lazy<IImmutableDictionary<string, FieldMember>>(() => type.TypeHandle.CreateFieldMembers().ToImmutableDictionary(), false)
			};
			this._Methods = new Lazy<IImmutableDictionary<string, IImmutableList<MethodMember>>>(() =>
				type.TypeHandle.CreateMethodMembers().ToDictionary(pair => pair.Key, pair => (IImmutableList<MethodMember>)pair.Value.ToImmutableArray()).ToImmutableDictionary(), false);
			this._Properties = new Lazy<IImmutableDictionary<string, PropertyMember>>(() => type.TypeHandle.CreatePropertyMembers().ToImmutableDictionary(), false);
		}

		private readonly Lazy<TypeMember> _BaseType;

		private readonly Lazy<TypeMember?> _EnclosedType;

		private readonly Lazy<IImmutableList<TypeMember>> _GenericTypes;

		private readonly Lazy<IImmutableList<TypeMember>> _InterfaceTypes;

		private readonly Lazy<IImmutableList<ConstructorMember>> _Constructors;

		private readonly Lazy<IImmutableDictionary<string, EventMember>> _Events;

		private readonly Lazy<IImmutableDictionary<string, FieldMember>> _Fields;

		private readonly Lazy<IImmutableDictionary<string, IImmutableList<MethodMember>>> _Methods;

		private readonly Lazy<IImmutableDictionary<string, PropertyMember>> _Properties;

		public IImmutableList<Attribute> Attributes { get; }

		public string Name { get; }

		public IImmutableList<ConstructorMember> Constructors => this._Constructors.Value;

		public IImmutableDictionary<string, EventMember> Events => this._Events.Value;

		public IImmutableDictionary<string, FieldMember> Fields => this._Fields.Value;

		public IImmutableDictionary<string, IImmutableList<MethodMember>> Methods => this._Methods.Value;

		public IImmutableDictionary<string, PropertyMember> Properties => this._Properties.Value;

		public TypeMember BaseType => this._BaseType.Value;

		public TypeMember? EnclosedType => this._EnclosedType.Value;

		public IImmutableList<TypeMember> GenericTypes => this._GenericTypes.Value;

		public RuntimeTypeHandle Handle { get; }

		public IImmutableList<TypeMember> InterfaceTypes => this._InterfaceTypes.Value;

		public Kind Kind { get; }

		public bool Ref { get; }

		public SystemType SystemType { get; }

		public bool Internal { get; }

		public bool Public { get; }

		public object Create(params object?[]? parameters)
		{
			var constructor = this.Constructors.FirstValue(constructor => constructor!.Parameters.IsCallableWith(parameters));
			if (constructor.HasValue)
				return constructor.Value.Create(parameters);
			throw new ArgumentException($"{this.Name}.{nameof(Create)}(...): no constructor found that takes the {parameters?.Length ?? 0} provided {nameof(parameters)}.");
		}

		public D? GetConstructor<D>()
			where D : Delegate
			=> this.Constructors.To(constructor => constructor.Method).First<D>();

		public D? GetMethod<D>(string name, bool isStatic = false)
			where D : Delegate
			=> this.Methods.Get(name).If(method => method.Static == isStatic).To(method => method!.Method).First<D>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Implements<T>()
			where T : class
			=> ((Type)this).Implements<T>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Implements(Type type)
			=> ((Type)this).Implements(type);

		public object? Invoke(string name, params object?[]? parameters)
		{
			var method = this.Methods.Get(name).FirstValue(method => method!.Parameters.IsCallableWith(parameters));
			if (method.HasValue)
				return method.Value.Invoke(parameters);
			throw new ArgumentException($"{this.Name}.{nameof(Invoke)}(...): no method found that takes the {parameters?.Length ?? 0} provided {nameof(parameters)}.");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Is<V>()
			=> this.Handle.Is<V>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Is(Type type)
			=> this.Handle.Is(type);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Type(TypeMember member)
			=> member.Handle.ToType();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(TypeMember other)
			=> this.Handle.Equals(other.Handle);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> this.Handle.GetHashCode();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(TypeMember typeMember1, TypeMember typeMember2)
			=> typeMember1.Equals(typeMember2);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(TypeMember typeMember1, TypeMember typeMember2)
			=> !typeMember1.Equals(typeMember2);
	}
}
