// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Reflection;

public enum ScalarType
{
	None = 0,
	/// <summary>
	/// <c><see cref="System.Numerics.BigInteger"/></c>
	/// </summary>
	BigInteger,
	/// <summary>
	/// <c><see cref="bool"/></c>
	/// </summary>
	Boolean,
	/// <summary>
	/// <c><see cref="byte"/></c>
	/// </summary>
	Byte,
	/// <summary>
	/// <c><see cref="char"/></c>
	/// </summary>
	Char,
	/// <summary>
	/// <c><see cref="System.DateOnly"/></c>
	/// </summary>
	DateOnly,
	/// <summary>
	/// <c><see cref="System.DateTime"/></c>
	/// </summary>
	DateTime,
	/// <summary>
	/// <c><see cref="System.DateTimeOffset"/></c>
	/// </summary>
	DateTimeOffset,
	/// <summary>
	/// <c><see cref="decimal"/></c>
	/// </summary>
	Decimal,
	/// <summary>
	/// <c><see cref="double"/></c>
	/// </summary>
	Double,
	/// <summary>
	/// <c><see cref="System.Enum"/></c>
	/// </summary>
	Enum,
	/// <summary>
	/// <c><see cref="System.Guid"/></c>
	/// </summary>
	Guid,
	/// <summary>
	/// <c><see cref="System.Half"/></c>
	/// </summary>
	Half,
	/// <summary>
	/// <c><see cref="System.Index"/></c>
	/// </summary>
	Index,
	/// <summary>
	/// <c><see cref="System.Int128"/></c>
	/// </summary>
	Int128,
	/// <summary>
	/// <c><see cref="short"/></c>
	/// </summary>
	Int16,
	/// <summary>
	/// <c><see cref="int"/></c>
	/// </summary>
	Int32,
	/// <summary>
	/// <c><see cref="long"/></c>
	/// </summary>
	Int64,
	/// <summary>
	/// <c><see cref="nint"/></c>
	/// </summary>
	IntPtr,
	/// <summary>
	/// <c><see cref="sbyte"/></c>
	/// </summary>
	SByte,
	/// <summary>
	/// <c><see cref="float"/></c>
	/// </summary>
	Single,
	/// <summary>
	/// <c><see cref="string"/></c>
	/// </summary>
	String,
	/// <summary>
	/// <c><see cref="System.TimeOnly"/></c>
	/// </summary>
	TimeOnly,
	/// <summary>
	/// <c><see cref="System.TimeSpan"/></c>
	/// </summary>
	TimeSpan,
	/// <summary>
	/// <c><see cref="System.UInt128"/></c>
	/// </summary>
	UInt128,
	/// <summary>
	/// <c><see cref="ushort"/></c>
	/// </summary>
	UInt16,
	/// <summary>
	/// <c><see cref="uint"/></c>
	/// </summary>
	UInt32,
	/// <summary>
	/// <c><see cref="ulong"/></c>
	/// </summary>
	UInt64,
	/// <summary>
	/// <c><see cref="nuint"/></c>
	/// </summary>
	UIntPtr,
	/// <summary>
	/// <c><see cref="System.Uri"/></c>
	/// </summary>
	Uri
}
