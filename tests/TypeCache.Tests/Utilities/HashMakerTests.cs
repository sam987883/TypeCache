// Copyright (c) 2021 Samuel Abraham

using System.Text;
using TypeCache.Extensions;
using TypeCache.Utilities;
using Xunit;

namespace TypeCache.Tests.Utilities;

public class HashMakerTests
{
	private readonly byte[] _Key = Encoding.UTF8.GetBytes("123456ABCDEFghij");

	[Fact]
	public void Encrypt()
	{
		using var hashMaker = new HashMaker(this._Key);

		var rgbIV = Encoding.UTF8.GetBytes("XXXXyyyyZZZZ1234");
		var id = 998999L;
		var hashId = hashMaker.Encrypt(id, rgbIV);

		Assert.Equal("61O-LQ358J9GQOxSZBmEWg", hashId);

		var bytes = hashMaker.Encrypt(id.ToBytes(), rgbIV);

	}
}
