// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using TypeCache.Extensions;

namespace TypeCache.Utilities;

public static class LambdaFactory
{
	public static LambdaExpression Create(Type[] parameterTypes, Func<ParameterExpression[], Expression> lambda)
	{
		var parameters = parameterTypes.Select((type, i) => Invariant($"parameter{i + 1}").ToParameterExpression(type)).ToArray();
		return lambda(parameters).Lambda(parameters);
	}

	public static Expression<Action<T>> CreateAction<T>(
		Func<ParameterExpression, Expression> lambda)
	{
		ParameterExpression parameter = nameof(parameter).ToParameterExpression<T>();
		return Expression.Lambda<Action<T>>(lambda(parameter), parameter);
	}

	public static LambdaExpression CreateAction(Type[] parameterTypes, Func<ParameterExpression[], Expression> lambda)
	{
		var parameters = parameterTypes.Select((type, i) => Invariant($"parameter{i + 1}").ToParameterExpression(type)).ToArray();
		return lambda(parameters).LambdaAction(parameters);
	}

	public static Expression<Action<T1, T2>> CreateAction<T1, T2>(
		Func<ParameterExpression, ParameterExpression, Expression> lambda)
	{
		ParameterExpression parameter1 = nameof(parameter1).ToParameterExpression<T1>();
		ParameterExpression parameter2 = nameof(parameter2).ToParameterExpression<T2>();
		return Expression.Lambda<Action<T1, T2>>(lambda(parameter1, parameter2), parameter1, parameter2);
	}

	public static Expression<Action<T1, T2, T3>> CreateAction<T1, T2, T3>(
		Func<ParameterExpression, ParameterExpression, ParameterExpression, Expression> lambda)
	{
		ParameterExpression parameter1 = nameof(parameter1).ToParameterExpression<T1>();
		ParameterExpression parameter2 = nameof(parameter2).ToParameterExpression<T2>();
		ParameterExpression parameter3 = nameof(parameter3).ToParameterExpression<T3>();
		return Expression.Lambda<Action<T1, T2, T3>>(lambda(parameter1, parameter2, parameter3), parameter1, parameter2, parameter3);
	}

	public static Expression<Action<T1, T2, T3, T4>> CreateAction<T1, T2, T3, T4>(
		Func<ParameterExpression, ParameterExpression, ParameterExpression, ParameterExpression, Expression> lambda)
	{
		ParameterExpression parameter1 = nameof(parameter1).ToParameterExpression<T1>();
		ParameterExpression parameter2 = nameof(parameter2).ToParameterExpression<T2>();
		ParameterExpression parameter3 = nameof(parameter3).ToParameterExpression<T3>();
		ParameterExpression parameter4 = nameof(parameter4).ToParameterExpression<T4>();
		return Expression.Lambda<Action<T1, T2, T3, T4>>(lambda(parameter1, parameter2, parameter3, parameter4), parameter1, parameter2, parameter3, parameter4);
	}

	public static Expression<Action<T1, T2, T3, T4, T5>> CreateAction<T1, T2, T3, T4, T5>(
		Func<ParameterExpression, ParameterExpression, ParameterExpression, ParameterExpression, ParameterExpression, Expression> lambda)
	{
		ParameterExpression parameter1 = nameof(parameter1).ToParameterExpression<T1>();
		ParameterExpression parameter2 = nameof(parameter2).ToParameterExpression<T2>();
		ParameterExpression parameter3 = nameof(parameter3).ToParameterExpression<T3>();
		ParameterExpression parameter4 = nameof(parameter4).ToParameterExpression<T4>();
		ParameterExpression parameter5 = nameof(parameter5).ToParameterExpression<T5>();
		return Expression.Lambda<Action<T1, T2, T3, T4, T5>>(lambda(parameter1, parameter2, parameter3, parameter4, parameter5), parameter1, parameter2, parameter3, parameter4, parameter5);
	}

	public static Expression<Action<T1, T2, T3, T4, T5, T6>> CreateAction<T1, T2, T3, T4, T5, T6>(
		Func<ParameterExpression, ParameterExpression, ParameterExpression, ParameterExpression, ParameterExpression, ParameterExpression, Expression> lambda)
	{
		ParameterExpression parameter1 = nameof(parameter1).ToParameterExpression<T1>();
		ParameterExpression parameter2 = nameof(parameter2).ToParameterExpression<T2>();
		ParameterExpression parameter3 = nameof(parameter3).ToParameterExpression<T3>();
		ParameterExpression parameter4 = nameof(parameter4).ToParameterExpression<T4>();
		ParameterExpression parameter5 = nameof(parameter5).ToParameterExpression<T5>();
		ParameterExpression parameter6 = nameof(parameter6).ToParameterExpression<T6>();
		return Expression.Lambda<Action<T1, T2, T3, T4, T5, T6>>(lambda(parameter1, parameter2, parameter3, parameter4, parameter5, parameter6), parameter1, parameter2, parameter3, parameter4, parameter5, parameter6);
	}

	public static Expression<Comparison<T>> CreateComparison<T>(
		Func<ParameterExpression, ParameterExpression, Expression> lambda)
	{
		ParameterExpression value1 = nameof(value1).ToParameterExpression<T>();
		ParameterExpression value2 = nameof(value2).ToParameterExpression<T>();
		return Expression.Lambda<Comparison<T>>(lambda(value1, value2), value1, value2);
	}

