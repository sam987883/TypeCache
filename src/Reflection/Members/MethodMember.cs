// Copyright (c) 2020 Samuel Abraham

using Sam987883.Reflection.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using static Sam987883.Common.Extensions.ExpressionExtensions;
using static Sam987883.Common.Extensions.IEnumerableExtensions;
using static Sam987883.Common.Extensions.ObjectExtensions;

namespace Sam987883.Reflection.Members
{
	internal sealed class MethodMember<T> : Member, IMethodMember<T>
		where T : class
	{
		private readonly Func<T, object?[]?, object?> _Invoke;

		public MethodMember(MethodInfo methodInfo) : base(methodInfo)
		{
			methodInfo.IsStatic.Assert($"{nameof(methodInfo)}.{nameof(methodInfo.IsStatic)}", false);

			var parameterPositionComparer = Comparer<ParameterInfo>.Create((x, y) => x.Position - y.Position);

			this.Void = methodInfo.ReturnType == typeof(void);

			ParameterExpression instance = nameof(instance).Parameter<T>();
			ParameterExpression parameters = nameof(parameters).Parameter<object[]>();

			var parameterInfos = methodInfo.GetParameters().Sort(parameterPositionComparer).ToList().ToArray();
			Expression call;
			if (parameterInfos.Any())
			{
				call = instance.Call(methodInfo, parameters.ToParameterArray(parameterInfos));

				var callParameters = parameterInfos.To(parameterInfo => parameterInfo.Parameter()).ToArrayOf(parameterInfos.Length);
				var methodParameters = new ParameterExpression[parameterInfos.Length + 1];
				methodParameters[0] = instance;
				callParameters.CopyTo(methodParameters, 1);

				this.Method = instance.Call(methodInfo, callParameters).Lambda(methodParameters).Compile();
				this.Parameters = parameterInfos.To(parameterInfo => (IParameter)new Parameter(parameterInfo)).ToImmutableArray();
			}
			else
			{
				call = instance.Call(methodInfo);

				this.Method = call.Lambda(instance).Compile();
				this.Parameters = ImmutableArray<IParameter>.Empty;
			}

			this._Invoke = methodInfo.ReturnType == typeof(void)
				? Expression.Block(call, Expression.Constant(null)).Lambda<Func<T, object?[]?, object?>>(instance, parameters).Compile()
				: call.As<object>().Lambda<Func<T, object?[]?, object?>>(instance, parameters).Compile();
		}

		public Delegate Method { get; }

		public IImmutableList<IParameter> Parameters { get; }

		public bool Void { get; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public object? Invoke(T instance, params object?[]? parameters) =>
			this._Invoke(instance, parameters);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsCallableWith(params object?[]? arguments) =>
			this.Parameters.IsCallableWith(arguments);
	}
}
