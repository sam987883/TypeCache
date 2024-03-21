// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using TypeCache.Utilities;

namespace TypeCache.Extensions;

public partial class ReflectionExtensions
{
	public static bool IsCallableWith(this MethodBase @this, object?[]? arguments)
	{
		var parameterInfos = @this.GetParameters();
		if (parameterInfos.Any(_ => _.IsOut))
			return false;

		if (arguments?.Any() is not true)
			return !parameterInfos.Any() || parameterInfos.All(parameterInfo => parameterInfo!.HasDefaultValue || parameterInfo.IsOptional);

		var argumentEnumerator = arguments.GetEnumerator();
		return parameterInfos
			.All(parameterInfo => argumentEnumerator.IfNext(out var argument) switch
			{
				false => parameterInfo.HasDefaultValue || parameterInfo.IsOptional,
				true when argument is not null => argument.GetType().IsAssignableTo(parameterInfo.ParameterType),
				_ => parameterInfo.ParameterType.IsNullable()
			}) && !argumentEnumerator.MoveNext();
	}

	/// <param name="arguments">The method arguments.</param>
	/// <exception cref="UnreachableException"></exception>
	public static object? InvokeMethod(this MethodBase @this, object?[]? arguments)
	{
		@this.DeclaringType.AssertNotNull();
		var method = TypeStore.MethodFuncs[(@this.DeclaringType.TypeHandle, @this.MethodHandle)];
		return method.Invoke(arguments);
	}

	[DebuggerHidden]
	internal static bool IsInvokable(this MethodBase @this)
		=> @this.GetParameters().All(parameterInfo => !parameterInfo.IsOut && parameterInfo.ParameterType.IsInvokable());
}