	public static LambdaExpression CreateFunc(Type[] parameterTypes, Type returnType, Func<ParameterExpression[], Expression> lambda)
	{
		var parameters = parameterTypes.Select((type, i) => Invariant($"parameter{i + 1}").ToParameterExpression(type)).ToArray();
		return lambda(parameters).LambdaFunc(returnType, parameters);
	}

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static LambdaExpression CreateFunc<T, R>(Expression<Func<T, R>> expression)
		=> expression;

	public static Expression<Func<T, R>> CreateFunc<T, R>(Func<ParameterExpression, Expression> lambda)
	{
		ParameterExpression parameter = nameof(parameter).ToParameterExpression<T>();
		return Expression.Lambda<Func<T, R>>(lambda(parameter), parameter);
	}

	public static Expression<Func<T1, T2, R>> CreateFunc<T1, T2, R>(
		Func<ParameterExpression, ParameterExpression, Expression> lambda)
	{
		ParameterExpression parameter1 = nameof(parameter1).ToParameterExpression<T1>();
		ParameterExpression parameter2 = nameof(parameter2).ToParameterExpression<T2>();
		return Expression.Lambda<Func<T1, T2, R>>(lambda(parameter1, parameter2), parameter1, parameter2);
	}

	public static Expression<Func<T1, T2, T3, R>> CreateFunc<T1, T2, T3, R>(
		Func<ParameterExpression, ParameterExpression, ParameterExpression, Expression> lambda)
	{
		ParameterExpression parameter1 = nameof(parameter1).ToParameterExpression<T1>();
		ParameterExpression parameter2 = nameof(parameter2).ToParameterExpression<T2>();
		ParameterExpression parameter3 = nameof(parameter3).ToParameterExpression<T3>();
		return Expression.Lambda<Func<T1, T2, T3, R>>(lambda(parameter1, parameter2, parameter3), parameter1, parameter2, parameter3);
	}

	public static Expression<Func<T1, T2, T3, T4, R>> CreateFunc<T1, T2, T3, T4, R>(
		Func<ParameterExpression, ParameterExpression, ParameterExpression, ParameterExpression, Expression> lambda)
	{
		ParameterExpression parameter1 = nameof(parameter1).ToParameterExpression<T1>();
		ParameterExpression parameter2 = nameof(parameter2).ToParameterExpression<T2>();
		ParameterExpression parameter3 = nameof(parameter3).ToParameterExpression<T3>();
		ParameterExpression parameter4 = nameof(parameter4).ToParameterExpression<T4>();
		return Expression.Lambda<Func<T1, T2, T3, T4, R>>(lambda(parameter1, parameter2, parameter3, parameter4), parameter1, parameter2, parameter3, parameter4);
	}

	public static Expression<Func<T1, T2, T3, T4, T5, R>> CreateFunc<T1, T2, T3, T4, T5, R>(
		Func<ParameterExpression, ParameterExpression, ParameterExpression, ParameterExpression, ParameterExpression, Expression> lambda)
	{
		ParameterExpression parameter1 = nameof(parameter1).ToParameterExpression<T1>();
		ParameterExpression parameter2 = nameof(parameter2).ToParameterExpression<T2>();
		ParameterExpression parameter3 = nameof(parameter3).ToParameterExpression<T3>();
		ParameterExpression parameter4 = nameof(parameter4).ToParameterExpression<T4>();
		ParameterExpression parameter5 = nameof(parameter5).ToParameterExpression<T5>();
		return Expression.Lambda<Func<T1, T2, T3, T4, T5, R>>(lambda(parameter1, parameter2, parameter3, parameter4, parameter5), parameter1, parameter2, parameter3, parameter4, parameter5);
	}

	public static Expression<Func<T1, T2, T3, T4, T5, T6, R>> CreateFunc<T1, T2, T3, T4, T5, T6, R>(
		Func<ParameterExpression, ParameterExpression, ParameterExpression, ParameterExpression, ParameterExpression, ParameterExpression, Expression> lambda)
	{
		ParameterExpression parameter1 = nameof(parameter1).ToParameterExpression<T1>();
		ParameterExpression parameter2 = nameof(parameter2).ToParameterExpression<T2>();
		ParameterExpression parameter3 = nameof(parameter3).ToParameterExpression<T3>();
		ParameterExpression parameter4 = nameof(parameter4).ToParameterExpression<T4>();
		ParameterExpression parameter5 = nameof(parameter5).ToParameterExpression<T5>();
		ParameterExpression parameter6 = nameof(parameter6).ToParameterExpression<T6>();
		return Expression.Lambda<Func<T1, T2, T3, T4, T5, T6, R>>(lambda(parameter1, parameter2, parameter3, parameter4, parameter5, parameter6), parameter1, parameter2, parameter3, parameter4, parameter5, parameter6);
	}

	public static Expression<Predicate<T>> CreatePredicate<T>(Func<ParameterExpression, Expression> lambda)
	{
		ParameterExpression value = nameof(value).ToParameterExpression<T>();
		return Expression.Lambda<Predicate<T>>(lambda(value), value);
	}
}
