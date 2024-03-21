// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using System.Reflection;
using TypeCache.Collections;

namespace TypeCache.Extensions;

public partial class ReflectionExtensions
{
	/// <param name="instance">Pass <c><see langword="null"/></c> if this is a static property getter.</param>
	/// <param name="index">Property <b>indexer</b> arguments; otherwise <c><see langword="null"/></c> if not an indexer.</param>
	public static object? GetPropertyValue(this PropertyInfo @this, object? instance, object?[]? index = null)
	{
		@this.CanRead.AssertTrue();

		var arguments = instance switch
		{
			null when index?.Any() is true => index,
			null => Array<object>.Empty,
			_ when index?.Any() is true => index.Prepend(instance).ToArray(),
			_ => new[] { instance }
		};
		return @this.GetMethod!.InvokeMethod(arguments);
	}

	/// <param name="instance">Pass <c><see langword="null"/></c> if this is a static property setter.</param>
	/// <param name="index">Property <b>indexer</b> arguments; otherwise <c><see langword="null"/></c> if not an indexer.</param>
	public static void SetPropertyValue(this PropertyInfo @this, object? instance, object? value, object?[]? index = null)
	{
		@this.CanWrite.AssertTrue();

		object?[] arguments = instance switch
		{
			null when index?.Any() is true => index.Append(value).ToArray(),
			null => new[] { value },
			_ when index?.Any() is true => index.Prepend(instance).Append(value).ToArray(),
			_ => new[] { instance, value }
		};
		@this.SetMethod!.InvokeMethod(arguments);
	}

	/// <inheritdoc cref="Expression.Property(Expression, PropertyInfo)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Property(<paramref name="instance"/>, @<paramref name="this"/>);</c>
	/// </remarks>
	/// <param name="instance">Pass <c><see langword="null"/></c> for static property access.</param>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MemberExpression ToExpression(this PropertyInfo @this, Expression? instance)
		=> Expression.Property(instance, @this);

	/// <inheritdoc cref="Expression.Property(Expression, PropertyInfo)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Property(<paramref name="instance"/>, @<paramref name="this"/>, <paramref name="indexes"/>);</c>
	/// </remarks>
	/// <param name="instance">Pass <c><see langword="null"/></c> for static property access.</param>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static IndexExpression ToExpression(this PropertyInfo @this, Expression? instance, Expression[] indexes)
		=> Expression.Property(instance, @this, indexes);
}
