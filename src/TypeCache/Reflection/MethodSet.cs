// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using System.Collections.Frozen;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;

namespace TypeCache.Reflection;

[DebuggerDisplay("{Count} methods named: {Name}")]
public sealed class MethodSet : IReadOnlyCollection<MethodEntity>
{
	private readonly IReadOnlySet<MethodEntity> _Methods;
	private readonly string _TypeName;

	public MethodSet(Type type, string name)
	{
		const BindingFlags INSTANCE_BINDING = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		this._Methods = type.GetMethods(INSTANCE_BINDING)
			.Where(methodInfo => !methodInfo.IsGenericMethodDefinition && methodInfo.Name.EqualsOrdinal(name))
			.Select(methodInfo => new MethodEntity(methodInfo))
			.ToFrozenSet();
		this._TypeName = type.Name;
		this.Name = name;
	}

	public int Count => this._Methods.Count;

	public string Name { get; }

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public MethodEntity? Find()
		=> this._Methods.FirstOrDefault(_ => _.IsCallableWithNoArguments());

	/// <param name="arguments">Method parameter arguments</param>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public MethodEntity? Find(object?[] arguments)
		=> this._Methods.FirstOrDefault(_ => _.IsCallableWith(arguments));

	/// <param name="arguments">Method parameter arguments</param>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public MethodEntity? Find(ITuple arguments)
		=> this._Methods.FirstOrDefault(_ => _.IsCallableWith(arguments));

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public IEnumerator<MethodEntity> GetEnumerator()
		=> this._Methods.GetEnumerator();

	/// <exception cref="ArgumentNullException"/>
	public object? Invoke(object instance)
	{
		var method = this.Find();
		method.ThrowIfNull(Invariant($"Invoke: Unable to find method overload for [{this._TypeName}.{this.Name}]."));

		return method.Invoke(instance);
	}

	/// <exception cref="ArgumentNullException"/>
	public object? Invoke(object instance, object?[] arguments)
	{
		if (arguments is null || arguments.Length == 0)
			return this.Invoke(instance);

		var method = this.Find(arguments);
		method.ThrowIfNull(Invariant($"Invoke: Unable to find method overload for [{this._TypeName}.{this.Name}]."));

		return method.Invoke(instance, arguments);
	}

	/// <exception cref="ArgumentNullException"/>
	public object? Invoke(object instance, ITuple arguments)
	{
		if (arguments is null || arguments.Length == 0)
			return this.Invoke(instance);

		var method = this.Find(arguments);
		method.ThrowIfNull(Invariant($"Invoke: Unable to find method overload for [{this._TypeName}.{this.Name}]."));

		return method.Invoke(instance, arguments);
	}

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	IEnumerator IEnumerable.GetEnumerator()
		=> ((IEnumerable)this._Methods).GetEnumerator();
}
