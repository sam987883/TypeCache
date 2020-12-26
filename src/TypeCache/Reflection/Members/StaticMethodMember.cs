// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;

namespace TypeCache.Reflection.Members
{
	internal sealed class StaticMethodMember : Member, IStaticMethodMember
	{
		private readonly Func<object[]?, object?> _Invoke;

		public StaticMethodMember(MethodInfo methodInfo) : base(methodInfo)
		{
			methodInfo.IsStatic.Assert($"{nameof(methodInfo)}.{nameof(methodInfo.IsStatic)}", true);

			var parameterPositionComparer = Comparer<ParameterInfo>.Create((x, y) => x.Position - y.Position);

			this.IsTask = methodInfo.ReturnType.IsTask();
			this.IsValueTask = methodInfo.ReturnType.IsValueTask();
			this.IsVoid = methodInfo.ReturnType.IsVoid();
			this.ReturnAttributes = methodInfo.ReturnParameter.GetCustomAttributes(true).As<Attribute>().ToImmutableArray();

			ParameterExpression parameters = nameof(parameters).Parameter<object[]>();

			var parameterInfos = methodInfo.GetParameters().If(parameterInfo => parameterInfo != methodInfo.ReturnParameter).ToArray();
			parameterInfos.Sort(parameterPositionComparer);
			MethodCallExpression call;
			if (parameterInfos.Any())
			{
				call = Expression.Call(methodInfo, parameters.ToParameterArray(parameterInfos));

				var methodParameters = parameterInfos.To(parameterInfo => parameterInfo.Parameter()).ToArrayOf(parameterInfos.Length);
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public object? Invoke(params object[]? parameters)
			=> this._Invoke(parameters);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsCallableWith(params object[]? arguments)
			=> this.Parameters.IsCallableWith(arguments);

		public bool IsTask { get; }

		public bool IsValueTask { get; }

		public bool IsVoid { get; }

		public Delegate Method { get; }

		public IImmutableList<IParameter> Parameters { get; }

		public IImmutableList<Attribute> ReturnAttributes { get; }
	}
}
