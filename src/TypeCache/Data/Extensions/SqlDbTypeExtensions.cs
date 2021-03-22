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
				SqlDbType.Bit => typeof(bool),
				SqlDbType.TinyInt => typeof(sbyte),
				SqlDbType.SmallInt => typeof(short),
				SqlDbType.Int => typeof(int),
				SqlDbType.BigInt => typeof(long),
				SqlDbType.Binary or SqlDbType.Image or SqlDbType.Timestamp or SqlDbType.VarBinary => typeof(byte[]),
				SqlDbType.Char or SqlDbType.Text or SqlDbType.VarChar or SqlDbType.NChar or SqlDbType.NText or SqlDbType.NVarChar => typeof(string),
				SqlDbType.Date or SqlDbType.DateTime or SqlDbType.DateTime2 or SqlDbType.SmallDateTime => typeof(DateTime),
				SqlDbType.DateTimeOffset => typeof(DateTimeOffset),
				SqlDbType.Time => typeof(TimeSpan),
				SqlDbType.Real => typeof(float),
				SqlDbType.Float => typeof(double),
				SqlDbType.Decimal or SqlDbType.Money or SqlDbType.SmallMoney => typeof(decimal),
				SqlDbType.UniqueIdentifier => typeof(Guid),
				_ => typeof(object)
			};
	}
}
