// Copyright (c) 2021 Samuel Abraham

using System.Data;
using TypeCache.Extensions;

namespace TypeCache.Data.Extensions;

public static class EnumExtensions
{
	[DebuggerHidden]
	public static bool IsSQLType(this SystemType @this) => @this switch
	{
		SystemType.Boolean
		or SystemType.SByte or SystemType.Byte
		or SystemType.Int16 or SystemType.Int32 or SystemType.Int64
		or SystemType.UInt16 or SystemType.UInt32 or SystemType.UInt64
		or SystemType.Single or SystemType.Double or SystemType.Decimal
		or SystemType.DateOnly or SystemType.DateTime or SystemType.DateTimeOffset
		or SystemType.TimeOnly or SystemType.TimeSpan
		or SystemType.Guid or SystemType.Char or SystemType.String => true,
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
