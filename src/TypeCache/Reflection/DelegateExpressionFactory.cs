using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection
{
	public static class DelegateExpressionFactory
	{
		public static Expression<CreateType> Create(ConstructorInfo constructorInfo)
		{
			var parameterInfos = constructorInfo.GetParameters();
			ParameterExpression arguments = nameof(arguments).Parameter<object[]>();

			if (parameterInfos.Any())
			{
				parameterInfos.Sort(Default.ParameterPositionComparer);

				var constructorParameters = parameterInfos.To(parameterInfo => arguments.Array()[parameterInfo!.Position].SystemConvert(parameterInfo.ParameterType));
				var lambdaExpression = constructorInfo.New(constructorParameters).As<object>().Lambda<CreateType>(arguments);
				return lambdaExpression;
			}
			else
			{
				var lambdaExpression = constructorInfo.New().As<object>().Lambda<CreateType>(arguments);
				return lambdaExpression;
			}
		}

		public static Expression<InvokeType> Create(MethodInfo methodInfo)
		{
			ParameterExpression instance = nameof(instance).Parameter<object>();
			ParameterExpression arguments = nameof(arguments).Parameter<object[]>();
			MethodCallExpression call;

			var parameterInfos = methodInfo.GetParameters();
			if (parameterInfos.Any())
			{
				parameterInfos.Sort(Default.ParameterPositionComparer);

				var methodParameters = parameterInfos.To(parameterInfo => arguments.Array()[parameterInfo!.Position].SystemConvert(parameterInfo.ParameterType));
				call = !methodInfo.IsStatic
					? instance.Cast(methodInfo.DeclaringType!).Call(methodInfo, methodParameters)
					: methodInfo.CallStatic(methodParameters);
			}
			else
			{
				call = !methodInfo.IsStatic
					? instance.Cast(methodInfo.DeclaringType!).Call(methodInfo)
					: methodInfo.CallStatic();
			}

			var delegateExpression = methodInfo.ReturnType == typeof(void)
				? call.Block(NullExpression).Lambda<InvokeType>(instance, arguments)
				: call.As<object>().Lambda<InvokeType>(instance, arguments);
			return delegateExpression;
		}

		public static Expression<Action<T>> CreateAction<T>(
			Func<ParameterExpression, Expression> bodyFactory)
		{
			ParameterExpression parameter = nameof(parameter).Parameter<T>();
			return Expression.Lambda<Action<T>>(bodyFactory(parameter), parameter);
		}

		public static Expression<Action<T1, T2>> CreateAction<T1, T2>(
			Func<ParameterExpression, ParameterExpression, Expression> bodyFactory)
		{
			var parameters = new[]
			{
				"parameter1".Parameter<T1>(),
				"parameter2".Parameter<T2>()
			};
			return Expression.Lambda<Action<T1, T2>>(bodyFactory(parameters[0], parameters[1]), parameters);
		}

		public static Expression<Action<T1, T2, T3>> CreateAction<T1, T2, T3>(
			Func<ParameterExpression, ParameterExpression, ParameterExpression, Expression> bodyFactory)
		{
			var parameters = new[]
			{
				"parameter1".Parameter<T1>(),
				"parameter2".Parameter<T2>(),
				"parameter3".Parameter<T3>()
			};
			return Expression.Lambda<Action<T1, T2, T3>>(bodyFactory(parameters[0], parameters[1], parameters[2]), parameters);
		}

		public static Expression<Action<T1, T2, T3, T4>> CreateAction<T1, T2, T3, T4>(
			Func<ParameterExpression, ParameterExpression, ParameterExpression, ParameterExpression, Expression> bodyFactory)
		{
			var parameters = new[]
			{
				"parameter1".Parameter<T1>(),
				"parameter2".Parameter<T2>(),
				"parameter3".Parameter<T3>(),
				"parameter4".Parameter<T4>()
			};
			return Expression.Lambda<Action<T1, T2, T3, T4>>(bodyFactory(parameters[0], parameters[1], parameters[2], parameters[3]), parameters);
		}

		public static Expression<Action<T1, T2, T3, T4, T5>> CreateAction<T1, T2, T3, T4, T5>(
			Func<ParameterExpression, ParameterExpression, ParameterExpression, ParameterExpression, ParameterExpression, Expression> bodyFactory)
		{
			var parameters = new[]
			{
				"parameter1".Parameter<T1>(),
				"parameter2".Parameter<T2>(),
				"parameter3".Parameter<T3>(),
				"parameter4".Parameter<T4>(),
				"parameter5".Parameter<T5>()
			};
			return Expression.Lambda<Action<T1, T2, T3, T4, T5>>(bodyFactory(parameters[0], parameters[1], parameters[2], parameters[3], parameters[4]), parameters);
		}

		public static Expression<Action<T1, T2, T3, T4, T5, T6>> CreateAction<T1, T2, T3, T4, T5, T6>(
			Func<ParameterExpression, ParameterExpression, ParameterExpression, ParameterExpression, ParameterExpression, ParameterExpression, Expression> bodyFactory)
		{
			var parameters = new[]
			{
				"parameter1".Parameter<T1>(),
				"parameter2".Parameter<T2>(),
				"parameter3".Parameter<T3>(),
				"parameter4".Parameter<T4>(),
				"parameter5".Parameter<T5>(),
				"parameter6".Parameter<T6>()
			};
			return Expression.Lambda<Action<T1, T2, T3, T4, T5, T6>>(bodyFactory(parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], parameters[5]), parameters);
		}

		public static Expression<Comparison<T>> CreateComparison<T>(
			Func<ParameterExpression, ParameterExpression, Expression> bodyFactory)
		{
			ParameterExpression value1 = nameof(value1).Parameter<T>();
			ParameterExpression value2 = nameof(value2).Parameter<T>();
			return Expression.Lambda<Comparison<T>>(bodyFactory(value1, value2), value1, value2);
		}

		public static Expression<Func<R>> CreateFunc<R>(Expression body)
			=> Expression.Lambda<Func<R>>(body);

		public static Expression<Func<T, R>> CreateFunc<T, R>(Func<ParameterExpression, Expression> bodyFactory)
		{
			ParameterExpression parameter = nameof(parameter).Parameter<T>();
			return Expression.Lambda<Func<T, R>>(bodyFactory(parameter), parameter);
		}

		public static Expression<Func<T1, T2, R>> CreateFunc<T1, T2, R>(
			Func<ParameterExpression, ParameterExpression, Expression> bodyFactory)
		{
			var parameters = new[]
			{
				"parameter1".Parameter<T1>(),
				"parameter2".Parameter<T2>()
			};
			return Expression.Lambda<Func<T1, T2, R>>(bodyFactory(parameters[0], parameters[1]), parameters);
		}

		public static Expression<Func<T1, T2, T3, R>> CreateFunc<T1, T2, T3, R>(
			Func<ParameterExpression, ParameterExpression, ParameterExpression, Expression> bodyFactory)
		{
			var parameters = new[]
			{
				"parameter1".Parameter<T1>(),
				"parameter2".Parameter<T2>(),
				"parameter3".Parameter<T3>()
			};
			return Expression.Lambda<Func<T1, T2, T3, R>>(bodyFactory(parameters[0], parameters[1], parameters[2]), parameters);
		}

		public static Expression<Func<T1, T2, T3, T4, R>> CreateFunc<T1, T2, T3, T4, R>(
			Func<ParameterExpression, ParameterExpression, ParameterExpression, ParameterExpression, Expression> bodyFactory)
		{
			var parameters = new[]
			{
				"parameter1".Parameter<T1>(),
				"parameter2".Parameter<T2>(),
				"parameter3".Parameter<T3>(),
				"parameter4".Parameter<T4>()
			};
			return Expression.Lambda<Func<T1, T2, T3, T4, R>>(bodyFactory(parameters[0], parameters[1], parameters[2], parameters[3]), parameters);
		}

		public static Expression<Func<T1, T2, T3, T4, T5, R>> CreateFunc<T1, T2, T3, T4, T5, R>(
			Func<ParameterExpression, ParameterExpression, ParameterExpression, ParameterExpression, ParameterExpression, Expression> bodyFactory)
		{
			var parameters = new[]
			{
				"parameter1".Parameter<T1>(),
				"parameter2".Parameter<T2>(),
				"parameter3".Parameter<T3>(),
				"parameter4".Parameter<T4>(),
				"parameter5".Parameter<T5>()
			};
			return Expression.Lambda<Func<T1, T2, T3, T4, T5, R>>(bodyFactory(parameters[0], parameters[1], parameters[2], parameters[3], parameters[4]), parameters);
		}

		public static Expression<Func<T1, T2, T3, T4, T5, T6, R>> CreateFunc<T1, T2, T3, T4, T5, T6, R>(
			Func<ParameterExpression, ParameterExpression, ParameterExpression, ParameterExpression, ParameterExpression, ParameterExpression, Expression> bodyFactory)
		{
			var parameters = new[]
			{
				"parameter1".Parameter<T1>(),
				"parameter2".Parameter<T2>(),
				"parameter3".Parameter<T3>(),
				"parameter4".Parameter<T4>(),
				"parameter5".Parameter<T5>(),
				"parameter6".Parameter<T6>()
			};
			return Expression.Lambda<Func<T1, T2, T3, T4, T5, T6, R>>(bodyFactory(parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], parameters[5]), parameters);
		}

		public static Expression<GetValue> CreateGetter(FieldInfo fieldInfo)
		{
			ParameterExpression instance = nameof(instance).Parameter<object>();
			return !fieldInfo.IsStatic
				? instance.Cast(fieldInfo.DeclaringType!).Field(fieldInfo).As<object>().Lambda<GetValue>(instance)
				: fieldInfo.StaticField().As<object>().Lambda<GetValue>(instance);
		}

		public static Expression<Predicate<T>> CreatePredicate<T>(Func<ParameterExpression, Expression> bodyFactory)
		{
			ParameterExpression value = nameof(value).Parameter<T>();
			return Expression.Lambda<Predicate<T>>(bodyFactory(value), value);
		}

		public static Expression<SetValue> CreateSetter(FieldInfo fieldInfo)
		{
			fieldInfo.IsInitOnly.Assert($"{nameof(FieldInfo)}.{nameof(fieldInfo.IsInitOnly)}", false);
			fieldInfo.IsLiteral.Assert($"{nameof(FieldInfo)}.{nameof(fieldInfo.IsLiteral)}", false);

			ParameterExpression instance = nameof(instance).Parameter<object>();
			ParameterExpression value = nameof(value).Parameter<object>();

			return !fieldInfo.IsStatic
				? instance.Cast(fieldInfo.DeclaringType!).Field(fieldInfo).Assign(value.SystemConvert(fieldInfo.FieldType)).Lambda<SetValue>(instance, value)
				: fieldInfo.StaticField().Assign(value.SystemConvert(fieldInfo.FieldType)).Lambda<SetValue>(instance, value);
		}
	}
}
