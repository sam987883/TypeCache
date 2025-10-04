// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using TypeCache.Extensions;

namespace TypeCache.Reflection;

[DebuggerDisplay("Generic method: {Name}")]
public class GenericMethodDefinition : Method
{
	public GenericMethodDefinition(MethodInfo methodInfo)
		: base(methodInfo)
	{
		methodInfo.IsGenericMethodDefinition.ThrowIfFalse();

		this.HasReturnValue = methodInfo.ReturnType != typeof(void);
		this.Return = new(methodInfo.ReturnParameter);
	}

	public bool HasReturnValue { get; }

	public ParameterEntity Return { get; }

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
	public Method MakeGenericMethod(Type[] genericTypeArguments)
		=> !this.IsStatic
			? new MethodEntity(this.ToMethodInfo().MakeGenericMethod(genericTypeArguments))
			: new StaticMethodEntity(this.ToMethodInfo().MakeGenericMethod(genericTypeArguments));

	public bool Supports(Type[] genericTypes)
		=> this.GenericTypes.Zip(genericTypes).All(_ => _.Second.IsAssignableTo(_.First));

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public MethodInfo ToMethodInfo()
		=> (MethodInfo)this.Handle.ToMethodBase(this._TypeHandle);
}
