// Copyright (c) 2021 Samuel Abraham

using System.Data;
using System.Text.Json;
using TypeCache.Data.Extensions;
using TypeCache.Reflection;
using Xunit;

namespace TypeCache.Tests.Data.Extensions;

public class DataEnumExtensionsTests
{
	[Theory]
	[InlineData(ScalarType.Boolean, true)]
	[InlineData(ScalarType.SByte, true)]
	[InlineData(ScalarType.Byte, true)]
	[InlineData(ScalarType.Int16, true)]
	[InlineData(ScalarType.Int32, true)]
	[InlineData(ScalarType.Int64, true)]
	[InlineData(ScalarType.UInt16, true)]
	[InlineData(ScalarType.UInt32, true)]
	[InlineData(ScalarType.UInt64, true)]
	[InlineData(ScalarType.Single, true)]
	[InlineData(ScalarType.Double, true)]
	[InlineData(ScalarType.Decimal, true)]
	[InlineData(ScalarType.DateOnly, true)]
	[InlineData(ScalarType.DateTime, true)]
	[InlineData(ScalarType.DateTimeOffset, true)]
	[InlineData(ScalarType.TimeOnly, true)]
	[InlineData(ScalarType.TimeSpan, true)]
	[InlineData(ScalarType.Guid, true)]
	[InlineData(ScalarType.Char, true)]
	[InlineData(ScalarType.String, true)]
	[InlineData(ScalarType.None, false)]
	[InlineData(ScalarType.Enum, false)]
	[InlineData(ScalarType.BigInteger, false)]
	[InlineData(ScalarType.Half, false)]
	[InlineData(ScalarType.Index, false)]
	[InlineData(ScalarType.Int128, false)]
	[InlineData(ScalarType.IntPtr, false)]
	[InlineData(ScalarType.UInt128, false)]
	[InlineData(ScalarType.UIntPtr, false)]
	[InlineData(ScalarType.Uri, false)]
	public void IsSQLType(ScalarType scalarType, bool isSqlType)
	{
		Assert.Equal(isSqlType, scalarType.IsSQLType());
	}

	[Theory]
	[InlineData(SqlDbType.BigInt, typeof(long))]
	[InlineData(SqlDbType.Binary, typeof(byte[]))]
	[InlineData(SqlDbType.Bit, typeof(bool))]
	[InlineData(SqlDbType.Char, typeof(string))]
	[InlineData(SqlDbType.Date, typeof(DateOnly))]
	[InlineData(SqlDbType.DateTime, typeof(DateTime))]
	[InlineData(SqlDbType.DateTime2, typeof(DateTime))]
	[InlineData(SqlDbType.DateTimeOffset, typeof(DateTimeOffset))]
	[InlineData(SqlDbType.Decimal, typeof(decimal))]
	[InlineData(SqlDbType.Float, typeof(double))]
	[InlineData(SqlDbType.Image, typeof(byte[]))]
	[InlineData(SqlDbType.Int, typeof(int))]
	[InlineData(SqlDbType.Json, typeof(JsonDocument))]
	[InlineData(SqlDbType.Money, typeof(decimal))]
	[InlineData(SqlDbType.NChar, typeof(string))]
	[InlineData(SqlDbType.NText, typeof(string))]
	[InlineData(SqlDbType.NVarChar, typeof(string))]
	[InlineData(SqlDbType.Real, typeof(float))]
	[InlineData(SqlDbType.SmallDateTime, typeof(DateTime))]
	[InlineData(SqlDbType.SmallInt, typeof(short))]
	[InlineData(SqlDbType.SmallMoney, typeof(decimal))]
	[InlineData(SqlDbType.Time, typeof(TimeOnly))]
	[InlineData(SqlDbType.Timestamp, typeof(byte[]))]
	[InlineData(SqlDbType.TinyInt, typeof(sbyte))]
	[InlineData(SqlDbType.UniqueIdentifier, typeof(Guid))]
	[InlineData(SqlDbType.VarBinary, typeof(byte[]))]
	[InlineData(SqlDbType.VarChar, typeof(string))]
	[InlineData(SqlDbType.Variant, typeof(object))]
	[InlineData(SqlDbType.Xml, typeof(object))]
	[InlineData((SqlDbType)99999, typeof(object))]
	public void ToType(SqlDbType sqlDbType, Type type)
	{
		Assert.Equal(type, sqlDbType.ToType());
	}
}
