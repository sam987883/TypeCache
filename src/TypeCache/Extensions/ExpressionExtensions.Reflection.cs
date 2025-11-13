// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using System.Reflection;

namespace TypeCache.Extensions;

public static partial class ExpressionExtensions
{
	extension(ConstructorInfo @this)
	{
		/// <inheritdoc cref="Expression.New(ConstructorInfo)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Expression"/>.New(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public NewExpression ToExpression()
			=> Expression.New(@this);

		/// <inheritdoc cref="Expression.New(ConstructorInfo, IEnumerable{Expression})"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Expression"/>.New(@this, <paramref name="parameters"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public NewExpression ToExpression(IEnumerable<Expression> parameters)
			=> Expression.New(@this, parameters);

		/// <inheritdoc cref="Expression.New(ConstructorInfo, Expression[])"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Expression"/>.New(@this, <paramref name="parameters"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public NewExpression ToExpression(Expression[]? parameters = null)
			=> Expression.New(@this, parameters);

		/// <inheritdoc cref="Expression.New(ConstructorInfo, IEnumerable{Expression}, IEnumerable{MemberInfo})"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Expression"/>.New(@this, <paramref name="parameters"/>, <paramref name="memberInfos"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public NewExpression ToExpression(IEnumerable<Expression> parameters, IEnumerable<MemberInfo> memberInfos)
			=> Expression.New(@this, parameters, memberInfos);

		/// <inheritdoc cref="Expression.New(ConstructorInfo, IEnumerable{Expression}, MemberInfo[])"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Expression"/>.New(@this, <paramref name="parameters"/>, <paramref name="memberInfos"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public NewExpression ToExpression(IEnumerable<Expression> parameters, MemberInfo[]? memberInfos = null)
			=> Expression.New(@this, parameters, memberInfos);
	}

	extension(FieldInfo @this)
	{
		/// <inheritdoc cref="Expression.Field(Expression, FieldInfo)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Expression"/>.Field(<paramref name="instance"/>, @this);</c>
		/// </remarks>
		/// <param name="instance">Pass <c><see langword="null"/></c> for static field access.</param>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public MemberExpression ToExpression(Expression? instance)
			=> Expression.Field(instance, @this);
	}

	extension(PropertyInfo @this)
	{
		/// <inheritdoc cref="Expression.Property(Expression, PropertyInfo)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Expression"/>.Property(<paramref name="instance"/>, @this);</c>
		/// </remarks>
		/// <param name="instance">Pass <c><see langword="null"/></c> for static property access.</param>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public MemberExpression ToExpression(Expression? instance)
			=> Expression.Property(instance, @this);

		/// <inheritdoc cref="Expression.Property(Expression?, PropertyInfo, IEnumerable{Expression})"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Expression"/>.Property(<paramref name="instance"/>, @this, <paramref name="indexes"/>);</c>
		/// </remarks>
		/// <param name="instance">Pass <c><see langword="null"/></c> for static property access.</param>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public IndexExpression ToExpression(Expression? instance, IEnumerable<Expression> indexes)
			=> Expression.Property(instance, @this, indexes);

		/// <inheritdoc cref="Expression.Property(Expression?, PropertyInfo, Expression[])"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Expression"/>.Property(<paramref name="instance"/>, @this, <paramref name="indexes"/>);</c>
		/// </remarks>
		/// <param name="instance">Pass <c><see langword="null"/></c> for static property access.</param>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public IndexExpression ToExpression(Expression? instance, Expression[] indexes)
			=> Expression.Property(instance, @this, indexes);
	}

	extension(MethodInfo @this)
	{
		/// <inheritdoc cref="Expression.Call(MethodInfo, IEnumerable{Expression}?)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Expression"/>.Call(@this, <paramref name="arguments"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public MethodCallExpression ToExpression(Expression? instance, IEnumerable<Expression> arguments)
		=> Expression.Call(instance, @this, arguments);

		/// <inheritdoc cref="Expression.Call(MethodInfo, Expression[])"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Expression"/>.Call(@this, <paramref name="instance"/>, <paramref name="arguments"/>);</c>
		/// </remarks>
		/// <param name="instance">Pass <c><see langword="null"/></c> for static method call.</param>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public MethodCallExpression ToExpression(Expression? instance, Expression[]? arguments = null)
			=> Expression.Call(instance, @this, arguments);
	}

	extension(ParameterInfo @this)
	{
		/// <inheritdoc cref="Expression.Parameter(Type, string)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Expression"/>.Parameter(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public ParameterExpression ToExpression()
			=> Expression.Parameter(@this.ParameterType, @this.Name);
	}

	extension(Type @this)
	{
		/// <inheritdoc cref="Expression.Default(Type)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Expression"/>.Default(@this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public DefaultExpression ToDefaultExpression()
			=> Expression.Default(@this);

		/// <inheritdoc cref="Expression.Field(Expression, Type, string)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Expression"/>.Field(<see langword="null"/>, @this, <paramref name="name"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public MemberExpression ToStaticFieldExpression(string name)
			=> Expression.Field(null, @this, name);

		/// <inheritdoc cref="Expression.Property(Expression, Type, string)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Expression"/>.Property(<see langword="null"/>, @this, <paramref name="name"/>);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public MemberExpression ToStaticPropertyExpression(string name)
			=> Expression.Property(null, @this, name);
	}

	extension(string @this)
	{
		/// <inheritdoc cref="Expression.Parameter(Type, string)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Expression"/>.Parameter(<see langword="typeof"/>(<typeparamref name="T"/>), @this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public ParameterExpression ToParameterExpression<T>()
		=> Expression.Parameter(typeof(T), @this);

		/// <inheritdoc cref="Expression.Parameter(Type, string)"/>
		/// <remarks>
		/// <c>=&gt; <see cref="Expression"/>.Parameter(<paramref name="type"/>, @this);</c>
		/// </remarks>
		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public ParameterExpression ToParameterExpression(Type type)
			=> Expression.Parameter(type, @this);
	}
}
