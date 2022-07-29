// Copyright (c) 2021 Samuel Abraham

using System;
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using TypeCache.Reflection;
using static TypeCache.Default;

namespace TypeCache.Data.Extensions;

public static class EnumExtensions
{
	private static readonly SystemType[] SQLSystemTypes =
	{
		SystemType.Boolean,
		SystemType.Byte,
		SystemType.SByte,
		SystemType.Int16,
		SystemType.UInt16,
		SystemType.Int32,
		SystemType.UInt32,
		SystemType.Int64,
		SystemType.UInt64,
		SystemType.Single,
		SystemType.Double,
		SystemType.Decimal,
		SystemType.DateOnly,
		SystemType.DateTime,
		SystemType.DateTimeOffset,
		SystemType.TimeOnly,
		SystemType.TimeSpan,
		SystemType.Guid,
		SystemType.Char,
		SystemType.String
	};

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public static bool IsSQLType(this SystemType @this)
		=> SQLSystemTypes.Has(@this);

	public static Type ToType(this SqlDbType @this)
		=> @this switch
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
