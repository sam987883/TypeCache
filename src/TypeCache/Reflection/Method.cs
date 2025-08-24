// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections;
using TypeCache.Extensions;

namespace TypeCache.Reflection;

public abstract class Method
{
	private readonly IReadOnlyList<RuntimeTypeHandle> _GenericTypeHandles;
	private readonly RuntimeTypeHandle _TypeHandle;

	public Method(MethodInfo methodInfo)
	{
		methodInfo.ThrowIfNull();
		methodInfo.DeclaringType.ThrowIfNull();

		this._GenericTypeHandles = methodInfo.GetGenericArguments().Select(_ => _.TypeHandle).ToArray();
		this._TypeHandle = methodInfo.DeclaringType.TypeHandle;

		this.Attributes = new ReadOnlyCollection<Attribute>(methodInfo.GetCustomAttributes());
		this.CodeName = GetCodeName(methodInfo);
		this.Handle = methodInfo.MethodHandle;
		this.HasReturnValue = methodInfo.ReturnType != typeof(void);
		this.IsPublic = methodInfo.IsPublic;
		this.Name = methodInfo.Name;
		this.Parameters = methodInfo.GetParameters()
			.Where(_ => !_.IsRetval)
			.OrderBy(_ => _.Position)
			.Select(_ => new ParameterEntity(_))
			.ToArray();
		this.Return = new(methodInfo.ReturnParameter);
	}

	public IReadOnlyCollection<Attribute> Attributes { get; }

	public string CodeName { get; }

	public Type[] GenericTypes => this._GenericTypeHandles.Select(_ => _.ToType()).ToArray();

	public RuntimeMethodHandle Handle { get; }

	public bool HasReturnValue { get; }

	public bool IsPublic { get; }

	public string Name { get; }

	public IReadOnlyList<ParameterEntity> Parameters { get; }

	public ParameterEntity Return { get; }

	public Type Type => this._TypeHandle.ToType();

	public bool IsCallableWith(object?[] arguments)
	{
		if (this.Parameters.Any(_ => _.IsOut))
			return false;

		arguments ??= [];
		if (arguments.Length is 0)
			return this.Parameters.Count is 0 || this.Parameters.All(_ => _.HasDefaultValue || _.IsOptional);

		if (arguments.Length > this.Parameters.Count)
			return false;

		return this.Parameters
			.Select((parameter, index) => (Item: parameter, Index: index))
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

	public bool IsCallableWith(Type[] argumentTypes)
	{
		if (this.Parameters.Any(_ => _.IsOut))
			return false;

		argumentTypes ??= [];
		if (argumentTypes.Length is 0)
			return this.Parameters.Count is 0 || this.Parameters.All(_ => _.HasDefaultValue || _.IsOptional);

		if (argumentTypes.Length > this.Parameters.Count)
			return false;

		return this.Parameters
			.Select((parameter, index) => (Item: parameter, Index: index))
			.All(_ => _.Index >= argumentTypes.Length
				? _.Item.HasDefaultValue || _.Item.IsOptional
				: argumentTypes[_.Index]!.IsAssignableTo(_.Item.ParameterType));
	}

	public bool IsCallableWithNoArguments()
		=> this.Parameters.Count is 0 || this.Parameters.All(_ => _.HasDefaultValue || _.IsOptional);

	public bool Match(RuntimeTypeHandle[] genericTypeHandles)
		=> this.GenericTypes.Zip(genericTypeHandles.Select(_ => _.ToType())).All(_ => _.Second.IsAssignableTo(_.First));

	public bool Match(Type[] genericTypes)
		=> this.GenericTypes.Zip(genericTypes).All(_ => _.Second.IsAssignableTo(_.First));

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public MethodInfo ToMethodInfo()
		=> (MethodInfo)this.Handle.ToMethodBase(this._TypeHandle);

	private static string GetCodeName(MethodInfo methodInfo)
		=> methodInfo switch
		{
			{ IsGenericMethodDefinition: true } => Invariant($"{methodInfo.Name[0..methodInfo.Name.IndexOf(TypeStore.GENERIC_TICKMARK)]}<{','.Repeat(methodInfo.GetGenericArguments().Length - 1)}>"),
			{ IsGenericMethod: true } => Invariant($"{methodInfo.Name[0..methodInfo.Name.IndexOf(TypeStore.GENERIC_TICKMARK)]}<{string.Join(", ", methodInfo.GetGenericArguments().Select(_ => _.CodeName()))}>"),
			_ => methodInfo.Name
		};

	internal static IEnumerable<Expression> GetValueTupleFields(Expression tupleExpression, int count)
	{
		const string Item = nameof(Item);
		const string Rest = nameof(Rest);

		var n = 0;
		for (var i = 0; i < count; ++i)
		{
			if (++n is 8)
			{
				tupleExpression = tupleExpression.Field(Rest);
				n = 1;
			}

			yield return tupleExpression.Field(Invariant($"{Item}{n}"));
		}
	}

	internal static Type GetValueTupleType(IReadOnlyList<ParameterEntity> parameters)
	{
		var parameterTypeChunks = parameters
			.Select(_ => _.ParameterType)
			.Chunk(7)
			.ToArray();
		var valueTupleTypeStack = new Stack<Type>(parameterTypeChunks.Select((types, i) => getValueTupleGenericType(i < types.Length - 1 ? 8 : types.Length)));
		var parameterTypeStack = new Stack<Type[]>(parameterTypeChunks);
		var valueTupleType = valueTupleTypeStack.Pop().MakeGenericType(parameterTypeStack.Pop());
		while (valueTupleTypeStack.Count > 0)
		{
			valueTupleType = valueTupleTypeStack.Pop().MakeGenericType([.. parameterTypeStack.Pop(), valueTupleType]);
		}

		return valueTupleType;

		static Type getValueTupleGenericType(int arity)
			=> arity switch
			{
				1 => typeof(ValueTuple<>),
				2 => typeof(ValueTuple<,>),
				3 => typeof(ValueTuple<,,>),
				4 => typeof(ValueTuple<,,,>),
				5 => typeof(ValueTuple<,,,,>),
				6 => typeof(ValueTuple<,,,,,>),
				7 => typeof(ValueTuple<,,,,,,>),
				_ => typeof(ValueTuple<,,,,,,,>),
			};
	}
}
