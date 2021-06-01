// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Reflection.Extensions
{
	public static class MethodInfoExtensions
	{
		private static readonly IComparer<ParameterInfo> ParameterSortComparer = Comparer<ParameterInfo>.Create((x, y) => x.Position - y.Position)!;

		private static ParameterInfo[] GetParameterInfos(this MethodInfo @this)
			=> @this.GetParameters()
				.If(parameterInfo => parameterInfo != @this.ReturnParameter)
				.Sort(ParameterSortComparer!)!;

		public static bool IsInvokable(this MethodBase @this)
			=> @this.GetParameters().All(_ => !_!.IsOut && _.ParameterType.IsInvokable());

		public static Delegate ToDelegate(this MethodInfo @this)
		{
			var parameters = @this.GetParameterInfos().ToArray(parameterInfo => parameterInfo!.Parameter());
			ParameterExpression instance = nameof(instance).Parameter(@this.DeclaringType!);
			return parameters switch
			{
				{ Length: 0 } when @this.IsStatic => @this.CallStatic().Lambda().Compile(),
				{ Length: 0 } => instance.Call(@this).Lambda(instance).Compile(),
				_ when @this.IsStatic => @this.CallStatic(parameters).Lambda(parameters).Compile(),
				_ => instance.Call(@this, parameters).Lambda(getLambdaParameters(@this, instance, parameters)).Compile()
			};

			static IEnumerable<ParameterExpression> getLambdaParameters(MethodInfo methodInfo, ParameterExpression instance, ParameterExpression[] parameters)
			{
				yield return instance;
				var count = parameters.Length;
				for (var i = 0; i < count; ++i)
					yield return parameters[i];
			}
		}

		public static InvokeType ToInvokeType(this MethodInfo @this)
		{
			ParameterExpression instance = nameof(instance).Parameter(typeof(object));
			ParameterExpression arguments = nameof(arguments).Parameter<object?[]?>();

			var callParameters = @this.GetParameterInfos()
				.To(parameterInfo => arguments.Array()[parameterInfo!.Position].SystemConvert(parameterInfo.ParameterType));
			var cast = instance.Cast(@this.DeclaringType!);
			var call = callParameters.Any() ? cast.Call(@this, callParameters) : cast.Call(@this);

			return @this.ReturnType == typeof(void)
				? call.Block(Expression.Constant(null)).Lambda<InvokeType>(instance, arguments).Compile()
				: call.As<object>().Lambda<InvokeType>(instance, arguments).Compile();
		}

		public static ConstructorMember ToMember(this ConstructorInfo @this)
		{
			var parameterInfos = @this.GetParameters();
			parameterInfos.Sort(ParameterSortComparer);

			CreateType? create = null;
			Delegate? method = null;
			if (@this.IsInvokable())
			{
				var methodParameters = parameterInfos.ToArray(parameterInfo => parameterInfo.Parameter());
				method = parameterInfos.Any()
					? @this.New(methodParameters).Lambda(methodParameters).Compile()
					: @this.New().Lambda().Compile();

				ParameterExpression arguments = nameof(arguments).Parameter<object?[]?>();
				var callParameters = parameterInfos
					.To(parameterInfo => arguments.Array()[parameterInfo!.Position].SystemConvert(parameterInfo.ParameterType));
				create = parameterInfos.Any()
					? @this.New(callParameters).Cast<object>().Lambda<CreateType>(arguments).Compile()
					: @this.New().Cast<object>().Lambda<CreateType>(arguments).Compile();
			}

			var attributes = @this.GetCustomAttributes<Attribute>(true).ToImmutableArray();
			var parameters = parameterInfos.To(ToParameter!).ToImmutableArray();
			var type = @this.DeclaringType!.GetTypeMember();
			var returnParameter = new ReturnParameter(ImmutableArray<Attribute>.Empty, type);

			return new ConstructorMember(@this.Name, type, attributes, @this.IsAssembly, @this.IsPublic, @this.MethodHandle, create, method, parameters, returnParameter);
		}

		public static InstanceMethodMember ToMember(this MethodInfo @this)
		{
			@this.IsStatic.Assert($"{nameof(@this)}.{nameof(@this.IsStatic)}", false);

			var attributes = @this.GetCustomAttributes<Attribute>(true).ToImmutableArray();
			var invoke = @this.ToInvokeType();
			var method = @this.ToDelegate();
			var parameters = @this.GetParameterInfos().To(ToParameter!).ToImmutableArray();
			var returnAttributes = @this.ReturnParameter.GetCustomAttributes<Attribute>(true).ToImmutableArray();
			var returnType = @this.ReturnType.GetTypeMember();
			var returnParameter = new ReturnParameter(returnAttributes, returnType);
			var type = @this.DeclaringType!.GetTypeMember();

			return new InstanceMethodMember(@this.GetName(), type, attributes, @this.IsAssembly, @this.IsPublic, @this.MethodHandle, method, invoke, parameters, returnParameter);
		}

		public static MethodParameter ToParameter(ParameterInfo parameterInfo)
		{
			var attributes = parameterInfo.GetCustomAttributes<Attribute>(true).ToImmutableArray();
			var type = parameterInfo.ParameterType.GetTypeMember();

			return new MethodParameter(parameterInfo.GetName(), attributes, parameterInfo.IsOptional, parameterInfo.IsOut, parameterInfo.DefaultValue, parameterInfo.HasDefaultValue, type);
		}

		public static StaticInvokeType ToStaticInvokeType(this MethodInfo @this)
		{
			ParameterExpression arguments = nameof(arguments).Parameter<object?[]?>();

			var callParameters = @this.GetParameterInfos()
				.To(parameterInfo => arguments.Array()[parameterInfo!.Position].SystemConvert(parameterInfo.ParameterType));
			var call = callParameters.Any() ? @this.CallStatic(callParameters) : @this.CallStatic();

			return @this.ReturnType == typeof(void)
				? call.Block(Expression.Constant(null)).Lambda<StaticInvokeType>(arguments).Compile()
				: call.As<object>().Lambda<StaticInvokeType>(arguments).Compile();
		}

		public static StaticMethodMember ToStaticMember(this MethodInfo @this)
		{
			@this.IsStatic.Assert($"{nameof(@this)}.{nameof(@this.IsStatic)}", true);

			var attributes = @this.GetCustomAttributes<Attribute>(true).ToImmutableArray();
			var invoke = @this.ToStaticInvokeType();
			var method = @this.ToDelegate();
			var parameters = @this.GetParameterInfos().To(ToParameter!).ToImmutableArray();
			var returnAttributes = @this.ReturnParameter.GetCustomAttributes<Attribute>(true).ToImmutableArray();
			var returnType = @this.ReturnType.GetTypeMember();
			var returnParameter = new ReturnParameter(returnAttributes, returnType);
			var type = @this.DeclaringType!.GetTypeMember();

			return new StaticMethodMember(@this.GetName(), type, attributes, @this.IsAssembly, @this.IsPublic, @this.MethodHandle, method, invoke, parameters, returnParameter);
		}
	}
}
