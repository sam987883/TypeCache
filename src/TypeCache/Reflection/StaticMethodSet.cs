// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using System.Collections.Frozen;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;

namespace TypeCache.Reflection;

[DebuggerDisplay("{Count} static methods named: {Name}")]
public sealed class StaticMethodSet : IReadOnlyCollection<StaticMethodEntity>
{
	private readonly IReadOnlySet<StaticMethodEntity> _StaticMethods;
	private readonly string _TypeName;

	public StaticMethodSet(Type type, string name)
	{
		const BindingFlags STATIC_BINDING = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

		this._StaticMethods = type.GetMethods(STATIC_BINDING)
			.Where(methodInfo => !methodInfo.IsGenericMethodDefinition && methodInfo.Name.EqualsOrdinal(name))
			.Select(methodInfo => new StaticMethodEntity(methodInfo))
			.ToFrozenSet();
		this._TypeName = type.Name;
		this.Name = name;
	}

	public int Count => this._StaticMethods.Count;

	public string Name { get; }

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public StaticMethodEntity? Find()
		=> this._StaticMethods.FirstOrDefault(_ => _.IsCallableWithNoArguments());

	/// <param name="arguments">Method parameter arguments</param>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public StaticMethodEntity? Find(object?[] arguments)
		=> this._StaticMethods.FirstOrDefault(_ => _.IsCallableWith(arguments));

	/// <param name="arguments">Method parameter arguments</param>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public StaticMethodEntity? Find(ITuple arguments)
		=> this._StaticMethods.FirstOrDefault(_ => _.IsCallableWith(arguments));

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public IEnumerator<StaticMethodEntity> GetEnumerator()
		=> this._StaticMethods.GetEnumerator();

	/// <exception cref="ArgumentNullException"/>
	public object? Invoke()
	{
		var method = this.Find();
		method.ThrowIfNull(Invariant($"Invoke: Unable to find method overload for [{this._TypeName}.{this.Name}]."));

		return method.Invoke();
	}

	/// <exception cref="ArgumentNullException"/>
	public object? Invoke(object?[] arguments)
	{
		if (arguments is null || arguments.Length == 0)
			return this.Invoke();

		var method = this.Find(arguments);
		method.ThrowIfNull(Invariant($"Invoke: Unable to find method overload for [{this._TypeName}.{this.Name}]."));

		return method.Invoke(arguments);
	}

	/// <exception cref="ArgumentNullException"/>
	public object? Invoke(ITuple arguments)
	{
		if (arguments is null || arguments.Length == 0)
			return this.Invoke();

		var method = this.Find(arguments);
		method.ThrowIfNull(Invariant($"Invoke: Unable to find method overload for [{this._TypeName}.{this.Name}]."));

		return method.Invoke(arguments);
	}

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	IEnumerator IEnumerable.GetEnumerator()
		=> ((IEnumerable)this._StaticMethods).GetEnumerator();
}
