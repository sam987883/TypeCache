// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using GraphQL.Types;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using static System.Globalization.CultureInfo;

namespace TypeCache.GraphQL.Extensions;

public static class GraphQLAttributeExtensions
{
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string? GraphQLDeprecationReason(this MemberInfo @this)
		=> @this.GetCustomAttribute<GraphQLDeprecationReasonAttribute>()?.DeprecationReason;

	public static string? GraphQLDescription(this MemberInfo @this)
		=> @this.GetCustomAttribute<GraphQLDescriptionAttribute>()?.Description switch
		{
			null => null,
			var description when @this is Type type && type.IsGenericType =>
				string.Format(InvariantCulture, description, type.GenericTypeArguments.Select(_ => _.GraphQLName()).ToArray()),
			var description => description
		};

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string? GraphQLDescription(this ParameterInfo @this)
		=> @this.GetCustomAttribute<GraphQLDescriptionAttribute>()?.Description;

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool GraphQLIgnore(this MemberInfo @this)
		=> @this.HasCustomAttribute<GraphQLIgnoreAttribute>();

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static bool GraphQLIgnore(this ParameterInfo @this)
		=> @this.HasCustomAttribute<GraphQLIgnoreAttribute>();

	public static string GraphQLInputName(this Type @this)
		=> @this.GetCustomAttribute<GraphQLInputNameAttribute>()?.Name ?? Invariant($"{@this.GraphQLName()}Input");

	public static string GraphQLName(this MemberInfo @this)
		=> @this.GetCustomAttribute<GraphQLNameAttribute>()?.Name ?? @this switch
		{
			MethodInfo methodInfo => methodInfo.Name.TrimEnd("Async"),
			Type type when type.IsGenericType => Invariant($"{type.Name()}{type.GenericTypeArguments.Select(_ => _.GraphQLName()).Concat()}"),
			Type type => type.Name(),
			_ => @this.Name
		};

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static string GraphQLName(this ParameterInfo @this)
		=> @this.GetCustomAttribute<GraphQLNameAttribute>()?.Name ?? @this.Name();

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Type? GraphQLType(this MemberInfo @this)
		=> @this.GetCustomAttribute<GraphQLTypeAttribute>()?.GetType().GenericTypeArguments[0];

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Type? GraphQLType(this ParameterInfo @this)
		=> @this.GetCustomAttribute<GraphQLTypeAttribute>()?.GetType().GenericTypeArguments[0];
}
