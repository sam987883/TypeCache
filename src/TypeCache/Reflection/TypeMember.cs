// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Reflection
{
	public sealed class TypeMember : Member, IEquatable<TypeMember>
	{
		private const BindingFlags BINDINGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

		static TypeMember()
		{
			Cache = new LazyDictionary<RuntimeTypeHandle, TypeMember>(typeHandle => new TypeMember(typeHandle.ToType()));
		}

		internal static IReadOnlyDictionary<RuntimeTypeHandle, TypeMember> Cache { get; }

		internal TypeMember(Type type)
			: base(type, !type.IsVisible, type.IsPublic)
		{
			this.BaseTypeHandle = type.BaseType?.TypeHandle ?? typeof(object).TypeHandle;
			this.SystemType = type.GetSystemType();
			this.EnclosedTypeHandle = this.SystemType switch
			{
				_ when type.HasElementType => type.GetElementType()?.TypeHandle,
				SystemType.Dictionary or SystemType.ImmutableDictionary or SystemType.ImmutableSortedDictionary or SystemType.SortedDictionary
					=> typeof(KeyValuePair<,>).MakeGenericType(type.GenericTypeArguments).TypeHandle,
				_ when type.GenericTypeArguments.Length == 1 => type.GenericTypeArguments[0].TypeHandle,
				_ => (RuntimeTypeHandle?)null
			};
			this.GenericTypeHandles = type.GenericTypeArguments.To(_ => _.TypeHandle).ToImmutableArray();
			this.InterfaceTypeHandles = type.GetInterfaces().To(_ => _.TypeHandle).ToImmutableArray();
			this.IsEnumerable = type.IsEnumerable();
			this.IsPointer = type.IsPointer;
			this.IsRef = type.IsByRef || type.IsByRefLike;
			this.Kind = type switch
			{
				_ when typeof(Delegate).IsAssignableFrom(type.BaseType) => Kind.Delegate,
				_ when type.IsEnum => Kind.Enum,
				_ when type.IsInterface => Kind.Interface,
				_ when type.IsValueType => Kind.Struct,
				_ => Kind.Class,
			};

			this._Constructors = new Lazy<IImmutableList<ConstructorMember>>(this.CreateConstructorMembers, false);
			this._Events = new Lazy<IImmutableDictionary<string, EventMember>>(this.CreateEventMembers, false);
			this._Fields = new Lazy<IImmutableDictionary<string, FieldMember>>(this.CreateFieldMembers, false);
			this._Methods = new Lazy<IImmutableDictionary<string, IImmutableList<MethodMember>>>(this.CreateMethodMembers, false);
			this._Properties = new Lazy<IImmutableDictionary<string, PropertyMember>>(this.CreatePropertyMembers, false);
		}

		private readonly Lazy<IImmutableList<ConstructorMember>> _Constructors;

		private readonly Lazy<IImmutableDictionary<string, EventMember>> _Events;

		private readonly Lazy<IImmutableDictionary<string, FieldMember>> _Fields;

		private readonly Lazy<IImmutableDictionary<string, IImmutableList<MethodMember>>> _Methods;

		private readonly Lazy<IImmutableDictionary<string, PropertyMember>> _Properties;

		public IImmutableList<ConstructorMember> Constructors => _Constructors.Value;

		public IImmutableDictionary<string, EventMember> Events => _Events.Value;

		public IImmutableDictionary<string, FieldMember> Fields => _Fields.Value;

		public IImmutableDictionary<string, IImmutableList<MethodMember>> Methods => _Methods.Value;

		public IImmutableDictionary<string, PropertyMember> Properties => _Properties.Value;

		public RuntimeTypeHandle BaseTypeHandle { get; }

		public RuntimeTypeHandle? EnclosedTypeHandle { get; }

		public IImmutableList<RuntimeTypeHandle> GenericTypeHandles { get; }

		public RuntimeTypeHandle Handle { get; }

		public IImmutableList<RuntimeTypeHandle> InterfaceTypeHandles { get; }

		public bool IsEnumerable { get; }

		public bool IsPointer { get; }

		public bool IsRef { get; }

		public Kind Kind { get; }

		public SystemType SystemType { get; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Type(TypeMember typeMember)
			=> typeMember.Handle.ToType();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(TypeMember? other)
			=> this.Handle == other?.Handle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> this.Handle.GetHashCode();

		private IImmutableList<ConstructorMember> CreateConstructorMembers()
			=> this.Handle.ToType().GetConstructors(BINDINGS)
				.To(constructorInfo => constructorInfo!.MethodHandle.GetConstructorMember())
				.ToImmutableArray();

		private IImmutableDictionary<string, EventMember> CreateEventMembers()
			=> this.Handle.ToType().GetEvents(BINDINGS)
				.To(eventInfo => KeyValuePair.Create(eventInfo.Name, new EventMember(eventInfo)))
				.ToImmutableDictionary(StringComparison.Ordinal);

		private IImmutableDictionary<string, FieldMember> CreateFieldMembers()
			=> this.Handle.ToType().GetFields(BINDINGS)
				.If(fieldInfo => !fieldInfo.FieldType.IsByRefLike)
				.To(fieldInfo => KeyValuePair.Create(fieldInfo!.Name, fieldInfo.FieldHandle.GetFieldMember()))
				.ToImmutableDictionary(StringComparison.Ordinal);

		private IImmutableDictionary<string, IImmutableList<MethodMember>> CreateMethodMembers()
			=> this.Handle.ToType().GetMethods(BINDINGS)
				.If(methodInfo => !methodInfo.ContainsGenericParameters && !methodInfo.IsSpecialName)
				.To(methodInfo => methodInfo!.MethodHandle.GetMethodMember())
				.Group(method => method.Name, StringComparer.Ordinal)
				.ToImmutableDictionary(_ => _.Key, _ => (IImmutableList<MethodMember>)_.Value.ToImmutableArray(), StringComparison.Ordinal);

		private IImmutableDictionary<string, PropertyMember> CreatePropertyMembers()
			=> this.Handle.ToType().GetProperties(BINDINGS)
				.To(propertyInfo => KeyValuePair.Create(propertyInfo!.Name, new PropertyMember(propertyInfo)))
				.ToImmutableDictionary(StringComparison.Ordinal);

		public object Create(params object?[]? parameters)
		{
			var constructor = this.Constructors.First(constructor => constructor!.IsCallableWith(parameters));
			if (constructor != null)
				return constructor.Create!(parameters);
			throw new ArgumentException($"{this.Name}.{nameof(Create)}(...): no constructor found that takes the {parameters?.Length ?? 0} provided {nameof(parameters)}.");
		}

		public D? GetConstructor<D>()
			where D : Delegate
			=> this.Constructors.To(constructor => constructor.Method).First<D>();

		public D? GetMethod<D>(string name, bool isStatic = false)
			where D : Delegate
			=> this.Methods.Get(name).If(method => method.Static == isStatic).To(method => method!.Method).First<D>();
	}
}
