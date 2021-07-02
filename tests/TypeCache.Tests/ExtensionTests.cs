// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Json;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests
{
	public class ExtensionTests
	{
		[Fact]
		public void AssertExtensionTests()
		{
			const string NAME = "TestName";

			123456.Assert(NAME, 123456);
			"AAA".Assert(NAME, "AAA");
			(null as string).Assert(NAME, null);
			"AAA".Assert(NAME, "AAA", StringComparer.Ordinal);
			Assert.Throws<ArgumentOutOfRangeException>(() => 123.Assert(NAME, 456));
			Assert.Throws<ArgumentOutOfRangeException>(() => "AAA".Assert(NAME, "bbb"));
			Assert.Throws<ArgumentOutOfRangeException>(() => (null as string).Assert(NAME, "bbb"));
			Assert.Throws<ArgumentOutOfRangeException>(() => "AAA".Assert(NAME, "bbb", StringComparer.Ordinal));
			Assert.Throws<ArgumentNullException>(() => "AAA".Assert(NAME, null, null));

			((int?)123456).AssertNotNull(NAME);
			"AAA".AssertNotNull(NAME);
			Assert.Throws<ArgumentNullException>(() => (null as string).AssertNotNull(NAME));
			Assert.Throws<ArgumentNullException>(() => (null as int?).AssertNotNull(NAME));

			"AAA".AssertNotBlank(NAME);
			Assert.Throws<ArgumentOutOfRangeException>(() => (null as string).AssertNotBlank(NAME));
			Assert.Throws<ArgumentOutOfRangeException>(() => string.Empty.AssertNotBlank(NAME));
			Assert.Throws<ArgumentOutOfRangeException>(() => "      ".AssertNotBlank(NAME));
		}

		[Fact]
		public void BitConverterExtensionTests()
		{
			Assert.True(true.ToBytes().ToBoolean());
			Assert.False(false.ToBytes().ToBoolean());
			Assert.Equal(char.MinValue, char.MinValue.ToBytes().ToChar());
			Assert.Equal(char.MaxValue, char.MaxValue.ToBytes().ToChar());
			Assert.Equal(short.MinValue, short.MinValue.ToBytes().ToInt16());
			Assert.Equal(short.MaxValue, short.MaxValue.ToBytes().ToInt16());
			Assert.Equal(ushort.MinValue, ushort.MinValue.ToBytes().ToUInt16());
			Assert.Equal(ushort.MaxValue, ushort.MaxValue.ToBytes().ToUInt16());
			Assert.Equal(int.MinValue, int.MinValue.ToBytes().ToInt32());
			Assert.Equal(int.MaxValue, int.MaxValue.ToBytes().ToInt32());
			Assert.Equal(uint.MinValue, uint.MinValue.ToBytes().ToUInt32());
			Assert.Equal(uint.MaxValue, uint.MaxValue.ToBytes().ToUInt32());
			Assert.Equal(long.MinValue, long.MinValue.ToBytes().ToInt64());
			Assert.Equal(long.MaxValue, long.MaxValue.ToBytes().ToInt64());
			Assert.Equal(ulong.MinValue, ulong.MinValue.ToBytes().ToUInt64());
			Assert.Equal(ulong.MaxValue, ulong.MaxValue.ToBytes().ToUInt64());
			Assert.Equal(float.MinValue, float.MinValue.ToBytes().ToSingle());
			Assert.Equal(float.MaxValue, float.MaxValue.ToBytes().ToSingle());
			Assert.Equal(double.MinValue, double.MinValue.ToBytes().ToDouble());
			Assert.Equal(double.MaxValue, double.MaxValue.ToBytes().ToDouble());
			Assert.Equal(16, (-999999.9999M).ToBytes().Length);
			Assert.Equal(16, 999999.9999M.ToBytes().Length);
			Assert.Equal(float.MinValue, float.MinValue.ToInt32().ToSingle());
			Assert.Equal(float.MaxValue, float.MaxValue.ToInt32().ToSingle());
			Assert.Equal(double.MinValue, double.MinValue.ToInt64().ToDouble());
			Assert.Equal(double.MaxValue, double.MaxValue.ToInt64().ToDouble());
			Assert.NotEmpty(decimal.MinValue.ToBytes());
			Assert.NotEmpty(decimal.MaxValue.ToBytes());

			Assert.True(((ReadOnlySpan<byte>)true.ToBytes().AsSpan()).ToBoolean());
			Assert.False(((ReadOnlySpan<byte>)false.ToBytes().AsSpan()).ToBoolean());
			Assert.Equal(char.MinValue, ((ReadOnlySpan<byte>)char.MinValue.ToBytes()).ToChar());
			Assert.Equal(char.MaxValue, ((ReadOnlySpan<byte>)char.MaxValue.ToBytes()).ToChar());
			Assert.Equal(short.MinValue, ((ReadOnlySpan<byte>)short.MinValue.ToBytes()).ToInt16());
			Assert.Equal(short.MaxValue, ((ReadOnlySpan<byte>)short.MaxValue.ToBytes()).ToInt16());
			Assert.Equal(ushort.MinValue, ((ReadOnlySpan<byte>)ushort.MinValue.ToBytes()).ToUInt16());
			Assert.Equal(ushort.MaxValue, ((ReadOnlySpan<byte>)ushort.MaxValue.ToBytes()).ToUInt16());
			Assert.Equal(int.MinValue, ((ReadOnlySpan<byte>)int.MinValue.ToBytes()).ToInt32());
			Assert.Equal(int.MaxValue, ((ReadOnlySpan<byte>)int.MaxValue.ToBytes()).ToInt32());
			Assert.Equal(uint.MinValue, ((ReadOnlySpan<byte>)uint.MinValue.ToBytes()).ToUInt32());
			Assert.Equal(uint.MaxValue, ((ReadOnlySpan<byte>)uint.MaxValue.ToBytes()).ToUInt32());
			Assert.Equal(long.MinValue, ((ReadOnlySpan<byte>)long.MinValue.ToBytes()).ToInt64());
			Assert.Equal(long.MaxValue, ((ReadOnlySpan<byte>)long.MaxValue.ToBytes()).ToInt64());
			Assert.Equal(ulong.MinValue, ((ReadOnlySpan<byte>)ulong.MinValue.ToBytes()).ToUInt64());
			Assert.Equal(ulong.MaxValue, ((ReadOnlySpan<byte>)ulong.MaxValue.ToBytes()).ToUInt64());
			Assert.Equal(float.MinValue, ((ReadOnlySpan<byte>)float.MinValue.ToBytes()).ToSingle());
			Assert.Equal(float.MaxValue, ((ReadOnlySpan<byte>)float.MaxValue.ToBytes()).ToSingle());
			Assert.Equal(double.MinValue, ((ReadOnlySpan<byte>)double.MinValue.ToBytes()).ToDouble());
			Assert.Equal(double.MaxValue, ((ReadOnlySpan<byte>)double.MaxValue.ToBytes()).ToDouble());
		}

		[Fact]
		public void CharExtensionTests()
		{
			Assert.False('f'.IsControl());

			Assert.False('f'.IsDigit());
			Assert.True('6'.IsDigit());

			Assert.False('f'.IsHighSurrogate());

			Assert.False('6'.IsLetter());
			Assert.True('f'.IsLetter());

			Assert.True('f'.IsLetterOrDigit());
			Assert.True('6'.IsLetterOrDigit());

			Assert.True('f'.IsLower());
			Assert.False('F'.IsLower());

			Assert.False('f'.IsLowSurrogate());

			Assert.True('6'.IsNumber());
			Assert.False('f'.IsNumber());

			Assert.False('6'.IsPunctuation());
			Assert.True('!'.IsPunctuation());

			Assert.False('f'.IsSeparator());
			Assert.True(' '.IsSeparator());

			Assert.False('f'.IsSurrogate());

			Assert.False('f'.IsSymbol());
			Assert.False('#'.IsSymbol());

			Assert.False('f'.IsUpper());
			Assert.True('F'.IsUpper());

			Assert.False('f'.IsWhiteSpace());
			Assert.True(' '.IsWhiteSpace());

			Assert.Equal("A,b,C,1,2,3", ','.Join("A", "b", "C", "1", "2", "3"));
			Assert.Equal("A.b.C.1.2.3", '.'.Join((IEnumerable<string>)new[] { "A", "b", "C", "1", "2", "3" }));
			Assert.Equal("A;b;C;1;2;3", ';'.Join('A', 'b', 'C', 1, 2, 3));
			Assert.Equal("A|b|C|1|2|3", '|'.Join((IEnumerable<object>)new object[] { 'A', 'b', 'C', 1, 2, 3 }));

			Assert.Equal('f', 'F'.ToLower());

			Assert.Equal(0D, '0'.ToNumber());

			Assert.Equal('F', 'f'.ToUpper());

			Assert.Equal(UnicodeCategory.LowercaseLetter, 'f'.ToUnicodeCategory());
			Assert.Equal(UnicodeCategory.UppercaseLetter, 'F'.ToUnicodeCategory());
		}

		[Fact]
		public void DateTimeExtensionTests()
		{
			var now = DateTime.Now;
			var nowOffset = DateTimeOffset.Now;
			var utcNow = DateTime.UtcNow;

			Assert.Equal(DateTimeKind.Local, utcNow.As(DateTimeKind.Local).Kind);
			Assert.Equal(DateTimeKind.Unspecified, utcNow.As(DateTimeKind.Unspecified).Kind);
			Assert.Equal(DateTimeKind.Utc, now.As(DateTimeKind.Utc).Kind);

			Assert.Equal(now, now.ConvertTime(TimeZoneInfo.Local, TimeZoneInfo.Utc).ConvertTime(TimeZoneInfo.Utc, TimeZoneInfo.Local));

			Assert.Equal(now, now.ConvertTime(TimeZoneInfo.Local.Id, TimeZoneInfo.Utc.Id).ConvertTime(TimeZoneInfo.Utc.Id, TimeZoneInfo.Local.Id));

			Assert.Equal(now, now.ConvertTimeToUTC(TimeZoneInfo.Local).ConvertTime(TimeZoneInfo.Utc.Id, TimeZoneInfo.Local.Id));

			Assert.Equal(now, now.To(DateTimeKind.Utc).To(DateTimeKind.Local));

			Assert.Equal(utcNow, utcNow.To(DateTimeKind.Local).To(DateTimeKind.Utc));

			Assert.Equal(now, now.To(TimeZoneInfo.Utc).To(TimeZoneInfo.Local));

			Assert.Equal(nowOffset, nowOffset.To(TimeZoneInfo.Utc).To(TimeZoneInfo.Local));

			Assert.Equal(nowOffset, nowOffset.To(TimeZoneInfo.Utc.Id).To(TimeZoneInfo.Local.Id));
		}

		[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
		private class TestAttribute : Attribute
		{
		}

		private enum TestEnum
		{
			TestValue1 = 1,
			[TestAttribute]
			[TestAttribute]
			[TestAttribute]
			TestValue2 = 2,
			TestValue3 = 3,
		}

		[Fact]
		public void EnumExtensionTests()
		{
			Assert.Equal(3, TestEnum.TestValue2.Attributes().Count);

			Assert.Equal(TestEnum.TestValue2.ToString("X"), TestEnum.TestValue2.Hex());

			Assert.Equal(TestEnum.TestValue2.ToString("G"), TestEnum.TestValue2.Name());

			Assert.Equal(TestEnum.TestValue2.ToString("D"), TestEnum.TestValue2.Number());

			Assert.Equal(StringComparer.CurrentCulture, StringComparison.CurrentCulture.ToStringComparer());
			Assert.Equal(StringComparer.CurrentCultureIgnoreCase, StringComparison.CurrentCultureIgnoreCase.ToStringComparer());
			Assert.Equal(StringComparer.InvariantCulture, StringComparison.InvariantCulture.ToStringComparer());
			Assert.Equal(StringComparer.InvariantCultureIgnoreCase, StringComparison.InvariantCultureIgnoreCase.ToStringComparer());
			Assert.Equal(StringComparer.Ordinal, StringComparison.Ordinal.ToStringComparer());
			Assert.Equal(StringComparer.OrdinalIgnoreCase, StringComparison.OrdinalIgnoreCase.ToStringComparer());
			Assert.Throws<ArgumentException>(() => ((StringComparison)666).ToStringComparer());
		}

		[Fact]
		public void JsonExtensionTests()
		{
			var arrayJson = JsonSerializer.Deserialize<JsonElement>(@$"[null,true,3,{long.MaxValue},{ulong.MaxValue},""{DateTime.UtcNow.ToShortDateString()}"",""{Guid.NewGuid().ToString("D")}"",""Word""]");
			var objectJson = JsonSerializer.Deserialize<JsonElement>(@"{""a"": 1, ""B"": 2, ""c"": 3, ""D"": 4, ""e"": 5, ""F"": 6}");

			Assert.Equal(8, arrayJson.GetArrayElements().Length);

			Assert.Equal(8, arrayJson.GetArrayValues().Length);

			Assert.Equal(6, objectJson.GetObjectElements().Count);

			Assert.Equal(6, objectJson.GetObjectValues().Count);
		}

		[Fact]
		public void MathExtensionTests()
		{
			Assert.Equal(-(sbyte.MinValue + 1), (sbyte.MinValue + 1).AbsoluteValue());
			Assert.Equal(-(short.MinValue + 1), (short.MinValue + 1).AbsoluteValue());
			Assert.Equal(-(int.MinValue + 1), (int.MinValue + 1).AbsoluteValue());
			Assert.Equal(-(long.MinValue + 1), (long.MinValue + 1).AbsoluteValue());
			Assert.Equal(1F, (-1F).AbsoluteValue());
			Assert.Equal(1D, (-1D).AbsoluteValue());
			Assert.Equal(1M, (-1M).AbsoluteValue());

			Assert.Equal(1D, 1D.BitIncrement().BitDecrement());

			Assert.Equal(2D, 1.111D.Ceiling());
			Assert.Equal(2M, 1.111M.Ceiling());

			Assert.Equal(1D, 1.111D.Floor());
			Assert.Equal(1M, 1.111M.Floor());

			Assert.Equal(123D, 123.456D.Round());
			Assert.Equal(123.5D, 123.456D.Round(1));
			Assert.Equal(123D, 123.456D.Round(MidpointRounding.AwayFromZero));
			Assert.Equal(123.46D, 123.456D.Round(2, MidpointRounding.ToEven));
			Assert.Equal(123M, 123.456M.Round());
			Assert.Equal(123.5M, 123.456M.Round(1));
			Assert.Equal(123M, 123.456M.Round(MidpointRounding.AwayFromZero));
			Assert.Equal(123.46M, 123.456M.Round(2, MidpointRounding.ToEven));

			Assert.Equal(1, sbyte.MaxValue.Sign());
			Assert.Equal(-1, sbyte.MinValue.Sign());
			Assert.Equal(1, short.MaxValue.Sign());
			Assert.Equal(-1, short.MinValue.Sign());
			Assert.Equal(1, int.MaxValue.Sign());
			Assert.Equal(-1, int.MinValue.Sign());
			Assert.Equal(1, long.MaxValue.Sign());
			Assert.Equal(-1, long.MinValue.Sign());
			Assert.Equal(-1, -666F.Sign());
			Assert.Equal(1, 666F.Sign());
			Assert.Equal(-1, -666D.Sign());
			Assert.Equal(1, 666D.Sign());
			Assert.Equal(-1, -666M.Sign());
			Assert.Equal(1, 666M.Sign());

			Assert.Equal(-123D, -123.456D.Truncate());
			Assert.Equal(123M, 123.456M.Truncate());
		}

		[Fact]
		public void StringExtensionTests()
		{
			const string TEST_STRING = "AaBbCc 123 `~!#$%^\t\r\n";

			Assert.Equal(TEST_STRING, TEST_STRING.ToBase64(Encoding.ASCII).FromBase64(Encoding.ASCII));
			Assert.Equal(TEST_STRING, TEST_STRING.ToBase64(Encoding.ASCII).FromBase64(Encoding.UTF8));

			Assert.True(TEST_STRING.Has("BCC 1"));
			Assert.False(TEST_STRING.Has("BCC 1", StringComparison.Ordinal));

			Assert.True(TEST_STRING.Is("AABBCC 123 `~!#$%^\t\r\n"));
			Assert.False(TEST_STRING.Is("AABBCC 123 `~!#$%^\t\r\n", StringComparison.Ordinal));

			Assert.True(string.Empty.IsBlank());
			Assert.True(" \t \r \n ".IsBlank());
			Assert.True((null as string).IsBlank());
			Assert.False(TEST_STRING.IsBlank());

			Assert.Equal(TEST_STRING, " ".Join("AaBbCc", "123", "`~!#$%^\t\r\n"));
			Assert.Equal(TEST_STRING, " ".Join(TEST_STRING));

			Assert.Equal("AaBbCc 1", TEST_STRING.Left(8));
			Assert.Equal(string.Empty, TEST_STRING.Left(0));

			Assert.True(TEST_STRING.Left('A'));
			Assert.False(TEST_STRING.Left('a'));

			Assert.True(TEST_STRING.Left("AABBCC 123"));
			Assert.False(TEST_STRING.Left("AABBCC 123", StringComparison.Ordinal));

			Assert.Equal(string.Empty, (null as string).Mask());
			Assert.Equal("++++++ +++ `~!#$%^\t\r\n", TEST_STRING.Mask('+'));

			Assert.Equal("--Bb-- 123 `~!#$%^\t\r\n", TEST_STRING.MaskHide('-', StringComparison.OrdinalIgnoreCase, "A", "C", "\t\r\n"));

			Assert.Equal("ooBboo 123 `~!#$%^\t\r\n", TEST_STRING.MaskShow('o', StringComparison.Ordinal, "Bb", " ", "123", "`~!#$%^"));

			Assert.Equal(string.Empty, string.Empty.Reverse());
			Assert.Equal("321 cCbBaA", "AaBbCc 123".Reverse());

			Assert.True("321 cCbBaA".Right('A'));
			Assert.False("321 cCbBaA".Right('a'));

			Assert.True("321 cCbBaA".Right("ccbbaa"));
			Assert.False("321 cCbBaA".Right("ccbbaa", StringComparison.Ordinal));

			Assert.Equal(TEST_STRING, TEST_STRING.ToBytes(Encoding.UTF8).ToText(Encoding.UTF8));

			Assert.Equal("BbCc 1", TEST_STRING.ToBytes(Encoding.UTF8).ToText(Encoding.UTF8, 2, 6));

			Assert.Equal(TEST_STRING, ((ReadOnlySpan<byte>)TEST_STRING.ToBytes(Encoding.UTF8).AsSpan()).ToText(Encoding.UTF8));

			Assert.Equal(StringComparison.Ordinal, nameof(StringComparison.Ordinal).ToEnum<StringComparison>());
			Assert.Equal(StringComparison.OrdinalIgnoreCase, nameof(StringComparison.OrdinalIgnoreCase).ToUpperInvariant().ToEnum<StringComparison>());

			Assert.Equal("AaB", TEST_STRING.TrimEnd("BCC 123 `~!#$%^\t\r\n"));
			Assert.NotEqual("AaB", TEST_STRING.TrimEnd("BCC 123 `~!#$%^\t\r\n", StringComparison.Ordinal));
			Assert.Equal(TEST_STRING, TEST_STRING.TrimEnd("******"));

			Assert.Equal(" `~!#$%^\t\r\n", TEST_STRING.TrimStart("aabbcc 123"));
			Assert.NotEqual(" `~!#$%^\t\r\n", TEST_STRING.TrimStart("aabbcc 123", StringComparison.Ordinal));
			Assert.Equal(TEST_STRING, TEST_STRING.TrimStart("******"));
		}

		[Fact]
		public void ValueExtensionTests()
		{
			Assert.Equal(new[] { 'f', 'f', 'f', 'f', 'f', 'f' }, 'f'.Repeat(6).ToArray());
			Assert.Equal(Array<int>.Empty, 123.Repeat(0).ToArray());
			Assert.Equal(Array<int>.Empty, 123.Repeat(-18).ToArray());

			Assert.True(new Range(Index.FromStart(20), Index.FromStart(3)).IsReverse());
			Assert.False(new Range(Index.FromStart(6), Index.FromStart(6)).IsReverse());
			Assert.False(new Range(Index.FromStart(3), Index.FromStart(20)).IsReverse());

			Assert.Equal(Index.FromStart(15), Index.FromEnd(5).Normalize(20));
			Assert.Equal(Index.FromStart(5), Index.FromStart(5).Normalize(20));

			Assert.Equal(new Range(Index.FromStart(5), Index.FromStart(15)), new Range(Index.FromEnd(15), Index.FromEnd(5)).Normalize(20));
			Assert.Equal(new Range(Index.FromStart(5), Index.FromStart(15)), new Range(Index.FromStart(5), Index.FromStart(15)).Normalize(20));

			Assert.Equal(new[] { -2, -1, 0, 1, 2 }, (-2).Range(5).ToArray());
			Assert.Equal(new[] { 2, 2, 2, 2, 2, 2 }, 2.Range(6, 0).ToArray());
			Assert.Equal(new[] { 9, 6, 3, 0, -3, -6 }, 9.Range(6, -3).ToArray());
			Assert.Equal(Array<int>.Empty, 123.Range(0, 123).ToArray());

			var a = 123;
			var b = -456;
			b.Swap(ref a);
			Assert.Equal(123, b);

			Assert.Equal(new[] { -2, -1, 0, 1, 2 }, (-2).To(2).ToArray());
			Assert.Equal(new[] { 2 }, 2.To(2, 0).ToArray());
			Assert.Equal(new[] { 9, 6, 3 }, 9.To(3, -3).ToArray());
			Assert.Equal(new[] { 123 }, 123.To(123, 123).ToArray());

			Assert.Equal(new[] { 5, 6, 7 }, new Range(Index.FromStart(5), Index.FromStart(8)).Values().ToArray());
			Assert.Equal(new[] { 7, 6, 5 }, new Range(Index.FromStart(7), Index.FromStart(4)).Values().ToArray());
			Assert.Equal(new[] { 0, 1, 2, 3 }, new Range(Index.FromStart(0), Index.FromStart(4)).Values().ToArray());
			Assert.Equal(new[] { 4, 3, 2, 1 }, new Range(Index.FromStart(4), Index.FromStart(0)).Values().ToArray());
		}
	}
}
