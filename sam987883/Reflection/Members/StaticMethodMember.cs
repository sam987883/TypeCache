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
	internal sealed class StaticMethodMember : Member, IStaticMethodMember
	{
		private readonly Func<object[]?, object?> _Invoke;

		public StaticMethodMember(MethodInfo methodInfo) : base(methodInfo)
		{
			methodInfo.IsStatic.Assert($"{nameof(methodInfo)}.{nameof(methodInfo.IsStatic)}", true);

			var parameterPositionComparer = Comparer<ParameterInfo>.Create((x, y) => x.Position - y.Position);

			this.Void = methodInfo.ReturnType == typeof(void);

			ParameterExpression parameters = nameof(parameters).Parameter<object[]>();

			var parameterInfos = methodInfo.GetParameters().Sort(parameterPositionComparer).ToArray();
			MethodCallExpression call;
			if (parameterInfos.Any())
			{
				call = Expression.Call(methodInfo, parameters.ToParameterArray(parameterInfos));
				
				var methodParameters = parameterInfos.To(parameterInfo => parameterInfo.Parameter()).ToArray(parameterInfos.Length);
				this.Method = Expression.Call(methodInfo, methodParameters).Lambda(methodParameters).Compile();
				this.Parameters = parameterInfos.To(parameterInfo => (IParameter)new Parameter(parameterInfo)).ToImmutableArray();
			}
			else
			{
				call = Expression.Call(methodInfo);
				
				this.Method = Expression.Call(methodInfo).Lambda().Compile();
				this.Parameters = ImmutableArray<IParameter>.Empty;
			}

			this._Invoke = methodInfo.ReturnType == typeof(void)
				? Expression.Block(call, Expression.Constant(null)).Lambda<Func<object[]?, object?>>(parameters).Compile()
				: call.As<object>().Lambda<Func<object[]?, object?>>(parameters).Compile();
		}

		public Delegate Method { get; }

		public IImmutableList<IParameter> Parameters { get; }

		public bool Void { get; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public object Invoke(params object[]? parameters) =>
			this._Invoke(parameters);

		public bool IsCallable(params object[]? arguments)
		{
			if (arguments?.Length > 0)
			{
				var argumentEnumerator = arguments.GetEnumerator();
				for (var i = 0; i < this.Parameters.Count; ++i)
				{
					var parameter = this.Parameters[i];
					if (!(argumentEnumerator.MoveNext() ? parameter.Supports(argumentEnumerator.Current.GetType()) : parameter.HasDefaultValue || parameter.Optional))
						break;
				}
				return !argumentEnumerator.MoveNext();
			}
			return this.Parameters.Count == 0 || this.Parameters.All(parameter => parameter.HasDefaultValue || parameter.Optional);
		}
	}
}
