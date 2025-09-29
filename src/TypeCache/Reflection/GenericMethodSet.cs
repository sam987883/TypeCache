// Copyright (c) 2021 Samuel Abraham

using System.Collections.Frozen;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections;
using TypeCache.Extensions;

namespace TypeCache.Reflection;

public sealed class GenericMethodSet : ReadOnlySet<Method>
{
	internal GenericMethodSet(Type type, BindingFlags binding, string name)
		: base(type.GetMethods(binding)
			.Where(methodInfo => methodInfo.IsGenericMethodDefinition && methodInfo.Name.EqualsOrdinal(name))
			.Select(methodInfo => new Method(methodInfo))
			.ToFrozenSet())
	{
		this.Name = name;
	}

	public string Name { get; }

	/// <param name="arguments">Method parameter arguments</param>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public Method? Find(MethodInfo methodInfo)
		=> this.Find(methodInfo.MethodHandle);

	/// <param name="arguments">Method parameter arguments</param>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public Method? Find(RuntimeMethodHandle handle)
		=> this.FirstOrDefault(_ => _.Handle == handle);

	/// <param name="genericTypeArguments">Method generic type arguments</param>
	public Method[] Find(Type[] genericTypeArguments)
		=> this.Where(_ => _.Supports(genericTypeArguments)).ToArray();

	/// <param name="genericTypeArguments">Method generic type arguments</param>
	/// <param name="arguments">Method parameter arguments</param>
	/// <exception cref="ArgumentException"/>
	/// <exception cref="InvalidOperationException"/>
	public Method? Find(Type[] genericTypeArguments, object?[] arguments)
		=> this.SingleOrDefault(_ => _.Supports(genericTypeArguments) && _.IsCallableWith(arguments));

	/// <param name="genericTypeArguments">Method generic type arguments</param>
	/// <param name="arguments">Method parameter arguments</param>
	/// <exception cref="ArgumentException"/>
	/// <exception cref="InvalidOperationException"/>
	public Method? Find(Type[] genericTypeArguments, ITuple arguments)
		=> this.SingleOrDefault(_ => _.Supports(genericTypeArguments) && _.IsCallableWith(arguments));

	/// <param name="genericTypeArguments">Method generic type arguments</param>
	/// <exception cref="ArgumentException"/>
	/// <exception cref="InvalidOperationException"/>
	public Method? FindWithNoArguments(Type[] genericTypeArguments)
		=> this.SingleOrDefault(_ => _.Supports(genericTypeArguments) && _.IsCallableWithNoArguments());
}
