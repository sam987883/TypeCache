// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Reflection;

public enum Kind
{
	/// <summary>
	/// <c><see cref="Type.IsClass"/> <see langword="is true"/></c>
	/// </summary>
	Class,
	/// <summary>
	/// <c><see cref="Type.IsInterface"/> <see langword="is true"/></c>
	/// </summary>
	Interface,
	/// <summary>
	/// <c><see cref="Type.IsValueType"/> <see langword="is true"/></c>
	/// </summary>
	Struct,
	/// <summary>
	/// <c><see cref="Type.IsPointer"/> <see langword="is true"/></c>
	/// </summary>
	Pointer
}
