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
	internal sealed class ConstructorMember<T> : Member, IConstructorMember<T>
	{
		private readonly Func<object[], T> _Invoke;

		public ConstructorMember(ConstructorInfo constructorInfo) : base(constructorInfo)
		{
			var parameterPositionComparer = Comparer<ParameterInfo>.Create((x, y) => x.Position - y.Position);

			var parameterInfos = constructorInfo.GetParameters().Sort(parameterPositionComparer).ToArray();
			ParameterExpression parameters = nameof(parameters).Parameter<object[]>();

			if (parameterInfos.Any())
			{
				this._Invoke = Expression.New(constructorInfo, parameters.ToParameterArray(parameterInfos)).Lambda<Func<object[], T>>(parameters).Compile();

				var methodParameters = parameterInfos.To(parameterInfo => parameterInfo.Parameter()).ToArray(parameterInfos.Length);
				this.Method = Expression.New(constructorInfo, methodParameters).Lambda(methodParameters).Compile();
				this.Parameters = parameterInfos.To(parameterInfo => (IParameter)new Parameter(parameterInfo)).ToImmutableArray();
			}
			else
			{
				this._Invoke = Expression.New(constructorInfo).Lambda<Func<object[], T>>(parameters).Compile();

				this.Method = Expression.New(constructorInfo).Lambda().Compile();
				this.Parameters = ImmutableArray<IParameter>.Empty;
			}
		}

		public Delegate Method { get; }

		public IImmutableList<IParameter> Parameters { get; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T Invoke(params object[] parameters) =>
			this._Invoke(parameters);

		public bool IsCallable(params object[] arguments)
		{
			if (arguments.Any())
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
