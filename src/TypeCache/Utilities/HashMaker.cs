// Copyright (c) 2021 Samuel Abraham

using System.Security.Cryptography;
using TypeCache.Extensions;

namespace TypeCache.Utilities;

public class HashMaker : IHashMaker
{
	private readonly Aes _Provider;

	public HashMaker(byte[] rgbKey, byte[] rgbIV)
	{
		this._Provider = Aes.Create();
		this._Provider.Key = rgbKey;
		this._Provider.IV = rgbIV;
	}

	public HashMaker(decimal rgbKey, decimal rgbIV)
		: this(rgbKey.ToBytes(), rgbIV.ToBytes())
	{
	}

	public byte[] Decrypt(byte[] data)
	{
		data.AssertNotNull();
		return this._Provider.DecryptCbc(data, this._Provider.IV);
	}

	public long Decrypt(string hashId)
	{
		var hasPadding = hashId[^1] == '=' && hashId[^2] == '=';
		var length = hashId.Length;
		if (!hasPadding)
			length += 2;
		Span<char> span = stackalloc char[length];
		hashId.CopyTo(span);
		if (!hasPadding)
		{
			span[^1] = '=';
			span[^2] = '=';
		}
		var bytes = new string(span.Replace('-', '+').Replace('_', '/').ToArray()).FromBase64();
		return this.Decrypt(bytes).ToInt64();
	}

	public byte[] Encrypt(byte[] data)
	{
		data.AssertNotNull();
		return this._Provider.EncryptCbc(data, this._Provider.IV);
	}

	public string Encrypt(long id)
	{
		var chars = this.Encrypt(id.GetBytes()).ToBase64Chars().AsSpan().Replace('+', '-').Replace('/', '_');
		return new string(chars.Slice(0, chars.Length - 2).ToArray());
	}

	public void Dispose()
	{
		this._Provider.Clear();
		this._Provider.Dispose();
	}
}
