// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using Microsoft.Extensions.Logging;
using TypeCache.Extensions;
using TypeCache.Reflection;

namespace TypeCache.Testing;

/// <summary>
/// Used for mocking interfaces in unit tests.
/// </summary>
/// <typeparam name="T">Must be an <c><see langword="interface"/></c> type.</typeparam>
public class Shadow<T> : DispatchProxy
	where T : class
{
	private readonly ILogger? _Logger = null;
	private readonly Dictionary<MethodEntity, Delegate> _Methods = new();

	public Shadow() : base()
	{
		Type<T>.ClrType.ThrowIfNotEqual(ClrType.Interface);
	}

	public Shadow(ILogger logger) : base()
	{
		logger.ThrowIfNull();
		Type<T>.ClrType.ThrowIfNotEqual(ClrType.Interface, logger: logger);

		this._Logger = logger;
	}

	public T Instance => DispatchProxy.Create<T, Shadow<T>>();

	public void Override(string method, Delegate call)
	{
		Type<T>.Methods.TryGetValue(method, out var methods).ThrowIfFalse(logger: this._Logger);

		var parameterTypes = call.Method.GetParameters().Where(_ => !_.IsRetval).OrderBy(_ => _.Position).Select(_ => _.ParameterType).ToArray();
		var methodEntity = methods?.Find(parameterTypes);
		methodEntity.ThrowIfNull(
			() => Invariant($"Method overload was not found: {Type<T>.CodeName}.{method}({parameterTypes.Select(_ => _.CodeName()).ToCSV()})"), this._Logger);

		this._Methods[methodEntity] = call;
	}

	protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
	{
		targetMethod.ThrowIfNull(() => "Call was made to non-existent method.", this._Logger);

		args ??= [];
		MethodEntity? methodEntity = null;
		if (Type<T>.Methods.TryGetValue(targetMethod.Name, out var methods))
		{
			if (targetMethod.IsGenericMethod)
				methodEntity = methods.Find(args!);
		}

		// If method is found, run override if one exists
		if (methodEntity is not null)
			return this._Methods.TryGetValue(methodEntity, out var call) ? call.DynamicInvoke(args) : null;

		// If method is not found, could be an extension method
		if (targetMethod.GetParameters().FirstOrDefault(_ => _.Position is 0)?.ParameterType == typeof(T))
		{
			var targetMethodEntity = targetMethod.DeclaringType!.Methods()[targetMethod.Name].Find(args)!;
			// Invoke override if found
			if (this._Methods.TryGetValue(targetMethodEntity, out var extensionOverride))
				return extensionOverride.DynamicInvoke([this.Instance, .. args]);
			else // Invoke potential extension method
				return targetMethodEntity.Invoke(this.Instance, args);
		}

		return null;
	}
}
