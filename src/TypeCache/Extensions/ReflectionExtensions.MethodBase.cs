// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using System.Runtime.CompilerServices;

namespace TypeCache.Extensions;

public partial class ReflectionExtensions
{
	public static bool IsCallableWith(this MethodBase @this, object?[]? arguments)
	{
		var parameterInfos = @this.GetParameters();
		if (parameterInfos.Any(_ => _.IsOut))
			return false;

		if (arguments is null || arguments.Length == 0)
			return !parameterInfos.Any() || parameterInfos.All(parameterInfo => parameterInfo!.HasDefaultValue || parameterInfo.IsOptional);

		if (arguments.Length > parameterInfos.Length)
			return false;

		return parameterInfos
			.All(parameterInfo => parameterInfo switch
			{
				_ when parameterInfo.Position >= arguments.Length => parameterInfo.HasDefaultValue || parameterInfo.IsOptional,
				_ when arguments[parameterInfo.Position] is not null => arguments[parameterInfo.Position]!.GetType().IsAssignableTo(parameterInfo.ParameterType),
				_ => parameterInfo.ParameterType.IsNullable()
			});
	}

	public static bool IsCallableWith(this MethodBase @this, ITuple? arguments)
	{
		var parameterInfos = @this.GetParameters();
		if (parameterInfos.Any(_ => _.IsOut))
			return false;

		if (arguments is null)
			return !parameterInfos.Any() || parameterInfos.All(parameterInfo => parameterInfo!.HasDefaultValue || parameterInfo.IsOptional);

		if (arguments.Length > parameterInfos.Length)
			return false;

		return parameterInfos
			.All(parameterInfo => parameterInfo switch
			{
				_ when parameterInfo.Position >= arguments.Length => parameterInfo.HasDefaultValue || parameterInfo.IsOptional,
				_ when arguments[parameterInfo.Position] is not null => arguments[parameterInfo.Position]!.GetType().IsAssignableTo(parameterInfo.ParameterType),
				_ => parameterInfo.ParameterType.IsNullable()
			});
	}

	[DebuggerHidden]
	internal static bool IsInvokable(this MethodBase @this)
		=> @this.GetParameters().All(parameterInfo => !parameterInfo.IsOut && parameterInfo.ParameterType.IsInvokable());
}
