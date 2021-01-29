// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;

namespace TypeCache.Reflection.Members
{
	internal sealed class ConstructorMember : Member, IConstructorMember
	{
		public ConstructorMember(ConstructorInfo constructorInfo) : base(constructorInfo)
		{
			var parameterPositionComparer = Comparer<ParameterInfo>.Create((x, y) => x.Position - y.Position);

			var parameterInfos = constructorInfo.GetParameters();
			parameterInfos.Sort(parameterPositionComparer);
			ParameterExpression parameters = nameof(parameters).Parameter<object?[]?>();

			if (parameterInfos.Any())
			{
				this.Invoke = constructorInfo.New(parameters.ToParameterArray(parameterInfos)).Lambda<Func<object?[]?, object>>(parameters).Compile();

				var methodParameters = parameterInfos.To(parameterInfo => parameterInfo.Parameter()).ToArray(parameterInfos.Length);
				this.Method = constructorInfo.New(methodParameters).Lambda(methodParameters).Compile();
				this.Parameters = parameterInfos.To(parameterInfo => (IParameter)new Parameter(parameterInfo)).ToImmutableArray();
			}
			else
			{
				this.Invoke = constructorInfo.New().Lambda<Func<object?[]?, object>>(parameters).Compile();

				this.Method = constructorInfo.New().Lambda().Compile();
				this.Parameters = ImmutableArray<IParameter>.Empty;
			}
		}

		public Func<object?[]?, object> Invoke { get; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsCallableWith(params object?[]? arguments) =>
			this.Parameters.IsCallableWith(arguments);

		public Delegate Method { get; }

		public IImmutableList<IParameter> Parameters { get; }
	}
}
