// Copyright (c) 2021 Samuel Abraham

using System.Data;
using TypeCache.Reflection;

namespace TypeCache.Data.Extensions;

public static class EnumExtensions
{
	[DebuggerHidden]
	public static bool IsSQLType(this ScalarType @this) => @this switch
	{
		ScalarType.Boolean
		or ScalarType.SByte or ScalarType.Byte
		or ScalarType.Int16 or ScalarType.Int32 or ScalarType.Int64
		or ScalarType.UInt16 or ScalarType.UInt32 or ScalarType.UInt64
		or ScalarType.Single or ScalarType.Double or ScalarType.Decimal
		or ScalarType.DateOnly or ScalarType.DateTime or ScalarType.DateTimeOffset
		or ScalarType.TimeOnly or ScalarType.TimeSpan
		or ScalarType.Guid or ScalarType.Char or ScalarType.String => true,
		_ => false
	};

	[DebuggerHidden]
	public static Type ToType(this SqlDbType @this) => @this switch
	{
		SqlDbType.Bit => typeof(bool),
		SqlDbType.TinyInt => typeof(sbyte),
		SqlDbType.SmallInt => typeof(short),
		SqlDbType.Int => typeof(int),
		SqlDbType.BigInt => typeof(long),
		SqlDbType.Binary or SqlDbType.Image or SqlDbType.Timestamp or SqlDbType.VarBinary => typeof(byte[]),
		SqlDbType.Char or SqlDbType.Text or SqlDbType.VarChar or SqlDbType.NChar or SqlDbType.NText or SqlDbType.NVarChar => typeof(string),
		SqlDbType.Date => typeof(DateOnly),
		SqlDbType.DateTime or SqlDbType.DateTime2 or SqlDbType.SmallDateTime => typeof(DateTime),
		SqlDbType.DateTimeOffset => typeof(DateTimeOffset),
		SqlDbType.Time => typeof(TimeOnly),
		SqlDbType.Real => typeof(float),
		SqlDbType.Float => typeof(double),
		SqlDbType.Decimal or SqlDbType.Money or SqlDbType.SmallMoney => typeof(decimal),
		SqlDbType.UniqueIdentifier => typeof(Guid),
		_ => typeof(object)
	};
}
