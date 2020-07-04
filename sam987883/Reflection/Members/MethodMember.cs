// Copyright (c) 2020 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using static sam987883.Extensions.ExpressionExtensions;
using static sam987883.Extensions.IEnumerableExtensions;
using static sam987883.Extensions.ObjectExtensions;

namespace sam987883.Reflection.Members
{
	internal sealed class MethodMember<T> : Member, IMethodMember<T>
	{
		private readonly Func<T, object[]?, object?> _Invoke;

		public MethodMember(MethodInfo methodInfo) : base(methodInfo)
		{
			methodInfo.IsStatic.Assert($"{nameof(methodInfo)}.{nameof(methodInfo.IsStatic)}", false);

			var parameterPositionComparer = Comparer<ParameterInfo>.Create((x, y) => x.Position - y.Position);

			this.Void = methodInfo.ReturnType == typeof(void);

			ParameterExpression instance = nameof(instance).Parameter<T>();
			ParameterExpression parameters = nameof(parameters).Parameter<object[]>();

			var parameterInfos = methodInfo.GetParameters().Sort(parameterPositionComparer).ToArray();
			MethodCallExpression call;
			if (parameterInfos.Any())
			{
				call = Expression.Call(instance, methodInfo, parameters.ToParameterArray(parameterInfos));

				var callParameters = parameterInfos.To(parameterInfo => parameterInfo.Parameter()).ToArray(parameterInfos.Length);
				var methodParameters = new ParameterExpression[parameterInfos.Length + 1];
				methodParameters[0] = instance;
				callParameters.CopyTo(methodParameters, 1);

				this.Method = instance.Call(methodInfo, callParameters).Lambda(methodParameters).Compile();
				this.Parameters = parameterInfos.To(parameterInfo => (IParameter)new Parameter(parameterInfo)).ToImmutableArray();
			}
			else
			{
				call = (MethodCallExpression)instance.Call(methodInfo);

				this.Method = call.Lambda(instance).Compile();
				this.Parameters = ImmutableArray<IParameter>.Empty;
			}

			this._Invoke = methodInfo.ReturnType == typeof(void)
				? Expression.Block(call, Expression.Constant(null)).Lambda<Func<T, object[]?, object?>>(instance, parameters).Compile()
				: call.As<object>().Lambda<Func<T, object[]?, object?>>(instance, parameters).Compile();
		}

		public Delegate Method { get; }

		public IImmutableList<IParameter> Parameters { get; }

		public bool Void { get; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public object? Invoke(T instance, params object[]? parameters) =>
			this._Invoke(instance, parameters);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsCallableWith(params object[]? arguments) =>
			this.Parameters.IsCallableWith(arguments);
	}
}
