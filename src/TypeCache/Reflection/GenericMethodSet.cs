// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using System.Collections.Frozen;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;

namespace TypeCache.Reflection;

public sealed class GenericMethodSet : IReadOnlyCollection<GenericMethodEntity>
{
	private readonly IReadOnlySet<GenericMethodEntity> _GenericMethods;
	private readonly string _TypeName;

	public GenericMethodSet(Type type, string name)
	{
		const BindingFlags INSTANCE_BINDING = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		this._GenericMethods = type.GetMethods(INSTANCE_BINDING)
			.Where(methodInfo => methodInfo.IsGenericMethodDefinition && methodInfo.Name.EqualsOrdinal(name))
			.Select(methodInfo => new GenericMethodEntity(methodInfo))
			.ToFrozenSet();
		this._TypeName = type.Name;
		this.Name = name;
	}

	public int Count => this._GenericMethods.Count;

	public string Name { get; }

	/// <param name="genericTypeArguments">Method generic type arguments</param>
	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="InvalidOperationException"/>
	/// <exception cref="NotSupportedException"/>
	public MethodEntity? Find(Type[] genericTypeArguments)
		=> this._GenericMethods
			.Where(_ => _.Match(genericTypeArguments) && _.IsCallableWithNoArguments())
			.Select(_ => _.GetDerivedMethod(genericTypeArguments))
			.WhereNotNull()
			.SingleOrDefault();

	/// <param name="genericTypeArguments">Method generic type arguments</param>
	/// <param name="arguments">Method parameter arguments</param>
	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="InvalidOperationException"/>
	/// <exception cref="NotSupportedException"/>
	public MethodEntity? Find(Type[] genericTypeArguments, object?[] arguments)
		=> this._GenericMethods
			.Where(_ => _.Match(genericTypeArguments) && _.IsCallableWith(arguments))
			.Select(_ => _.GetDerivedMethod(genericTypeArguments))
			.WhereNotNull()
			.SingleOrDefault();

	/// <param name="genericTypeArguments">Method generic type arguments</param>
	/// <param name="arguments">Method parameter arguments</param>
	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="InvalidOperationException"/>
	/// <exception cref="NotSupportedException"/>
	public MethodEntity? Find(Type[] genericTypeArguments, ITuple arguments)
		=> this._GenericMethods
			.Where(_ => _.Match(genericTypeArguments) && _.IsCallableWith(arguments))
			.Select(_ => _.GetDerivedMethod(genericTypeArguments))
			.WhereNotNull()
			.SingleOrDefault();

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public IEnumerator<GenericMethodEntity> GetEnumerator()
		=> this._GenericMethods.GetEnumerator();

	/// <exception cref="ArgumentNullException"/>
	public object? Invoke(Type[] types, object instance)
	{
		var method = this.Find(types);
		method.ThrowIfNull(Invariant($"Invoke: Unable to find method overload for [{this._TypeName}.{this.Name}]."));

		return method.Invoke(instance);
	}

	/// <exception cref="ArgumentNullException"/>
	public object? Invoke(Type[] types, object instance, object?[] arguments)
	{
		if (arguments is null || arguments.Length == 0)
			return this.Invoke(types, instance);

		var method = this.Find(types, arguments);
		method.ThrowIfNull(Invariant($"Invoke: Unable to find method overload for [{this._TypeName}.{this.Name}]."));

		return method.Invoke(instance, arguments);
	}

	/// <exception cref="ArgumentNullException"/>
	public object? Invoke(Type[] types, object instance, ITuple arguments)
	{
		if (arguments is null || arguments.Length == 0)
			return this.Invoke(types, instance);

		var method = this.Find(types, arguments);
		method.ThrowIfNull(Invariant($"Invoke: Unable to find method overload for [{this._TypeName}.{this.Name}]."));

		return method.Invoke(instance, arguments);
	}

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	IEnumerator IEnumerable.GetEnumerator()
		=> ((IEnumerable)this._GenericMethods).GetEnumerator();
}
