﻿// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;

namespace TypeCache.Reflection.Members
{
	internal sealed class MethodMember : Member, IMethodMember
	{
		public MethodMember(MethodInfo methodInfo) : base(methodInfo)
		{
			methodInfo.IsStatic.Assert($"{nameof(methodInfo)}.{nameof(methodInfo.IsStatic)}", false);

			var parameterPositionComparer = Comparer<ParameterInfo?>.Create((x, y) => x.Position - y.Position);

			this.IsVoid = methodInfo.ReturnType.IsVoid();
			this.ReturnAttributes = methodInfo.ReturnParameter.GetCustomAttributes(true).As<Attribute>().ToImmutableArray();

			ParameterExpression instance = nameof(instance).Parameter(typeof(object));
			ParameterExpression parameters = nameof(parameters).Parameter<object[]>();

			var parameterInfos = methodInfo.GetParameters().If(parameterInfo => parameterInfo != methodInfo.ReturnParameter).ToArray();
			parameterInfos.Sort(parameterPositionComparer);
			Expression call;
			if (parameterInfos.Any())
			{
				call = methodInfo.Call(instance.Cast(methodInfo.DeclaringType), parameters.ToParameterArray(parameterInfos));

				var callParameters = parameterInfos.To(parameterInfo => parameterInfo.Parameter()).ToArray(parameterInfos.Length);
				var methodParameters = new ParameterExpression[parameterInfos.Length + 1];
				methodParameters[0] = instance;
				callParameters.CopyTo(methodParameters, 1);

				this.Method = methodInfo.Call(instance.Cast(methodInfo.DeclaringType), callParameters).Lambda(methodParameters).Compile();
				this.Parameters = parameterInfos.To(parameterInfo => (IParameter)new Parameter(parameterInfo)).ToImmutableArray();
			}
			else
			{
				call = methodInfo.Call(instance.Cast(methodInfo.DeclaringType));

				this.Method = call.Lambda(instance).Compile();
				this.Parameters = ImmutableArray<IParameter>.Empty;
			}

			this.Invoke = methodInfo.ReturnType == typeof(void)
				? Expression.Block(call, Expression.Constant(null)).Lambda<Func<object, object?[]?, object?>>(instance, parameters).Compile()
				: call.As<object>().Lambda<Func<object, object?[]?, object?>>(instance, parameters).Compile();
		}

		public Func<object, object?[]?, object?> Invoke { get; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsCallableWith(params object?[]? arguments)
			=> this.Parameters.IsCallableWith(arguments);

		public bool IsVoid { get; }

		public Delegate Method { get; }

		public IImmutableList<IParameter> Parameters { get; }

		public IImmutableList<Attribute> ReturnAttributes { get; }
	}
}
