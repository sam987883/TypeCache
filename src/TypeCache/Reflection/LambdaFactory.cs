// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using TypeCache.Extensions;

namespace TypeCache.Reflection;

public static class LambdaFactory
{
	public static LambdaExpression Create(Type[] parameterTypes, Func<ParameterExpression[], Expression> bodyFactory)
	{
		var parameters = parameterTypes.Select((type, i) => $"parameter{i + 1}".Parameter(type)).ToArray();
		return bodyFactory(parameters).Lambda(parameters);
	}

	public static Expression<Action<T>> CreateAction<T>(
		Func<ParameterExpression, Expression> bodyFactory)
	{
		ParameterExpression parameter = nameof(parameter).Parameter<T>();
		return Expression.Lambda<Action<T>>(bodyFactory(parameter), parameter);
	}

	public static LambdaExpression CreateAction(Type[] parameterTypes, Func<ParameterExpression[], Expression> bodyFactory)
	{
		var parameters = parameterTypes.Select((type, i) => $"parameter{i + 1}".Parameter(type)).ToArray();
		return bodyFactory(parameters).LambdaAction(parameters);
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

	public static LambdaExpression CreateFunc(Type[] parameterTypes, Type returnType, Func<ParameterExpression[], Expression> bodyFactory)
	{
		var parameters = parameterTypes.Select((type, i) => $"parameter{i + 1}".Parameter(type)).ToArray();
		return bodyFactory(parameters).LambdaFunc(returnType, parameters);
	}

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

	public static Expression<Predicate<T>> CreatePredicate<T>(Func<ParameterExpression, Expression> bodyFactory)
	{
		ParameterExpression value = nameof(value).Parameter<T>();
		return Expression.Lambda<Predicate<T>>(bodyFactory(value), value);
	}
}
