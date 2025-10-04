// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using System.Reflection;
using TypeCache.Reflection;
using TypeCache.Utilities;

namespace TypeCache.Extensions;

public static partial class ExpressionExtensions
{
	/// <inheritdoc cref="Expression.AndAlso(Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.AndAlso(@<paramref name="this"/>, <paramref name="operand"/>);</c><br/><br/>
	/// <c>a &amp;&amp; b</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static BinaryExpression And(this Expression @this, Expression operand)
		=> Expression.AndAlso(@this, operand);

	/// <remarks>
	/// <c>=&gt; <see langword="new"/>(@<paramref name="this"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ArrayExpressionFactory Array(this Expression @this)
		=> new(@this);

	/// <inheritdoc cref="Expression.TypeAs(Expression, Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.TypeAs(@<paramref name="this"/>, <see langword="typeof"/>(<typeparamref name="T"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Expression As<T>(this Expression @this)
		where T : class
		=> Expression.TypeAs(@this, typeof(T));

	/// <inheritdoc cref="Expression.TypeAs(Expression, Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.TypeAs(@<paramref name="this"/>, <paramref name="type"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Expression As(this Expression @this, Type type)
		=> Expression.TypeAs(@this, type);

	/// <inheritdoc cref="Expression.Assign(Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Assign(@<paramref name="this"/>, <paramref name="expression"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static BinaryExpression Assign(this Expression @this, Expression expression)
		=> Expression.Assign(@this, expression);

	/// <inheritdoc cref="Expression.Block(Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Block(@<paramref name="this"/>, <paramref name="expression"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static BlockExpression Block(this Expression @this, Expression expression)
		=> Expression.Block(@this, expression);

	/// <inheritdoc cref="Expression.Call(Expression, MethodInfo)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="methodInfo"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodCallExpression Call(this Expression @this, MethodInfo methodInfo)
		=> Expression.Call(@this, methodInfo);

	/// <inheritdoc cref="Expression.Call(Expression, MethodInfo, IEnumerable{Expression})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="methodInfo"/>, <paramref name="arguments"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodCallExpression Call(this Expression @this, MethodInfo methodInfo, IEnumerable<Expression>? arguments = null)
		=> Expression.Call(@this, methodInfo, arguments);

	/// <inheritdoc cref="Expression.Call(Expression, MethodInfo, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="methodInfo"/>, <paramref name="arguments"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodCallExpression Call(this Expression @this, MethodInfo methodInfo, Expression[]? arguments = null)
		=> Expression.Call(@this, methodInfo, arguments);

	/// <inheritdoc cref="Expression.Call(Expression, string, Type[], Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="method"/>, <see cref="Type.EmptyTypes"/>, <paramref name="arguments"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodCallExpression Call(this Expression @this, string method, Expression[]? arguments = null)
		=> Expression.Call(@this, method, Type.EmptyTypes, arguments);

	/// <inheritdoc cref="Expression.Call(Expression, string, Type[], Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="method"/>, <paramref name="genericTypes"/>, <paramref name="arguments"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodCallExpression Call(this Expression @this, string method, Type[]? genericTypes, Expression[]? arguments = null)
		=> Expression.Call(@this, method, genericTypes, arguments);

	/// <inheritdoc cref="Expression.Call(Expression?, MethodInfo, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="arguments"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodCallExpression CallStatic(this MethodInfo @this, Expression[]? arguments = null)
		=> Expression.Call(@this, arguments);

	/// <inheritdoc cref="Expression.Call(Expression, string, Type[], Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="arguments"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodCallExpression CallStatic(this MethodInfo @this, IEnumerable<Expression> arguments)
		=> Expression.Call(@this, arguments);

	/// <inheritdoc cref="Expression.Call(Expression, string, Type[], Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="method"/>, <see cref="Type.EmptyTypes"/>, <paramref name="arguments"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodCallExpression CallStatic(this Type @this, string method, Expression[]? arguments = null)
		=> Expression.Call(@this, method, Type.EmptyTypes, arguments);

	/// <inheritdoc cref="Expression.Call(Expression, string, Type[], Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Call(@<paramref name="this"/>, <paramref name="method"/>, <paramref name="genericTypes"/>, <paramref name="arguments"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MethodCallExpression CallStatic(this Type @this, string method, Type[]? genericTypes, Expression[]? arguments = null)
		=> Expression.Call(@this, method, genericTypes, arguments);

