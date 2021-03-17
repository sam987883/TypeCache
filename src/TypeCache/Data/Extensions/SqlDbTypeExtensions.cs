// Copyright (c) 2021 Samuel Abraham

using System;
using System.Data;

namespace TypeCache.Data.Extensions
{
	public static class SqlDbTypeExtensions
	{
		public static Type ToType(this SqlDbType @this)
			=> @this switch
			{
				SqlDbType.BigInt => typeof(long),
				SqlDbType.Binary => typeof(byte),
				SqlDbType.Bit => typeof(bool),
				SqlDbType.Char => typeof(string),
				SqlDbType.Date => typeof(DateTime),
				SqlDbType.DateTime => typeof(DateTime),
				SqlDbType.DateTime2 => typeof(DateTime),
				SqlDbType.DateTimeOffset => typeof(DateTimeOffset),
				SqlDbType.Decimal => typeof(decimal),
				SqlDbType.Float => typeof(double),
				SqlDbType.Image => typeof(byte[]),
				SqlDbType.Int => typeof(int),
				SqlDbType.Money => typeof(decimal),
				SqlDbType.NChar => typeof(string),
				SqlDbType.NText => typeof(string),
				SqlDbType.NVarChar => typeof(string),
				SqlDbType.Real => typeof(float),
				SqlDbType.SmallDateTime => typeof(DateTime),
				SqlDbType.SmallInt => typeof(short),
				SqlDbType.SmallMoney => typeof(decimal),
				SqlDbType.Text => typeof(string),
				SqlDbType.Time => typeof(TimeSpan),
				SqlDbType.Timestamp => typeof(byte[]),
				SqlDbType.TinyInt => typeof(sbyte),
				SqlDbType.UniqueIdentifier => typeof(Guid),
				SqlDbType.VarBinary => typeof(byte[]),
				SqlDbType.VarChar => typeof(string),
				_ => typeof(object)
			};
	}
}
