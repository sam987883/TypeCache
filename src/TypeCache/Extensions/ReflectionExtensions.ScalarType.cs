// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Extensions;

public enum ScalarType
{
	None = 0,
	/// <summary>
	/// <c><see cref="System.Numerics.BigInteger"/></c>
	/// </summary>
	BigInteger,
	/// <summary>
	/// <c><see cref="System.Boolean"/></c>
	/// </summary>
	Boolean,
	/// <summary>
	/// <c><see cref="System.Byte"/></c>
	/// </summary>
	Byte,
	/// <summary>
	/// <c><see cref="System.Char"/></c>
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
	/// <c><see cref="System.DBNull"/></c>
	/// </summary>
	DBNull,
	/// <summary>
	/// <c><see cref="System.Decimal"/></c>
	/// </summary>
	Decimal,
	/// <summary>
	/// <c><see cref="System.Double"/></c>
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
	/// <c><see cref="System.Int16"/></c>
	/// </summary>
	Int16,
	/// <summary>
	/// <c><see cref="System.Int32"/></c>
	/// </summary>
	Int32,
	/// <summary>
	/// <c><see cref="System.Int64"/></c>
	/// </summary>
	Int64,
	/// <summary>
	/// <c><see cref="System.IntPtr"/></c>
	/// </summary>
	IntPtr,
	/// <summary>
	/// <c><see cref="System.SByte"/></c>
	/// </summary>
	SByte,
	/// <summary>
	/// <c><see cref="System.Single"/></c>
	/// </summary>
	Single,
	/// <summary>
	/// <c><see cref="System.String"/></c>
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
	/// <c><see cref="System.UInt16"/></c>
	/// </summary>
	UInt16,
	/// <summary>
	/// <c><see cref="System.UInt32"/></c>
	/// </summary>
	UInt32,
	/// <summary>
	/// <c><see cref="System.UInt64"/></c>
	/// </summary>
	UInt64,
	/// <summary>
	/// <c><see cref="System.UIntPtr"/></c>
	/// </summary>
	UIntPtr,
	/// <summary>
	/// <c><see cref="System.Uri"/></c>
	/// </summary>
	Uri
}