	/// <inheritdoc cref="Expression.Convert(Expression, Type)"/>
	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Convert(<see langword="typeof"/>(<typeparamref name="T"/>), <paramref name="overflowCheck"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Expression Cast<T>(this Expression @this, bool overflowCheck = false)
		=> @this.Cast(typeof(T), overflowCheck);

	/// <remarks>
	/// <code>
	/// =&gt; (@<paramref name="this"/>.Type.IsValueType, <paramref name="type"/>.IsValueType) <see langword="switch"/><br/>
	/// {<br/>
	/// <see langword="    "/>(<see langword="true"/>, <see langword="false"/>) =&gt; @<paramref name="this"/>.Unbox(<paramref name="type"/>),<br/>
	/// <see langword="    "/>_ <see langword="when"/> <paramref name="overflowCheck"/> =&gt; <see cref="Expression"/>.ConvertChecked(@<paramref name="this"/>, <paramref name="type"/>),<br/>
	/// <see langword="    "/>_ =&gt; <see cref="Expression"/>.Convert(@<paramref name="this"/>, <paramref name="type"/>),<br/>
	/// };
	/// </code>
	/// </remarks>
	public static UnaryExpression Cast(this Expression @this, Type type, bool overflowCheck = false) => (@this.Type.IsValueType, type.IsValueType) switch
	{
		(false, true) => @this.Unbox(type),
		_ when overflowCheck => Expression.ConvertChecked(@this, type),
		_ => Expression.Convert(@this, type)
	};

	/// <inheritdoc cref="Expression.Coalesce(Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Coalesce(@<paramref name="this"/>, <paramref name="expression"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static BinaryExpression Coalesce(this Expression @this, Expression expression)
		=> Expression.Coalesce(@this, expression);

	/// <remarks>
	/// <c>=&gt; @<paramref name="this"/>.Convert(<see langword="typeof"/>(<typeparamref name="T"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Expression Convert<T>(this Expression @this)
		=> @this.Convert(typeof(T));

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="InvalidOperationException"/>
	public static Expression Convert(this Expression @this, Type targetType)
	{
		@this.ThrowIfNull();
		targetType.ThrowIfNull();

		if (@this.Type == targetType)
			return @this;

		if (@this.Type.IsAssignableTo(targetType))
			return @this.Cast(targetType);

		if (@this.Type != typeof(object))
			return @this.CreateConversionExpression(targetType);

		var targetScalarType = targetType.ScalarType();
		if (targetScalarType is ScalarType.None)
			return @this.Cast(targetType);

		var targetObjectType = targetType.ObjectType();
		if (targetType.IsValueType && targetObjectType is ObjectType.Nullable)
			targetType = targetType.GenericTypeArguments[0];

		var expression = targetScalarType switch
		{
			ScalarType.Enum => typeof(ValueConverter).CallStatic(nameof(ValueConverter.ConvertToEnum), [@this]),
			ScalarType.Index => typeof(ValueConverter).CallStatic(nameof(ValueConverter.ConvertToIndex), [@this]),
			ScalarType.Boolean => typeof(ValueConverter).CallStatic(nameof(ValueConverter.ConvertToBoolean), [@this]),
			ScalarType.DateOnly => typeof(ValueConverter).CallStatic(nameof(ValueConverter.ConvertToDateOnly), [@this]),
			ScalarType.DateTime => typeof(ValueConverter).CallStatic(nameof(ValueConverter.ConvertToDateTime), [@this]),
			ScalarType.DateTimeOffset => typeof(ValueConverter).CallStatic(nameof(ValueConverter.ConvertToDateTimeOffset), [@this]),
			ScalarType.Guid => typeof(ValueConverter).CallStatic(nameof(ValueConverter.ConvertToGuid), [@this]),
			ScalarType.String => typeof(ValueConverter).CallStatic(nameof(ValueConverter.ConvertToString), [@this]),
			ScalarType.TimeOnly => typeof(ValueConverter).CallStatic(nameof(ValueConverter.ConvertToTimeOnly), [@this]),
			ScalarType.TimeSpan => typeof(ValueConverter).CallStatic(nameof(ValueConverter.ConvertToTimeSpan), [@this]),
			ScalarType.Uri => typeof(ValueConverter).CallStatic(nameof(ValueConverter.ConvertToUri), [@this]),
			_ => typeof(ValueConverter).CallStatic(nameof(ValueConverter.ConvertToNumber), [targetType], [@this])
		};

		if (targetType.IsValueType && targetObjectType is not ObjectType.Nullable)
			return expression.Property(nameof(Nullable<int>.Value));

		return expression;
	}

	/// <inheritdoc cref="Expression.Field(Expression, FieldInfo)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Field(@<paramref name="this"/>, <paramref name="fieldInfo"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MemberExpression Field(this Expression @this, FieldInfo fieldInfo)
		=> Expression.Field(@this, fieldInfo);

	/// <inheritdoc cref="Expression.Field(Expression, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Field(@<paramref name="this"/>, <paramref name="name"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MemberExpression Field(this Expression @this, string name)
		=> Expression.Field(@this, name);

	/// <inheritdoc cref="Expression.IfThen(Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.IfThen(@<paramref name="this"/>, <paramref name="trueResult"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ConditionalExpression If(this Expression @this, Expression trueResult)
		=> Expression.IfThen(@this, trueResult);

	/// <inheritdoc cref="Expression.IfThenElse(Expression, Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.IfThenElse(@<paramref name="this"/>, <paramref name="trueResult"/>, <paramref name="falseResult"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ConditionalExpression If(this Expression @this, Expression trueResult, Expression falseResult)
		=> Expression.IfThenElse(@this, trueResult, falseResult);

	/// <inheritdoc cref="Expression.Condition(Expression, Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Condition(@<paramref name="this"/>, <paramref name="trueResult"/>, <paramref name="falseResult"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static ConditionalExpression IIf(this Expression @this, Expression trueResult, Expression falseResult)
		=> Expression.Condition(@this, trueResult, falseResult);

	/// <inheritdoc cref="Expression.Invoke(Expression, IEnumerable{Expression})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Invoke(@<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static InvocationExpression Invoke(this LambdaExpression @this, IEnumerable<Expression> parameters)
		=> Expression.Invoke(@this, parameters);

	/// <inheritdoc cref="Expression.Invoke(Expression, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Invoke(@<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static InvocationExpression Invoke(this LambdaExpression @this, Expression[]? parameters)
		=> Expression.Invoke(@this, parameters);

	/// <inheritdoc cref="Expression.TypeIs(Expression, Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.TypeIs(@<paramref name="this"/>, <see langword="typeof"/>(<typeparamref name="T"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static TypeBinaryExpression Is<T>(this Expression @this)
		=> Expression.TypeIs(@this, typeof(T));

	/// <inheritdoc cref="Expression.TypeIs(Expression, Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.TypeIs(@<paramref name="this"/>, <paramref name="type"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static TypeBinaryExpression Is(this Expression @this, Type type)
		=> Expression.TypeIs(@this, type);

	/// <inheritdoc cref="Expression.ReferenceNotEqual(Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ReferenceNotEqual(@<paramref name="this"/>, <see cref="Expression"/>.Constant(<see langword="null"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static BinaryExpression IsNotNull(this Expression @this)
		=> Expression.ReferenceNotEqual(@this, Expression.Constant(null));

	/// <inheritdoc cref="Expression.ReferenceEqual(Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.ReferenceEqual(@<paramref name="this"/>, <see cref="Expression"/>.Constant(<see langword="null"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static BinaryExpression IsNull(this Expression @this)
		=> Expression.ReferenceEqual(@this, Expression.Constant(null));

	/// <inheritdoc cref="Expression.Lambda(Expression, IEnumerable{ParameterExpression})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Lambda(@<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static LambdaExpression Lambda(this Expression @this, IEnumerable<ParameterExpression> parameters)
		=> Expression.Lambda(@this, parameters);

	/// <inheritdoc cref="Expression.Lambda{TDelegate}(Expression, IEnumerable{ParameterExpression})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Lambda&lt;<typeparamref name="T"/>&gt;(@<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Expression<T> Lambda<T>(this Expression @this, IEnumerable<ParameterExpression> parameters)
		=> Expression.Lambda<T>(@this, parameters);

	/// <inheritdoc cref="Expression.Lambda(Expression, ParameterExpression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Lambda(@<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static LambdaExpression Lambda(this Expression @this, ParameterExpression[]? parameters = null)
		=> Expression.Lambda(@this, parameters);

	/// <inheritdoc cref="Expression.Lambda{TDelegate}(Expression, ParameterExpression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Lambda&lt;<typeparamref name="T"/>&gt;(@<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static Expression<T> Lambda<T>(this Expression @this, ParameterExpression[]? parameters = null)
		=> Expression.Lambda<T>(@this, parameters);

	/// <inheritdoc cref="Expression.Lambda(Type, Expression, IEnumerable{ParameterExpression})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Lambda(<see cref="Expression"/>.GetActionType(<paramref name="parameters"/>.Select(parameter => parameter.Type).ToArray(),
	/// @<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	public static LambdaExpression LambdaAction(this Expression @this, IEnumerable<ParameterExpression> parameters)
		=> Expression.Lambda(Expression.GetActionType(parameters.Select(parameter => parameter.Type).ToArray()), @this, parameters);

	/// <inheritdoc cref="Expression.Lambda(Type, Expression, ParameterExpression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Lambda(<see cref="Expression"/>.GetActionType(<paramref name="parameters"/>.Select(parameter => parameter.Type).ToArray(),
	/// @<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	public static LambdaExpression LambdaAction(this Expression @this, ParameterExpression[]? parameters = null)
		=> Expression.Lambda(Expression.GetActionType(parameters?.Select(parameter => parameter.Type).ToArray()), @this, parameters);

	/// <inheritdoc cref="Expression.Lambda(Type, Expression, IEnumerable{ParameterExpression})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Lambda(<see cref="Expression"/>.GetFuncType([..<paramref name="parameters"/>.Select(parameter => parameter.Type), <see langword="typeof"/>(<typeparamref name="T"/>)]),
	/// @<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	public static LambdaExpression LambdaFunc<T>(this Expression @this, IEnumerable<ParameterExpression> parameters)
		=> Expression.Lambda(Expression.GetFuncType([..parameters.Select(parameter => parameter.Type), typeof(T)]), @this, parameters);

	/// <inheritdoc cref="Expression.Lambda(Type, Expression, ParameterExpression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Lambda(<see cref="Expression"/>.GetFuncType([..<paramref name="parameters"/>.Select(parameter => parameter.Type), <see langword="typeof"/>(<typeparamref name="T"/>)]),
	/// @<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	public static LambdaExpression LambdaFunc<T>(this Expression @this, ParameterExpression[]? parameters = null)
		=> Expression.Lambda(Expression.GetFuncType([..parameters?.Select(parameter => parameter.Type) ?? [], typeof(T)]), @this, parameters);

	/// <inheritdoc cref="Expression.Lambda(Type, Expression, IEnumerable{ParameterExpression})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Lambda(<see cref="Expression"/>.GetFuncType([..<paramref name="parameters"/>.Select(parameter => parameter.Type), <paramref name="returnType"/>]),
	/// @<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	public static LambdaExpression LambdaFunc(this Expression @this, Type returnType, IEnumerable<ParameterExpression> parameters)
		=> Expression.Lambda(Expression.GetFuncType([..parameters.Select(parameter => parameter.Type), returnType]), @this, parameters);

	/// <inheritdoc cref="Expression.Lambda(Type, Expression, ParameterExpression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Lambda(<see cref="Expression"/>.GetFuncType([..<paramref name="parameters"/>.Select(parameter => parameter.Type), <paramref name="returnType"/>]),
	/// @<paramref name="this"/>, <paramref name="parameters"/>);</c>
	/// </remarks>
	public static LambdaExpression LambdaFunc(this Expression @this, Type returnType, ParameterExpression[]? parameters = null)
		=> Expression.Lambda(Expression.GetFuncType([..parameters?.Select(parameter => parameter.Type) ?? [], returnType]), @this, parameters);

	/// <inheritdoc cref="Expression.PropertyOrField(Expression, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.PropertyOrField(@<paramref name="this"/>, <paramref name="name"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MemberExpression Member(this Expression @this, string name)
		=> Expression.PropertyOrField(@this, name);

	/// <inheritdoc cref="Expression.MemberInit(NewExpression, IEnumerable{MemberBinding})"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.MemberInit(@<paramref name="this"/>, <paramref name="bindings"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MemberInitExpression MemberInit(this NewExpression @this, IEnumerable<MemberBinding> bindings)
		=> Expression.MemberInit(@this, bindings);

	/// <inheritdoc cref="Expression.MemberInit(NewExpression, MemberBinding[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.MemberInit(@<paramref name="this"/>, <paramref name="bindings"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MemberInitExpression MemberInit(this NewExpression @this, MemberBinding[] bindings)
		=> Expression.MemberInit(@this, bindings);

	/// <exception cref="UnreachableException" />
	public static BinaryExpression Operator(this Expression @this, BinaryOperator binaryOperator, Expression operand)
		=> binaryOperator switch
		{
			BinaryOperator.Add => Expression.Add(@this, operand),
			BinaryOperator.AddAssign => Expression.AddAssign(@this, operand),
			BinaryOperator.AddChecked => Expression.AddChecked(@this, operand),
			BinaryOperator.AddAssignChecked => Expression.AddAssignChecked(@this, operand),
			BinaryOperator.Divide => Expression.Divide(@this, operand),
			BinaryOperator.DivideAssign => Expression.DivideAssign(@this, operand),
			BinaryOperator.Modulo => Expression.Modulo(@this, operand),
			BinaryOperator.ModuloAssign => Expression.ModuloAssign(@this, operand),
			BinaryOperator.Multiply => Expression.Multiply(@this, operand),
			BinaryOperator.MultiplyAssign => Expression.MultiplyAssign(@this, operand),
			BinaryOperator.MultiplyChecked => Expression.MultiplyChecked(@this, operand),
			BinaryOperator.MultiplyAssignChecked => Expression.MultiplyAssignChecked(@this, operand),
			BinaryOperator.Power => Expression.Power(@this, operand),
			BinaryOperator.PowerAssign => Expression.PowerAssign(@this, operand),
			BinaryOperator.Subtract => Expression.Subtract(@this, operand),
			BinaryOperator.SubtractAssign => Expression.SubtractAssign(@this, operand),
			BinaryOperator.SubtractChecked => Expression.SubtractChecked(@this, operand),
			BinaryOperator.SubtractAssignChecked => Expression.SubtractAssignChecked(@this, operand),
			BinaryOperator.And => Expression.And(@this, operand),
			BinaryOperator.AndAssign => Expression.AndAssign(@this, operand),
			BinaryOperator.Or => Expression.Or(@this, operand),
			BinaryOperator.OrAssign => Expression.OrAssign(@this, operand),
			BinaryOperator.ExclusiveOr => Expression.ExclusiveOr(@this, operand),
			BinaryOperator.ExclusiveOrAssign => Expression.ExclusiveOrAssign(@this, operand),
			BinaryOperator.LeftShift => Expression.LeftShift(@this, operand),
			BinaryOperator.LeftShiftAssign => Expression.LeftShiftAssign(@this, operand),
			BinaryOperator.RightShift => Expression.RightShift(@this, operand),
			BinaryOperator.RightShiftAssign => Expression.RightShiftAssign(@this, operand),
			BinaryOperator.Equal => Expression.Equal(@this, operand),
			BinaryOperator.NotEqual => Expression.NotEqual(@this, operand),
			BinaryOperator.ReferenceEqual => Expression.ReferenceEqual(@this, operand),
			BinaryOperator.ReferenceNotEqual => Expression.ReferenceNotEqual(@this, operand),
			BinaryOperator.GreaterThan => Expression.GreaterThan(@this, operand),
			BinaryOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(@this, operand),
			BinaryOperator.LessThan => Expression.LessThan(@this, operand),
			BinaryOperator.LessThanOrEqual => Expression.LessThanOrEqual(@this, operand),
			_ => throw new UnreachableException(Invariant($"{nameof(Operator)}: {nameof(BinaryOperator)} [{binaryOperator.Name()}] is not supported."))
		};

	/// <exception cref="UnreachableException" />
	public static UnaryExpression Operator(this Expression @this, UnaryOperator unaryOperator)
		=> unaryOperator switch
		{
			UnaryOperator.IsTrue => Expression.IsTrue(@this),
			UnaryOperator.IsFalse => Expression.IsFalse(@this),
			UnaryOperator.PreIncrement => Expression.PreIncrementAssign(@this),
			UnaryOperator.Increment => Expression.Increment(@this),
			UnaryOperator.PostIncrement => Expression.PostIncrementAssign(@this),
			UnaryOperator.PreDecrement => Expression.PreDecrementAssign(@this),
			UnaryOperator.Decrement => Expression.Decrement(@this),
			UnaryOperator.PostDecrement => Expression.PostDecrementAssign(@this),
			UnaryOperator.Negate => Expression.Negate(@this),
			UnaryOperator.NegateChecked => Expression.NegateChecked(@this),
			UnaryOperator.Complement => Expression.OnesComplement(@this),
			_ => throw new UnreachableException(Invariant($"{nameof(Operator)}: {nameof(UnaryOperator)} [{unaryOperator.Name()}] is not supported."))
		};

	/// <inheritdoc cref="Expression.OrElse(Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.OrElse(@<paramref name="this"/>, <paramref name="operand"/>);</c><br/><br/>
	/// <c>a || b</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static BinaryExpression Or(this Expression @this, Expression operand)
		=> Expression.OrElse(@this, operand);

	/// <inheritdoc cref="Expression.Property(Expression, MethodInfo)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Property(@<paramref name="this"/>, <paramref name="getMethodInfo"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MemberExpression Property(this Expression @this, MethodInfo getMethodInfo)
		=> Expression.Property(@this, getMethodInfo);

	/// <inheritdoc cref="Expression.Property(Expression, PropertyInfo)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Property(@<paramref name="this"/>, <paramref name="propertyInfo"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MemberExpression Property(this Expression @this, PropertyInfo propertyInfo)
		=> Expression.Property(@this, propertyInfo);

	/// <inheritdoc cref="Expression.Property(Expression, PropertyInfo, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Property(@<paramref name="this"/>, <paramref name="propertyInfo"/>, <paramref name="index"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static IndexExpression Property(this Expression @this, PropertyInfo propertyInfo, Expression[] index)
		=> Expression.Property(@this, propertyInfo, index);

	/// <inheritdoc cref="Expression.Property(Expression, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Property(@<paramref name="this"/>, <paramref name="name"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MemberExpression Property(this Expression @this, string name)
		=> Expression.Property(@this, name);

	/// <inheritdoc cref="Expression.Property(Expression, string, Expression[])"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Property(@<paramref name="this"/>, <paramref name="name"/>, <paramref name="index"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static IndexExpression Property(this Expression @this, string name, Expression[] index)
		=> Expression.Property(@this, name, index);

	/// <inheritdoc cref="Expression.Property(Expression, Type, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Property(@<paramref name="this"/>, <see langword="typeof"/>(<typeparamref name="T"/>), <paramref name="name"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MemberExpression Property<T>(this Expression @this, string name)
		=> Expression.Property(@this, typeof(T), name);

	/// <inheritdoc cref="Expression.Property(Expression, Type, string)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Property(@<paramref name="this"/>, <paramref name="type"/>, <paramref name="name"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static MemberExpression Property(this Expression @this, Type type, string name)
		=> Expression.Property(@this, type, name);

	/// <inheritdoc cref="Expression.TypeEqual(Expression, Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.TypeEqual(@<paramref name="this"/>, <see langword="typeof"/>(<typeparamref name="T"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static TypeBinaryExpression TypeEqual<T>(this Expression @this)
		=> Expression.TypeEqual(@this, typeof(T));

	/// <inheritdoc cref="Expression.TypeEqual(Expression, Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.TypeEqual(@<paramref name="this"/>, <paramref name="type"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static TypeBinaryExpression TypeEqual(this Expression @this, Type type)
		=> Expression.TypeEqual(@this, type);

	/// <inheritdoc cref="Expression.Unbox(Expression, Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Unbox(@<paramref name="this"/>, <see langword="typeof"/>(<typeparamref name="T"/>));</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static UnaryExpression Unbox<T>(this Expression @this)
		where T : struct
		=> Expression.Unbox(@this, typeof(T));

	/// <inheritdoc cref="Expression.Unbox(Expression, Type)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Unbox(@<paramref name="this"/>, <paramref name="type"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static UnaryExpression Unbox(this Expression @this, Type type)
		=> Expression.Unbox(@this, type);

	/// <inheritdoc cref="Expression.Block(Expression, Expression)"/>
	/// <remarks>
	/// <c>=&gt; <see cref="Expression"/>.Block(@<paramref name="this"/>, <see cref="Expression.Empty()"/>);</c>
	/// </remarks>
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public static BlockExpression Void(this MethodCallExpression @this)
		=> Expression.Block(@this, Expression.Empty());
}
