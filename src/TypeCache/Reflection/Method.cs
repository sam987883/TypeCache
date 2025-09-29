// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections;
using TypeCache.Extensions;

namespace TypeCache.Reflection;

public class Method : IEquatable<Method>
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
		this.IsConstructedGenericMethod = methodInfo.IsConstructedGenericMethod;
		this.IsGenericMethod = methodInfo.IsGenericMethod;
		this.IsGenericMethodDefinition = methodInfo.IsGenericMethodDefinition;
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

	public bool IsConstructedGenericMethod { get; }

	public bool IsGenericMethod { get; }

	public bool IsGenericMethodDefinition { get; }

	public bool IsPublic { get; }

	public bool IsStatic { get; }

	public string Name { get; }

	public IReadOnlyList<ParameterEntity> Parameters { get; }

	public ParameterEntity Return { get; }

	public Type Type => this._TypeHandle.ToType();

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public bool Equals(Method? other)
		=> this.Handle == other?.Handle;

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public override bool Equals(object? obj)
		=> obj is MethodEntity method && this.Handle == method.Handle;

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public override int GetHashCode()
		=> this.Handle.GetHashCode();

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public override string ToString()
		=> this.CodeName;

	/// <summary>
	/// Creates a constructed generic method by providing the type arguments to a generic method definition.<br/>
	/// <b><c><see langword="null"/></c></b> is returned if this method is not a generic method definition.<br/>
	/// An exception is thrown if the generic method definition does not support the <b><c><paramref name="genericTypeArguments"/></c></b>.<br/>
	/// </summary>
	/// <param name="genericTypeArguments">Method generic type arguments</param>
	/// <exception cref="ArgumentException"/>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="InvalidOperationException"/>
	/// <exception cref="NotSupportedException"/>
	public Method? ConstructGenericMethod(Type[] genericTypeArguments)
		=> (this.IsGenericMethodDefinition, this.IsStatic) switch
		{
			(false, _) => null,
			(true, false) => new MethodEntity(this.ToMethodInfo().MakeGenericMethod(genericTypeArguments)),
			_ => new StaticMethodEntity(this.ToMethodInfo().MakeGenericMethod(genericTypeArguments))
		};

	public bool HasGenericTypes(Type[] genericTypes)
		=> this.IsGenericMethod && this._GenericTypeHandles.SequenceEqual(genericTypes.Select(_ => _.TypeHandle));

	public bool IsCallableWith(object?[] arguments)
		=> arguments switch
		{
			null or [] => this.IsCallableWithNoArguments(),
			_ when arguments.Length > this.Parameters.Count => false,
			_ => this.Parameters.Index().All(_ => _ switch
			{
				_ when _.Item.IsOut => false,
				_ when _.Index >= arguments.Length => _.Item.HasDefaultValue || _.Item.IsOptional,
				_ => _.Item.SupportsValue(arguments[_.Index])
			})
		};

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public bool IsCallableWith(ITuple arguments)
		=> this.IsCallableWith(arguments.ToArray());

	public bool IsCallableWith(Type[] argumentTypes)
		=> argumentTypes switch
		{
			null or [] => this.IsCallableWithNoArguments(),
			_ when argumentTypes.Length > this.Parameters.Count => false,
			_ => this.Parameters.Index().All(_ => _ switch
			{
				_ when _.Item.IsOut => false,
				_ when _.Index >= argumentTypes.Length => _.Item.HasDefaultValue || _.Item.IsOptional,
				_ => _.Item.Supports(argumentTypes[_.Index])
			})
		};

	public bool IsCallableWithNoArguments()
		=> this.Parameters.Count is 0 || this.Parameters.All(_ => _.HasDefaultValue || _.IsOptional);

	public bool Supports(Type[] genericTypes)
		=> this.IsGenericMethodDefinition
			? this.GenericTypes.Zip(genericTypes).All(_ => _.Second.IsAssignableTo(_.First))
			: this.HasGenericTypes(genericTypes);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public MethodInfo ToMethodInfo()
		=> (MethodInfo)this.Handle.ToMethodBase(this._TypeHandle);

	private static string GetCodeName(MethodInfo methodInfo)
		=> methodInfo switch
		{
			{ IsGenericMethodDefinition: true } => Invariant($"{methodInfo.Name}<{string.Concat(','.Repeat(methodInfo.GetGenericArguments().Length - 1))}>"),
			{ IsGenericMethod: true } => Invariant($"{methodInfo.Name}<{methodInfo.GetGenericArguments().Select(_ => _.CodeName()).ToCSV()}>"),
			_ => methodInfo.Name
		};

	protected internal static IEnumerable<Expression> GetValueTupleFields(Expression tupleExpression, int count)
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

	protected internal static Type GetValueTupleType(IReadOnlyList<ParameterEntity> parameters)
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
