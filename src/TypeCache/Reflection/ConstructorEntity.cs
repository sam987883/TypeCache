// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;

namespace TypeCache.Reflection;

public sealed class ConstructorEntity : Method
{
	private readonly Lazy<Delegate> _Create;
	private readonly Lazy<Func<object?[]?, object>> _CreateWithArray;
	private readonly Lazy<Func<ITuple, object>> _CreateWithTuple;

	public ConstructorEntity(ConstructorInfo constructorInfo)
		: base(constructorInfo)
	{
		this._Create = new(() => this.ToConstructorInfo().ToExpression().Lambda().Compile());
		this._CreateWithArray = new(this.CreateArrayCall);
		this._CreateWithTuple = new(this.CreateTupleCall);
	}

	public Delegate Delegate => this._Create.Value;

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public object Create()
		=> this._CreateWithArray.Value(null);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public object Create(object?[] arguments)
		=> this._CreateWithArray.Value(arguments);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public object Create(ITuple arguments)
		=> this._CreateWithTuple.Value(arguments);

	public bool Equals(ConstructorEntity? other)
		=> this.Handle == other?.Handle;

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public ConstructorInfo ToConstructorInfo()
		=> (ConstructorInfo)this.Handle.ToMethodBase(this._TypeHandle);

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	private Delegate CreateCall()
	{
		var parameters = this.Parameters.Select(_ => _.ToExpression());

		return this.ToConstructorInfo().ToExpression(parameters).Lambda(parameters).Compile();
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	private Func<object?[]?, object> CreateArrayCall()
	{
		ParameterExpression arguments = nameof(arguments).ToParameterExpression<object?[]?>();

		if (this.Parameters.Count > 0)
		{
			var parameters = this.Parameters
				.Index()
				.Select(_ => arguments.Array().Index(_.Index).Convert(_.Item.ParameterType));
			return this.ToConstructorInfo()
				.ToExpression(parameters)
				.Cast<object>()
				.Lambda<Func<object?[]?, object>>([arguments])
				.Compile();
		}

		return this.ToConstructorInfo()
			.ToExpression(null)
			.Cast<object>()
			.Lambda<Func<object?[]?, object>>([arguments])
			.Compile();
	}

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	private Func<ITuple, object> CreateTupleCall()
	{
		ParameterExpression argument = nameof(argument).ToParameterExpression<ITuple>();

		if (this.Parameters.Count > 0)
		{
			var valueTupleType = MethodEntity.GetValueTupleType(this.Parameters);
			var valueTupleFields = MethodEntity.GetValueTupleFields(argument.Cast(valueTupleType), this.Parameters.Count);
			return this.ToConstructorInfo()
				.ToExpression(valueTupleFields)
				.Cast<object>()
				.Lambda<Func<ITuple, object>>([argument])
				.Compile();
		}

		return this.ToConstructorInfo()
			.ToExpression(null)
			.Cast<object>()
			.Lambda<Func<ITuple, object>>([argument])
			.Compile();
	}
}
