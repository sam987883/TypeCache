// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using System.Collections.Frozen;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;

namespace TypeCache.Reflection;

public sealed class StaticGenericMethodSet : IReadOnlyCollection<StaticGenericMethodEntity>
{
	private readonly IReadOnlySet<StaticGenericMethodEntity> _StaticGenericMethods;
	private readonly string _TypeName;

	public StaticGenericMethodSet(Type type, string name)
	{
		const BindingFlags STATIC_BINDING = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

		this._StaticGenericMethods = type.GetMethods(STATIC_BINDING)
			.Where(methodInfo => methodInfo.IsGenericMethodDefinition && methodInfo.Name.EqualsOrdinal(name))
			.Select(methodInfo => new StaticGenericMethodEntity(methodInfo))
			.ToFrozenSet();
		this._TypeName = type.Name;
		this.Name = name;
	}

	public int Count => this._StaticGenericMethods.Count;

	public string Name { get; }

	/// <param name="genericTypeArguments">Method generic type arguments</param>
	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="InvalidOperationException"/>
	/// <exception cref="NotSupportedException"/>
	public StaticMethodEntity? Find(Type[] genericTypeArguments)
		=> this._StaticGenericMethods
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
	public StaticMethodEntity? Find(Type[] genericTypeArguments, object?[] arguments)
		=> this._StaticGenericMethods
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
	public StaticMethodEntity? Find(Type[] genericTypeArguments, ITuple arguments)
		=> this._StaticGenericMethods
			.Where(_ => _.Match(genericTypeArguments) && _.IsCallableWith(arguments))
			.Select(_ => _.GetDerivedMethod(genericTypeArguments))
			.WhereNotNull()
			.SingleOrDefault();

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public IEnumerator<StaticGenericMethodEntity> GetEnumerator()
		=> this._StaticGenericMethods.GetEnumerator();

	/// <exception cref="ArgumentNullException"/>
	public object? Invoke(Type[] types)
	{
		var method = this.Find(types);
		method.ThrowIfNull(Invariant($"Invoke: Unable to find method overload for [{this._TypeName}.{this.Name}]."));

		return method.Invoke();
	}

	/// <exception cref="ArgumentNullException"/>
	public object? Invoke(Type[] types, object?[] arguments)
	{
		if (arguments is null || arguments.Length == 0)
			return this.Invoke(types);

		var method = this.Find(types, arguments);
		method.ThrowIfNull(Invariant($"Invoke: Unable to find method overload for [{this._TypeName}.{this.Name}]."));

		return method.Invoke(arguments);
	}

	/// <exception cref="ArgumentNullException"/>
	public object? Invoke(Type[] types, ITuple arguments)
	{
		if (arguments is null || arguments.Length == 0)
			return this.Invoke(types);

		var method = this.Find(types, arguments);
		method.ThrowIfNull(Invariant($"Invoke: Unable to find method overload for [{this._TypeName}.{this.Name}]."));

		return method.Invoke(arguments);
	}

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	IEnumerator IEnumerable.GetEnumerator()
		=> ((IEnumerable)this._StaticGenericMethods).GetEnumerator();
}
