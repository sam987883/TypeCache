// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;

namespace TypeCache.Reflection;

[DebuggerDisplay("Static method: {Name}")]
public sealed class StaticMethodEntity : Method
{
	private readonly Lazy<Delegate> _Invoke;
	private readonly Lazy<Func<object?[]?, object?>> _InvokeWithArray;
	private readonly Lazy<Func<ITuple, object?>> _InvokeWithTuple;

	public StaticMethodEntity(MethodInfo methodInfo) : base(methodInfo)
	{
		methodInfo.IsStatic.ThrowIfFalse();

		this._Invoke = new(this.CreateCall);
		this._InvokeWithArray = new(this.CreateArrayCall);
		this._InvokeWithTuple = new(this.CreateTupleCall);
	}

	public Delegate Delegate => this._Invoke.Value;

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public object? Invoke()
		=> this._InvokeWithArray.Value([]);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public object? Invoke(object?[] arguments)
		=> this._InvokeWithArray.Value(arguments);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public object? Invoke(ITuple arguments)
		=> this._InvokeWithTuple.Value(arguments);

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	private Delegate CreateCall()
	{
		var parameters = this.Parameters.Select(_ => _.ToExpression()).ToArray();
		var lambda = this.ToMethodInfo().ToExpression(null, parameters).Lambda(parameters);

		return lambda.Compile();
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	private Func<object?[]?, object?> CreateArrayCall()
	{
		ParameterExpression arguments = nameof(arguments).ToParameterExpression<object?[]?>();
		Expression[]? parameters = null;
		if (this.Parameters.Count > 0)
			parameters = this.Parameters.Index().Select(_ => arguments.Array().Index(_.Index).Convert(_.Item.ParameterType)).ToArray();

		Expression methodExpression = this.ToMethodInfo().ToExpression(null, parameters);

		if (this.HasReturnValue)
		{
			if (this.Return.ParameterType != typeof(object))
				methodExpression = methodExpression.As<object>();

			return methodExpression.Lambda<Func<object?[]?, object?>>([arguments]).Compile();
		}

		var action = methodExpression.Lambda<Action<object?[]?>>([arguments]).Compile();
		return arguments =>
		{
			action(arguments);
			return null;
		};
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	private Func<ITuple, object?> CreateTupleCall()
	{
		ParameterExpression arguments = nameof(arguments).ToParameterExpression<ITuple>();
		Expression[]? parameters = null;
		if (this.Parameters.Count > 0)
		{
			var valueTupleType = GetValueTupleType(this.Parameters);
			parameters = GetValueTupleFields(arguments.Cast(valueTupleType), this.Parameters.Count).ToArray();
		}

		Expression methodExpression = this.ToMethodInfo().ToExpression(null, parameters);

		if (this.HasReturnValue)
		{
			if (this.Return.ParameterType != typeof(object))
				methodExpression = methodExpression.Cast<object>();

			return methodExpression.Lambda<Func<ITuple, object?>>([arguments]).Compile();
		}

		var action = methodExpression.Lambda<Action<ITuple>>([arguments]).Compile();
		return arguments =>
		{
			action(arguments);
			return null;
		};
	}
}
