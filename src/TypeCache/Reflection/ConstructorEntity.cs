// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections;
using TypeCache.Extensions;

namespace TypeCache.Reflection;

public sealed class ConstructorEntity
{
	private readonly Lazy<Delegate> _Create;
	private readonly Lazy<Func<object?[]?, object>> _CreateWithArray;
	private readonly Lazy<Func<ITuple, object>> _CreateWithTuple;
	private readonly RuntimeTypeHandle _TypeHandle;

	/// <summary>
	/// Intended for struct types only as they always have a default constructor that does not show up in Reflection.
	/// </summary>
	public ConstructorEntity(Type type)
	{
		this.Attributes = new ReadOnlyCollection<Attribute>(Enumerable<Attribute>.Empty);
		this._Create = new(this.CreateCall);
		this._CreateWithArray = new(this.CreateArrayCall);
		this._CreateWithTuple = new(this.CreateTupleCall);

		this.Parameters = [];
	}

	public ConstructorEntity(ConstructorInfo constructorInfo)
	{
		constructorInfo.DeclaringType.ThrowIfNull();

		this._TypeHandle = constructorInfo.DeclaringType.TypeHandle;
		this._Create = new(() => this.ToConstructorInfo().ToExpression().Lambda().Compile());
		this._CreateWithArray = new(this.CreateArrayCall);
		this._CreateWithTuple = new(this.CreateTupleCall);

		this.Attributes = new ReadOnlyCollection<Attribute>(constructorInfo.GetCustomAttributes());
		this.Handle = constructorInfo.MethodHandle;
		this.IsPublic = constructorInfo.IsPublic;
		this.Parameters = constructorInfo.GetParameters()
			.Where(_ => !_.IsRetval)
			.OrderBy(_ => _.Position)
			.Select(_ => new ParameterEntity(_))
			.ToArray();
	}

	public IReadOnlyCollection<Attribute> Attributes { get; }

	public Delegate Delegate => this._Create.Value;

	public RuntimeMethodHandle Handle { get; }

	public bool IsPublic { get; }

	public IReadOnlyList<ParameterEntity> Parameters { get; }

	public Type Type => this._TypeHandle.ToType();

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public object Create()
		=> this._CreateWithArray.Value(null);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public object Create(object?[] arguments)
		=> this._CreateWithArray.Value(arguments);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public object Create(ITuple arguments)
		=> this._CreateWithTuple.Value(arguments);

	public bool IsCallableWithNoArguments()
		=> this.Parameters.Count is 0 || this.Parameters.All(_ => _.HasDefaultValue || _.IsOptional);

	public bool IsCallableWith(object?[] arguments)
	{
		arguments ??= [];
		if (arguments.Length is 0)
			return this.Parameters.Count is 0 || this.Parameters.All(_ => _.HasDefaultValue || _.IsOptional);

		if (arguments.Length > this.Parameters.Count)
			return false;

		return this.Parameters
			.Index()
			.All(_ => _ switch
			{
				_ when _.Index >= arguments.Length => _.Item.HasDefaultValue || _.Item.IsOptional,
				_ when arguments[_.Index] == Type.Missing => _.Item.HasDefaultValue || _.Item.IsOptional,
				_ when arguments[_.Index] is not null => arguments[_.Index]!.GetType().IsAssignableTo(_.Item.ParameterType),
				_ => _.Item.IsNullable
			});
	}

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public bool IsCallableWith(ITuple arguments)
		=> this.IsCallableWith(arguments.ToArray());

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public ConstructorInfo ToConstructorInfo()
		=> (ConstructorInfo)this.Handle.ToMethodBase(this._TypeHandle);

	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	public Delegate CreateCall()
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
