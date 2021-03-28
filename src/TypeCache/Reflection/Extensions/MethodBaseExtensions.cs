// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Reflection.Extensions
{
	public static class MethodBaseExtensions
	{
		private static readonly IComparer<ParameterInfo> ParameterSortComparer = Comparer<ParameterInfo>.Create((x, y) => x.Position - y.Position)!;

		private static ParameterInfo[] GetParameterInfos(this MethodInfo @this)
			=> @this.GetParameters()
				.If(parameterInfo => parameterInfo != @this.ReturnParameter)
				.Sort(ParameterSortComparer!)!;

		public static ConstructorMember CreateMember(this ConstructorInfo @this)
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

				ParameterExpression callParameters = nameof(callParameters).Parameter<object?[]?>();
				create = parameterInfos.Any()
					? @this.New(callParameters.ToParameterArray(parameterInfos)).Cast<object>().Lambda<CreateType>(callParameters).Compile()
					: @this.New().Cast<object>().Lambda<CreateType>(callParameters).Compile();
			}

			var attributes = @this.GetCustomAttributes<Attribute>(true).ToImmutableArray();
			var parameters = parameterInfos.To(CreateParameter!).ToImmutableArray();
			var type = MemberCache.Types[@this.DeclaringType!.TypeHandle];

			return new ConstructorMember(@this.Name, attributes, @this.IsAssembly, @this.IsPublic, @this.MethodHandle, create, method, parameters, type);
		}

		public static Delegate CreateDelegate(this MethodInfo @this)
		{
			var callParameters = @this.GetParameterInfos().ToArray(parameterInfo => parameterInfo!.Parameter());
			var lambdaParameters = new ParameterExpression[callParameters.Length + 1];

			ParameterExpression instance = nameof(instance).Parameter(@this.DeclaringType!);
			lambdaParameters[0] = instance;
			callParameters.CopyTo(lambdaParameters, 1);

			return callParameters.Any()
				? instance.Call(@this, callParameters).Lambda(lambdaParameters).Compile()
				: instance.Call(@this).Lambda(instance).Compile();
		}

		public static InvokeType CreateInvokeType(this MethodInfo @this)
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

		public static MethodMember CreateMember(this MethodInfo @this)
		{
			@this.IsStatic.Assert($"{nameof(@this)}.{nameof(@this.IsStatic)}", false);

			var attributes = @this.GetCustomAttributes<Attribute>(true).ToImmutableArray();
			var invoke = @this.CreateInvokeType();
			var method = @this.CreateDelegate();
			var parameters = @this.GetParameterInfos().To(CreateParameter!).ToImmutableArray();
			var returnAttributes = @this.ReturnParameter.GetCustomAttributes<Attribute>(true).ToImmutableArray();
			var returnType = MemberCache.Types[@this.ReturnType.TypeHandle];
			var returnParameter = new ReturnParameter(returnAttributes, returnType);

			return new MethodMember(@this.GetName(), attributes, @this.IsAssembly, @this.IsPublic, @this.MethodHandle, invoke, method, parameters, returnParameter);
		}

		public static Parameter CreateParameter(ParameterInfo parameterInfo)
		{
			var attributes = parameterInfo.GetCustomAttributes<Attribute>(true).ToImmutableArray();
			var type = MemberCache.Types[parameterInfo.ParameterType.TypeHandle];

			return new Parameter(parameterInfo.GetName(), attributes, parameterInfo.IsOptional, parameterInfo.IsOut, parameterInfo.DefaultValue, parameterInfo.HasDefaultValue, type);
		}

		public static Delegate CreateStaticDelegate(this MethodInfo @this)
		{
			var callParameters = @this.GetParameterInfos().ToArray(parameterInfo => parameterInfo!.Parameter());

			return callParameters.Any()
				? @this.CallStatic(callParameters).Lambda(callParameters).Compile()
				: @this.CallStatic().Lambda().Compile();
		}

		public static StaticInvokeType CreateStaticInvokeType(this MethodInfo @this)
		{
			ParameterExpression arguments = nameof(arguments).Parameter<object?[]?>();

			var callParameters = @this.GetParameterInfos()
				.To(parameterInfo => arguments.Array()[parameterInfo!.Position].SystemConvert(parameterInfo.ParameterType));
			var call = callParameters.Any() ? @this.CallStatic(callParameters) : @this.CallStatic();

			return @this.ReturnType == typeof(void)
				? call.Block(Expression.Constant(null)).Lambda<StaticInvokeType>(arguments).Compile()
				: call.As<object>().Lambda<StaticInvokeType>(arguments).Compile();
		}

		public static StaticMethodMember CreateStaticMember(this MethodInfo @this)
		{
			@this.IsStatic.Assert($"{nameof(@this)}.{nameof(@this.IsStatic)}", true);

			var attributes = @this.GetCustomAttributes<Attribute>(true).ToImmutableArray();
			var invoke = @this.CreateStaticInvokeType();
			var method = @this.CreateStaticDelegate();
			var parameters = @this.GetParameterInfos().To(CreateParameter!).ToImmutableArray();
			var returnAttributes = @this.ReturnParameter.GetCustomAttributes<Attribute>(true).ToImmutableArray();
			var returnType = MemberCache.Types[@this.ReturnType.TypeHandle];
			var returnParameter = new ReturnParameter(returnAttributes, returnType);

			return new StaticMethodMember(@this.GetName(), attributes, @this.IsAssembly, @this.IsPublic, @this.MethodHandle, invoke, method, parameters, returnParameter);
		}

		public static bool IsInvokable(this MethodBase @this)
			=> @this.GetParameters().All(_ => !_!.IsOut && _.ParameterType.IsInvokable());
	}
}
