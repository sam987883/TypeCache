// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Reflection.Extensions
{
	public static class ReflectionExtensions
	{
		private static readonly IComparer<ParameterInfo> ParameterSortComparer = Comparer<ParameterInfo>.Create((x, y) => x.Position - y.Position)!;

		public static string GetName(this MemberInfo @this)
			=> @this.GetCustomAttribute<NameAttribute>()?.Name ?? (@this.Name.Contains('`') ? @this.Name.Left(@this.Name.IndexOf('`')) : @this.Name);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetName(this ParameterInfo @this)
			=> @this.GetCustomAttribute<NameAttribute>()?.Name ?? @this.Name!;

		public static bool IsInvokable(this MethodBase @this)
			=> @this.GetParameters().All(_ => !_!.IsOut && _.ParameterType.IsInvokable());

		public static CreateType ToCreateType(this ConstructorInfo @this)
		{
			ParameterExpression arguments = nameof(arguments).Parameter<object?[]?>();

			var parameterInfos = @this.GetParameters();
			parameterInfos.Sort(ParameterSortComparer);

			var callParameters = parameterInfos.To(parameterInfo => arguments.Array()[parameterInfo!.Position].SystemConvert(parameterInfo.ParameterType));
			return callParameters.Any()
				? @this.New(callParameters).As<object>().Lambda<CreateType>(arguments).Compile()
				: @this.New().As<object>().Lambda<CreateType>(arguments).Compile();
		}

		public static Delegate ToDelegate(this ConstructorInfo @this)
		{
			var parameterInfos = @this.GetParameters();
			parameterInfos.Sort(ParameterSortComparer);

			var methodParameters = parameterInfos.ToArray(parameterInfo => parameterInfo.Parameter());
			return methodParameters.Any()
				? @this.New(methodParameters).Lambda(methodParameters).Compile()
				: @this.New().Lambda().Compile();
		}

		public static Delegate ToDelegate(this MethodInfo @this)
		{
			var parameterInfos = @this.GetParameters();
			parameterInfos.Sort(ParameterSortComparer);

			var parameters = parameterInfos.ToArray(parameterInfo => parameterInfo!.Parameter());
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

		public static Delegate ToGetter(this FieldInfo @this)
		{
			ParameterExpression instance = nameof(instance).Parameter(@this.DeclaringType!);
			return !@this.IsStatic
				? instance.Field(@this).Lambda(instance).Compile()
				: @this.StaticField().Lambda().Compile();
		}

		public static GetValue ToGetValue(this FieldInfo @this)
		{
			ParameterExpression instance = nameof(instance).Parameter<object>();
			var field = !@this.IsStatic ? instance.Cast(@this.DeclaringType!).Field(@this) : @this.StaticField();
			return field.As<object>().Lambda<GetValue>(instance).Compile();
		}

		public static InvokeType ToInvokeType(this MethodInfo @this)
		{
			var parameterInfos = @this.GetParameters();
			parameterInfos.Sort(ParameterSortComparer);

			ParameterExpression instance = nameof(instance).Parameter<object>();
			ParameterExpression arguments = nameof(arguments).Parameter<object?[]?>();

			var callParameters = parameterInfos.To(parameterInfo => arguments.Array()[parameterInfo!.Position].SystemConvert(parameterInfo.ParameterType));
			var instanceCast = instance.Cast(@this.DeclaringType!);
			var body = !@this.IsStatic
				? callParameters.Any() ? instanceCast.Call(@this, callParameters) : instanceCast.Call(@this)
				: callParameters.Any() ? @this.CallStatic(callParameters) : @this.CallStatic();

			return @this.ReturnType == typeof(void)
				? body.Block(Expression.Constant(null)).Lambda<InvokeType>(instance, arguments).Compile()
				: body.As<object>().Lambda<InvokeType>(instance, arguments).Compile();
		}

		public static Delegate ToSetter(this FieldInfo @this)
		{
			@this.IsInitOnly.Assert($"{nameof(FieldInfo)}.{nameof(@this.IsInitOnly)}", false);
			@this.IsLiteral.Assert($"{nameof(FieldInfo)}.{nameof(@this.IsLiteral)}", false);

			ParameterExpression value = nameof(value).Parameter(@this.FieldType);
			if (!@this.IsStatic)
			{
				ParameterExpression instance = nameof(instance).Parameter(@this.DeclaringType!);
				return instance.Field(@this).Assign(value).LambdaAction(instance, value).Compile();
			}
			else
				return @this.StaticField().Assign(value).LambdaAction(value).Compile();
		}

		public static SetValue ToSetValue(this FieldInfo @this)
		{
			@this.IsInitOnly.Assert($"{nameof(FieldInfo)}.{nameof(@this.IsInitOnly)}", false);
			@this.IsLiteral.Assert($"{nameof(FieldInfo)}.{nameof(@this.IsLiteral)}", false);

			ParameterExpression value = nameof(value).Parameter<object>();
			ParameterExpression instance = nameof(instance).Parameter<object>();
			var field = !@this.IsStatic ? instance.Cast(@this.DeclaringType!).Field(@this) : @this.StaticField();
			return field.Assign(value.SystemConvert(@this.FieldType)).Lambda<SetValue>(instance, value).Compile();
		}
	}
}
