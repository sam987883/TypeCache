// Copyright (c) 2021 Samuel Abraham

using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;

namespace TypeCache.Reflection;

[DebuggerDisplay("Generic method: {Name}")]
public sealed class GenericMethodEntity : Method
{
	private readonly ConcurrentBag<MethodEntity> _DervivedMethods = new();

	public GenericMethodEntity(MethodInfo methodInfo) : base(methodInfo)
	{
		methodInfo.IsStatic.ThrowIfTrue();
	}

	public MethodEntity? GetDerivedMethod(Type[] genericTypeParameters)
	{
		var method = this._DervivedMethods.SingleOrDefault(_ => _.Match(genericTypeParameters)
			&& _.IsCallableWith([]));
		if (method is not null)
			return method;

		method = new(this.ToMethodInfo().MakeGenericMethod(genericTypeParameters));
		this._DervivedMethods.Add(method);
		return method;
	}

	public MethodEntity? GetDerivedMethod(Type[] genericTypeParameters, object?[] arguments)
	{
		var method = this._DervivedMethods.SingleOrDefault(_ => _.Match(genericTypeParameters)
			&& _.IsCallableWith(arguments));
		if (method is not null)
			return method;

		method = new(this.ToMethodInfo().MakeGenericMethod(genericTypeParameters));
		this._DervivedMethods.Add(method);
		return method;
	}

	public MethodEntity? GetDerivedMethod(Type[] genericTypeParameters, ITuple arguments)
	{
		var method = this._DervivedMethods.SingleOrDefault(_ => _.Match(genericTypeParameters)
			&& _.IsCallableWith(arguments));
		if (method is not null)
			return method;

		method = new(this.ToMethodInfo().MakeGenericMethod(genericTypeParameters));
		this._DervivedMethods.Add(method);
		return method;
	}
}
