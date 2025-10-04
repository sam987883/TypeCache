// Copyright (c) 2021 Samuel Abraham

using System.Collections.Frozen;
using System.Runtime.CompilerServices;
using TypeCache.Collections;
using TypeCache.Extensions;

namespace TypeCache.Reflection;

public sealed class ConstructorSet : ReadOnlySet<ConstructorEntity>
{
	public ConstructorSet(Type type)
		: base(type.GetConstructors().Select(_ => new ConstructorEntity(_)).ToFrozenSet())
	{
	}

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public ConstructorEntity? FindDefault()
		=> this.FirstOrDefault(_ => _.IsCallableWithNoArguments());

	/// <param name="arguments">Constructor parameter arguments</param>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public ConstructorEntity? Find(object?[] arguments)
		=> this.FirstOrDefault(_ => _.IsCallableWith(arguments));

	/// <param name="arguments">Constructor parameter arguments</param>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public ConstructorEntity? Find(ITuple arguments)
		=> this.FirstOrDefault(_ => _.IsCallableWith(arguments));
}
