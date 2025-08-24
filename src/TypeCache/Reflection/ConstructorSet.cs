// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using System.Collections.Frozen;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;

namespace TypeCache.Reflection;

public sealed class ConstructorSet : IReadOnlyCollection<ConstructorEntity>
{
	private readonly IReadOnlySet<ConstructorEntity> _Constructors;
	private readonly string _TypeName;

	public ConstructorSet(Type type)
	{
		this._Constructors = type.GetConstructors().Select(_ => new ConstructorEntity(_)).ToFrozenSet();
		this._TypeName = type.Name;
	}

	public int Count => this._Constructors.Count;

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public ConstructorEntity? FindDefault()
		=> this._Constructors.FirstOrDefault(_ => _.IsCallableWithNoArguments());

	/// <param name="arguments">Constructor parameter arguments</param>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public ConstructorEntity? Find(object?[] arguments)
		=> this._Constructors.FirstOrDefault(_ => _.IsCallableWith(arguments));

	/// <param name="arguments">Constructor parameter arguments</param>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public ConstructorEntity? Find(ITuple arguments)
		=> this._Constructors.FirstOrDefault(_ => _.IsCallableWith(arguments));

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public IEnumerator<ConstructorEntity> GetEnumerator()
		=> this._Constructors.GetEnumerator();

	/// <exception cref="MissingMethodException"></exception>
	public object? Invoke()
		=> this.FindDefault()?.Create()
			?? throw new MissingMethodException(this._TypeName, "Default Constructor");

	/// <param name="arguments">Constructor parameter arguments</param>
	/// <exception cref="MissingMethodException"></exception>
	public object? Invoke(object?[] arguments)
		=> this.Find(arguments)?.Create(arguments)
			?? throw new MissingMethodException(this._TypeName, "Constructor");

	/// <param name="arguments">Constructor parameter arguments</param>
	/// <exception cref="MissingMethodException"></exception>
	public object? Invoke(ITuple arguments)
		=> this.Find(arguments)?.Create(arguments)
			?? throw new MissingMethodException(this._TypeName, "Constructor");

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	IEnumerator IEnumerable.GetEnumerator()
		=> ((IEnumerable)this._Constructors).GetEnumerator();
}
