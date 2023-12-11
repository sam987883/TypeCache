// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using System.Reflection;
using TypeCache.Collections;

namespace TypeCache.Extensions;

partial class ReflectionExtensions
{
	/// <param name="instance">Pass <c><see langword="null"/></c> if this is a static property getter.</param>
	/// <param name="index">Property <b>indexer</b> arguments; otherwise <c><see langword="null"/></c> if not an indexer.</param>
	public static object? GetPropertyValue(this PropertyInfo @this, object? instance, params object?[]? index)
	{
		@this.CanRead.AssertTrue();

		var arguments = instance switch
		{
			null when index?.Any() is true => index,
			null => Array<object>.Empty,
			_ when index?.Any() is true => index.Prepend(instance).ToArray(),
			_ => [instance]
		};
		return @this.GetMethod!.InvokeMethod(arguments);
	}

	/// <param name="instance">Pass <c><see langword="null"/></c> if this is a static property setter.</param>
	/// <param name="index">Property <b>indexer</b> arguments; otherwise <c><see langword="null"/></c> if not an indexer.</param>
	public static void SetPropertyValue(this PropertyInfo @this, object? instance, object? value, params object?[]? index)
	{
		@this.CanWrite.AssertTrue();

		var arguments = instance switch
		{
			null when index?.Any() is true => index.Append(value).ToArray(),
			null => [value],
			_ when index?.Any() is true => index.Prepend(instance).Append(value).ToArray(),
			_ => [instance, value]
		};
		@this.SetMethod!.InvokeMethod(arguments);
	}

	/// <inheritdoc cref="Expression.Property(Expression, PropertyInfo)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Property(<see langword="null"/>, @<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MemberExpression ToStaticPropertyExpression(this PropertyInfo @this)
		=> Expression.Property(null, @this);

	/// <inheritdoc cref="Expression.Property(Expression, PropertyInfo, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Property(<see langword="null"/>, @<paramref name="this"/>, <paramref name="index"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static IndexExpression ToStaticPropertyExpression(this PropertyInfo @this, ParameterExpression index)
		=> Expression.Property(null, @this, index);
}
