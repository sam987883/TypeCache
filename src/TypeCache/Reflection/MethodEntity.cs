// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;

namespace TypeCache.Reflection;

[DebuggerDisplay("Method: {Name}")]
public sealed class MethodEntity : Method
{
	private readonly Lazy<Delegate> _Invoke;
	private readonly Lazy<Func<object, object?[]?, object?>> _InvokeWithArray;
	private readonly Lazy<Func<object, ITuple, object?>> _InvokeWithTuple;

	public MethodEntity(MethodInfo methodInfo) : base(methodInfo)
	{
		methodInfo.IsGenericMethodDefinition.ThrowIfTrue();
		methodInfo.IsStatic.ThrowIfTrue();

		this._Invoke = new(this.CreateCall);
		this._InvokeWithArray = new(this.CreateArrayCall);
		this._InvokeWithTuple = new(this.CreateTupleCall);

		this.HasReturnValue = methodInfo.ReturnType != typeof(void);
		this.Return = new(methodInfo.ReturnParameter);
	}

	public Delegate Delegate => this._Invoke.Value;

	public bool HasReturnValue { get; }

	public ParameterEntity Return { get; }

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public object? Invoke(object instance)
		=> this._InvokeWithArray.Value(instance, null);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public object? Invoke(object instance, object?[] arguments)
		=> this._InvokeWithArray.Value(instance, arguments);

	public object? Invoke(object instance, ITuple arguments)
		=> this.Parameters.Zip(arguments.GetType().GetGenericArguments()).All(_ => _.First.ParameterType == _.Second)
			? this._InvokeWithTuple.Value(instance, arguments)
			: this._InvokeWithArray.Value(instance, arguments.ToArray());

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public MethodInfo ToMethodInfo()
		=> (MethodInfo)this.Handle.ToMethodBase(this._TypeHandle);

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	private Delegate CreateCall()
	{
		ParameterExpression instance = nameof(instance).ToParameterExpression(this.Type);
		var parameters = this.Parameters.Select(_ => _.ToExpression()).ToArray();
		var call = instance.Call(this.ToMethodInfo(), parameters);
		var lambda = call.Lambda([instance, .. parameters]);

		return lambda.Compile();
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	private Func<object, object?[]?, object?> CreateArrayCall()
	{
		ParameterExpression arguments = nameof(arguments).ToParameterExpression<object?[]?>();
		Expression[]? parameters = null;
		if (this.Parameters.Any())
			parameters = this.Parameters.Select((parameter, index) => arguments.Array().Index(index).Convert(parameter.ParameterType)).ToArray();

		ParameterExpression instance = nameof(instance).ToParameterExpression<object>();
		Expression typedInstance = this.Type != typeof(object) ? instance.Cast(this.Type) : instance;
		Expression methodExpression = this.ToMethodInfo().ToExpression(typedInstance, parameters);

		if (this.HasReturnValue)
		{
			if (this.Return.ParameterType != typeof(object))
				methodExpression = methodExpression.As<object>();

			return methodExpression.Lambda<Func<object, object?[]?, object?>>([instance, arguments]).Compile();
		}

		var action = methodExpression.Lambda<Action<object, object?[]?>>([instance, arguments]).Compile();
		return (instance, arguments) =>
		{
			action(instance, arguments);
			return null;
		};
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	private Func<object, ITuple, object?> CreateTupleCall()
	{
		ParameterExpression arguments = nameof(arguments).ToParameterExpression<ITuple>();
		Expression[]? parameters = null;
		if (this.Parameters.Any())
		{
			var valueTupleType = GetValueTupleType(this.Parameters);
			parameters = GetValueTupleFields(arguments.Cast(valueTupleType), this.Parameters.Count).ToArray();
		}

		ParameterExpression instance = nameof(instance).ToParameterExpression<object>();
		Expression typedInstance = this.Type != typeof(object) ? instance.Cast(this.Type) : instance;
		Expression methodExpression = this.ToMethodInfo().ToExpression(typedInstance, parameters);

		if (this.HasReturnValue)
		{
			if (this.Return.ParameterType != typeof(object))
				methodExpression = methodExpression.Cast<object>();

			return methodExpression.Lambda<Func<object, ITuple, object?>>([instance, arguments]).Compile();
		}

		var action = methodExpression.Lambda<Action<object, ITuple>>([instance, arguments]).Compile();
		return (instance, arguments) =>
		{
			action(instance, arguments);
			return null;
		};
	}
}
