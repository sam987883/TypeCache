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
	public sealed class TypeMember : Member, IEquatable<TypeMember>
	{
		private const BindingFlags BINDINGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

		static TypeMember()
		{
			Cache = new LazyDictionary<RuntimeTypeHandle, TypeMember>(typeHandle => new TypeMember(typeHandle.ToType()));
		}

		internal static IReadOnlyDictionary<RuntimeTypeHandle, TypeMember> Cache { get; }

		internal TypeMember(Type type)
			: base(type)
		{
			this.Handle = type.TypeHandle;
			this.Kind = type.GetKind();
			this.SystemType = type.GetSystemType();
			this.Ref = type.IsByRef || type.IsByRefLike;

			this._BaseType = new Lazy<TypeMember>(() => this.Kind switch
			{
				Kind.Enum => typeof(Enum).GetTypeMember(),
				Kind.Struct => typeof(ValueType).GetTypeMember(),
				_ when type.BaseType is not null => type.BaseType.GetTypeMember(),
				_ when type == typeof(object) => this,
				_ => typeof(object).GetTypeMember()
			}, false);
			this._EnclosedType = new Lazy<TypeMember?>(() => this.SystemType switch
			{
				_ when type.HasElementType => type.GetElementType()?.GetTypeMember(),
				SystemType.Dictionary or SystemType.ImmutableDictionary or SystemType.ImmutableSortedDictionary or SystemType.SortedDictionary
					=> typeof(KeyValuePair<,>).MakeGenericType(type.GenericTypeArguments).GetTypeMember(),
				_ when type.GenericTypeArguments.Length == 1 => type.GenericTypeArguments[0].GetTypeMember(),
				_ => null
			}, false);
			this._GenericTypes = new Lazy<IImmutableList<TypeMember>>(() => type.GenericTypeArguments.To(_ => _.GetTypeMember()).ToImmutableArray(), false);
			this._InterfaceTypes = new Lazy<IImmutableList<TypeMember>>(() => type.GetInterfaces().To(_ => _.GetTypeMember()).ToImmutableArray(), false);

			this._Constructors = new Lazy<IImmutableList<ConstructorMember>>(this.CreateConstructorMembers, false);
			this._Events = new Lazy<IImmutableDictionary<string, EventMember>>(this.CreateEventMembers, false);
			this._Fields = new Lazy<IImmutableDictionary<string, FieldMember>>(this.CreateFieldMembers, false);
			this._Methods = new Lazy<IImmutableDictionary<string, IImmutableList<MethodMember>>>(this.CreateMethodMembers, false);
			this._Properties = new Lazy<IImmutableDictionary<string, PropertyMember>>(this.CreatePropertyMembers, false);
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

		private IImmutableList<ConstructorMember> CreateConstructorMembers()
			=> this.Handle.ToType().GetConstructors(BINDINGS)
				.If(constructorInfo => !constructorInfo.IsStatic && constructorInfo.IsInvokable())
				.To(constructorInfo => constructorInfo.MethodHandle.GetConstructorMember(this.Handle))
				.ToImmutableArray();

		private IImmutableDictionary<string, EventMember> CreateEventMembers()
			=> this.Handle.ToType().GetEvents(BINDINGS)
				.To(eventInfo => KeyValuePair.Create(eventInfo.Name, new EventMember(eventInfo)))
				.ToImmutableDictionary(StringComparison.Ordinal);

		private IImmutableDictionary<string, FieldMember> CreateFieldMembers()
			=> this.Handle.ToType().GetFields(BINDINGS)
				.If(fieldInfo => !fieldInfo.IsLiteral && !fieldInfo.FieldType.IsByRefLike)
				.To(fieldInfo => KeyValuePair.Create(fieldInfo.Name, fieldInfo.FieldHandle.GetFieldMember(this.Handle)))
				.ToImmutableDictionary(StringComparison.Ordinal);

		private IImmutableDictionary<string, IImmutableList<MethodMember>> CreateMethodMembers()
			=> this.Handle.ToType().GetMethods(BINDINGS)
				.If(methodInfo => !methodInfo.ContainsGenericParameters && !methodInfo.IsSpecialName && methodInfo.IsInvokable())
				.To(methodInfo => methodInfo.MethodHandle.GetMethodMember(this.Handle))
				.Group(method => method.Name, StringComparer.Ordinal)
				.ToImmutableDictionary(_ => _.Key, _ => (IImmutableList<MethodMember>)_.Value.ToImmutableArray(), StringComparison.Ordinal);

		private IImmutableDictionary<string, PropertyMember> CreatePropertyMembers()
			=> this.Handle.ToType().GetProperties(BINDINGS)
				.If(propertyInfo => propertyInfo.PropertyType.IsInvokable())
				.To(propertyInfo => KeyValuePair.Create(propertyInfo.Name, new PropertyMember(propertyInfo)))
				.ToImmutableDictionary(StringComparison.Ordinal);

		public object Create(params object?[]? parameters)
		{
			var constructor = this.Constructors.First(constructor => constructor!.Parameters.IsCallableWith(parameters));
			if (constructor != null)
				return constructor.Create(parameters);
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
			var method = this.Methods.Get(name).First(method => method!.Parameters.IsCallableWith(parameters));
			if (method != null)
				return method.Invoke(parameters);
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
		public bool Equals(TypeMember? other)
			=> this.Handle == other?.Handle;
	}
}
