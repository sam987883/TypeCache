// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using TypeCache.Collections;

namespace TypeCache.Extensions;

partial class ReflectionExtensions
{
	/// <param name="instance">Pass <c><see langword="null"/></c> if this is a static property getter.</param>
	/// <param name="index">Property <b>indexer</b> arguments; otherwise <c><see langword="null"/></c> if not an indexer.</param>
	public static object? GetPropertyValue(this PropertyInfo @this, object? instance, params object?[]? index)
		=> @this.CanRead ? @this.GetMethod?.InvokeMethod(instance switch
		{
			null when index?.Any() is true => index,
			null => Array<object>.Empty,
			_ when index?.Any() is true => new[] { instance }.Concat(index).ToArray(),
			_ => new[] { instance }
		}) : null;

	/// <param name="instance">Pass <c><see langword="null"/></c> if this is a static property setter.</param>
	/// <param name="index">Property <b>indexer</b> arguments; otherwise <c><see langword="null"/></c> if not an indexer.</param>
	public static void SetPropertyValue(this PropertyInfo @this, object? instance, object? value, params object?[]? index)
	{
		if (@this.CanWrite)
			@this.SetMethod?.InvokeMethod(instance switch
			{
				null when index?.Any() is true => index.Append(value).ToArray(),
				null => new[] { value },
				_ when index?.Any() is true => new[] { instance }.Concat(index).Append(value).ToArray(),
				_ => new[] { instance }.Append(value).ToArray()
			});
	}
}
