// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using TypeCache.Collections;
using TypeCache.Reflection;

namespace TypeCache.Extensions;

partial class ReflectionExtensions
{
	/// <inheritdoc cref="FieldInfo.GetValue(object?)"/>
	/// <remarks>
	/// <b>The code to get the field value is built once and used subsequently.<br/>
	/// This is much faster than late binding.<br/>
	/// In the case of a constant, <c><see cref="FieldInfo.GetRawConstantValue"/></c> is used instead.</b>
	/// </remarks>
	/// <param name="instance">Pass <c><see langword="null"/></c> if this is a static field.</param>
	public static object? GetFieldValue(this FieldInfo @this, object? instance)
		=> @this.IsLiteral
			? @this.GetRawConstantValue()
			: TypeStore.FieldGetInvokes.GetOrAdd(@this.FieldHandle, handle => @this.FieldGetInvoke().Compile())(instance);

	/// <inheritdoc cref="FieldInfo.SetValue(object, object)"/>
	/// <remarks>
	/// <b>The code to set the field is built once and used subsequently.<br/>
	/// This is much faster than late binding.</b>
	/// </remarks>
	/// <param name="instance">Pass <c><see langword="null"/></c> if this is a static field.</param>
	public static void SetFieldValue(this FieldInfo @this, object? instance, object? value)
	{
		if (!@this.IsInitOnly && !@this.IsLiteral)
			TypeStore.FieldSetInvokes.GetOrAdd(@this.FieldHandle, handle => @this.FieldSetInvoke().Compile())(instance, value);
	}
}
