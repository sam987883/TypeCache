// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections;
using TypeCache.Extensions;

namespace TypeCache.Reflection;

[DebuggerDisplay("{Count} methods named: {Name}")]
public sealed class MethodSet<T> : ReadOnlySet<T>
	where T : Method
{
	private readonly HashSet<T> _Methods;

	private MethodSet(Type type, string name, BindingFlags binding, HashSet<T> methods) : base(methods)
	{
		this._Methods = methods;
		this.Generic = new GenericMethodSet(type, binding, name);
		this.Name = name;
	}

	public MethodSet(Type type, string name, BindingFlags binding)
		: this(type, name, binding,
			type.GetMethods(binding)
				.Where(methodInfo => (!methodInfo.IsGenericMethod || methodInfo.IsConstructedGenericMethod) && methodInfo.Name.EqualsOrdinal(name))
				.Select(methodInfo => (!methodInfo.IsStatic ? new MethodEntity(methodInfo) as T : new StaticMethodEntity(methodInfo) as T)!)
				.ToHashSet())
	{
	}

	public GenericMethodSet Generic { get; }

	public string Name { get; }

	/// <param name="arguments">Method parameter arguments</param>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public T? Find(MethodInfo methodInfo)
		=> this.Find(methodInfo.MethodHandle);

	/// <param name="arguments">Method parameter arguments</param>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public T? Find(RuntimeMethodHandle handle)
		=> this._Methods.FirstOrDefault(_ => _.Handle == handle);

	/// <param name="arguments">Method parameter arguments</param>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public T? Find(object?[] arguments)
		=> this._Methods.FirstOrDefault(_ => !_.IsGenericMethod && _.IsCallableWith(arguments));

	/// <param name="arguments">Method parameter arguments</param>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public T? Find(ITuple arguments)
		=> this._Methods.FirstOrDefault(_ => !_.IsGenericMethod && _.IsCallableWith(arguments));

	/// <param name="argumentTypes">Method parameter argument types</param>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public T? Find(Type[] argumentTypes)
		=> this._Methods.FirstOrDefault(_ => !_.IsGenericMethod && _.IsCallableWith(argumentTypes));

	/// <summary>
	/// Finds a constructed generic method by the generic type arguments.<br/>
	/// If no matching constructed generic method is found then a generic method definition is found that supports the <b><c><paramref name="genericTypeArguments"/></c></b> and <b><c><paramref name="arguments"/></c></b>.<br/>
	/// If a generic method definition is found then a constructed generic method is created from it, stored in <b><c>ConstructedGeneric</c></b> and returned.<br/>
	/// Otherwise <b><c><see langword="null"/></c></b> is returned.
	/// </summary>
	/// <param name="genericTypeArguments">Method generic type arguments</param>
	/// <param name="arguments">Method parameter arguments</param>
	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="InvalidOperationException"/>
	/// <exception cref="NotSupportedException"/>
	public T? Find(Type[] genericTypeArguments, object?[] arguments)
	{
		var method = this._Methods.FirstOrDefault(_ => _.HasGenericTypes(genericTypeArguments) && _.IsCallableWith(arguments));
		if (method is not null)
			return method;

		method = this.Generic.Find(genericTypeArguments, arguments)?.ConstructGenericMethod(genericTypeArguments) as T;
		if (method is not null)
			this._Methods.Add(method);

		return method;
	}

	/// <summary>
	/// Finds a constructed generic method by the generic type arguments.<br/>
	/// If no matching constructed generic method is found then a generic method definition is found that supports the <b><c><paramref name="genericTypeArguments"/></c></b> and <b><c><paramref name="arguments"/></c></b>.<br/>
	/// If a generic method definition is found then a constructed generic method is created from it, stored in <b><c>ConstructedGeneric</c></b> and returned.<br/>
	/// Otherwise <b><c><see langword="null"/></c></b> is returned.
	/// </summary>
	/// <param name="genericTypeArguments">Method generic type arguments</param>
	/// <param name="arguments">Method parameter arguments</param>
	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="InvalidOperationException"/>
	/// <exception cref="NotSupportedException"/>
	public T? Find(Type[] genericTypeArguments, ITuple arguments)
	{
		var method = this._Methods.FirstOrDefault(_ => _.HasGenericTypes(genericTypeArguments) && _.IsCallableWith(arguments));
		if (method is not null)
			return method;

		method = this.Generic.Find(genericTypeArguments, arguments)?.ConstructGenericMethod(genericTypeArguments) as T;
		if (method is not null)
			this._Methods.Add(method);

		return method;
	}

	/// <summary>
	/// Finds a constructed generic method by the generic type arguments.<br/>
	/// If no matching constructed generic method is found then a generic method definition is found that supports the <b><c><paramref name="genericTypeArguments"/></c></b> and <b><c><paramref name="argumentTypes"/></c></b>.<br/>
	/// If a generic method definition is found then a constructed generic method is created from it, stored in <b><c>ConstructedGeneric</c></b> and returned.<br/>
	/// Otherwise <b><c><see langword="null"/></c></b> is returned.
	/// </summary>
	/// <param name="genericTypeArguments">Method generic type arguments</param>
	/// <param name="argumentTypes">Method parameter argument types</param>
	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="InvalidOperationException"/>
	/// <exception cref="NotSupportedException"/>
	public T? Find(Type[] genericTypeArguments, Type[] argumentTypes)
	{
		var method = this._Methods.FirstOrDefault(_ => _.HasGenericTypes(genericTypeArguments) && _.IsCallableWith(argumentTypes));
		if (method is not null)
			return method;

		method = this.Generic.Find(genericTypeArguments, argumentTypes)?.ConstructGenericMethod(genericTypeArguments) as T;
		if (method is not null)
			this._Methods.Add(method);

		return method;
	}

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public T? FindWithNoArguments()
		=> this._Methods.FirstOrDefault(_ => !_.IsGenericMethod && _.IsCallableWithNoArguments());

	/// <summary>
	/// Finds a constructed generic method by the generic type arguments.<br/>
	/// If no matching constructed generic method is found then a generic method definition is found that supports the <b><c><paramref name="genericTypeArguments"/></c></b>.<br/>
	/// If a generic method definition is found then a constructed generic method is created from it, stored in <b><c>ConstructedGeneric</c></b> and returned.<br/>
	/// Otherwise <b><c><see langword="null"/></c></b> is returned.
	/// </summary>
	/// <param name="genericTypeArguments">Method generic type arguments</param>
	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="InvalidOperationException"/>
	/// <exception cref="NotSupportedException"/>
	public T? FindWithNoArguments(Type[] genericTypeArguments)
	{
		var method = this._Methods.FirstOrDefault(_ => _.HasGenericTypes(genericTypeArguments) && _.IsCallableWithNoArguments());
		if (method is not null)
			return method;

		method = this.Generic.FindWithNoArguments(genericTypeArguments)?.ConstructGenericMethod(genericTypeArguments) as T;
		if (method is not null)
			this._Methods.Add(method);

		return method;
	}
}
