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
	protected readonly RuntimeTypeHandle _TypeHandle;

	public Method(MethodBase methodBase)
	{
		methodBase.ThrowIfNull();
		methodBase.DeclaringType.ThrowIfNull();

		this._GenericTypeHandles = methodBase.IsGenericMethod ? methodBase.GetGenericArguments().Select(_ => _.TypeHandle).ToArray() : [];
		this._TypeHandle = methodBase.DeclaringType.TypeHandle;

		this.Attributes = new ReadOnlyCollection<Attribute>(methodBase.GetCustomAttributes());
		this.CodeName = GetCodeName(methodBase);
		this.Handle = methodBase.MethodHandle;
		this.IsGeneric = methodBase.IsGenericMethod;
		this.IsPublic = methodBase.IsPublic;
		this.IsStatic = methodBase.IsStatic;
		this.Name = methodBase.Name;
		this.Parameters = methodBase.GetParameters()
			.Where(_ => !_.IsRetval)
			.OrderBy(_ => _.Position)
			.Select(_ => new ParameterEntity(_))
			.ToArray();
	}

	/// <summary>
	/// Method attributes.
	/// </summary>
	public IReadOnlyCollection<Attribute> Attributes { get; }

	/// <summary>
	/// The C# name of the method.  For example:
	/// <list type="bullet">
	/// <item><c>GetItems</c></item>
	/// <item><c>GetItems&lt;&gt;</c></item>
	/// <item><c>GetItems&lt;String&gt;</c></item>
	/// </list>
	/// </summary>
	public string CodeName { get; }

	/// <summary>
	/// Method generic parameter types.
	/// </summary>
	public Type[] GenericTypes => this._GenericTypeHandles.Select(_ => _.ToType()).ToArray();

	/// <summary>
	/// Method handle.
	/// </summary>
	public RuntimeMethodHandle Handle { get; }

	/// <inheritdoc cref="MethodBase.IsGenericMethod"/>
	/// <remarks>
	/// Returns <b><c><see langword="true"/></c></b> if a method is either a <b><i>generic method definition</i></b> or a <b><i>constructed generic method</i></b>.
	/// </remarks>
	public bool IsGeneric { get; }

	/// <inheritdoc cref="MethodBase.IsPublic"/>
	public bool IsPublic { get; }

	/// <inheritdoc cref="MethodBase.IsStatic"/>
	public bool IsStatic { get; }

	/// <summary>
	/// The method name.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// The ordered list of method parameters.
	/// </summary>
	public IReadOnlyList<ParameterEntity> Parameters { get; }

	/// <inheritdoc cref="MemberInfo.DeclaringType"/>
	public Type Type => this._TypeHandle.ToType();

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public bool Equals(Method? other)
		=> this.Handle == other?.Handle;

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public override bool Equals(object? obj)
		=> obj is Method method && this.Handle == method.Handle;

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public override int GetHashCode()
		=> this.Handle.GetHashCode();

	/// <summary>
	/// The C# name of the method.  For example:
	/// <list type="bullet">
	/// <item><c>GetItems</c></item>
	/// <item><c>GetItems&lt;&gt;</c></item>
	/// <item><c>GetItems&lt;String&gt;</c></item>
	/// </list>
	/// </summary>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public override string ToString()
		=> this.CodeName;

	public bool HasGenericTypes(Type[] genericTypes)
		=> this.IsGeneric && this._GenericTypeHandles.SequenceEqual(genericTypes.Select(_ => _.TypeHandle));

	public bool IsCallableWith(object?[] arguments)
		=> arguments switch
		{
			null or [] => this.IsCallableWithNoArguments(),
			_ when arguments.Length > this.Parameters.Count => false,
			_ => this.Parameters
				.Select((parameter, index) => (Item: parameter, Index: index))
				.All(_ => _ switch
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
			_ => this.Parameters
				.Select((parameter, index) => (Item: parameter, Index: index))
				.All(_ => _ switch
				{
					_ when _.Item.IsOut => false,
					_ when _.Index >= argumentTypes.Length => _.Item.HasDefaultValue || _.Item.IsOptional,
					_ => _.Item.Supports(argumentTypes[_.Index])
				})
		};

	public bool IsCallableWithNoArguments()
		=> this.Parameters.Count is 0 || this.Parameters.All(_ => _.HasDefaultValue || _.IsOptional);

	protected static string GetCodeName(MethodBase methodBase)
		=> methodBase switch
		{
			ConstructorInfo => methodBase.DeclaringType?.Name ?? methodBase.Name,
			{ IsGenericMethodDefinition: true } => Invariant($"{methodBase.Name}<{string.Concat(','.Repeat(methodBase.GetGenericArguments().Length - 1))}>"),
			{ IsGenericMethod: true } => Invariant($"{methodBase.Name}<{methodBase.GetGenericArguments().Select(_ => _.CodeName()).ToCSV()}>"),
			_ => methodBase.Name
		};

	protected static IEnumerable<Expression> GetValueTupleFields(Expression tupleExpression, int count)
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

	protected static Type GetValueTupleType(IReadOnlyList<ParameterEntity> parameters)
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
