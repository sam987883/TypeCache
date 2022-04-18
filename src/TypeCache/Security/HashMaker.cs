// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Security;

public class HashMaker : IHashMaker
{
	private readonly byte[] _RgbKey;
	private readonly byte[] _RgbIV;

	public HashMaker(byte[] rgbKey, byte[] rgbIV)
	{
		this._RgbKey = rgbKey;
		this._RgbIV = rgbIV;
	}

	public HashMaker(decimal rgbKey, decimal rgbIV)
	{
		this._RgbKey = rgbKey.ToBytes();
		this._RgbIV = rgbIV.ToBytes();
	}

	public long Decrypt(string hashId)
		=> Guid.TryParseExact(hashId, "N", out var guid) ? this.Decrypt(guid.ToByteArray()).ToInt64() : 0L;

	public long[] Decrypt(IEnumerable<string> hashIds)
		=> this.Decrypt(hashIds.Map(hashId => Guid.TryParseExact(hashId, "N", out var guid) ? guid.ToByteArray() : Array<byte>.Empty))
			.Map(buffer => buffer.ToInt64()).ToArray();

	public string Encrypt(long id)
		=> new Guid(this.Encrypt(id.ToBytes())).ToString("N");

	public string[] Encrypt(IEnumerable<long> ids)
	{
		ids.AssertNotNull();
		var buffers = this.Encrypt(ids.Map(id => id.ToBytes()).ToArray());
		return buffers.Map(buffer => new Guid(buffer).ToString("N")).ToArray();
	}

	public byte[] Decrypt(byte[] data)
	{
		using var provider = Aes.Create();
		using var decryptor = provider.CreateDecryptor(this._RgbKey, this._RgbIV);
		var result = decryptor.TransformFinalBlock(data, 0, data.Length);
		provider.Clear();
		return result;
	}

	public byte[] Encrypt(byte[] data)
	{
		using var provider = Aes.Create();
		using var encryptor = provider.CreateEncryptor(this._RgbKey, this._RgbIV);
		var result = encryptor.TransformFinalBlock(data, 0, data.Length);
		provider.Clear();
		return result;
	}

	public byte[][] Decrypt(IEnumerable<byte[]> items)
	{
		using var provider = Aes.Create();
		using var decryptor = provider.CreateDecryptor(this._RgbKey, this._RgbIV);
		//var results = await items.Map(async data => await Transform(data, decryptor, cancellationToken)).AllAsync();
		var results = items.Map(data => decryptor.TransformFinalBlock(data, 0, data.Length)).ToArray();
		provider.Clear();
		return results;
	}

	public byte[][] Encrypt(IEnumerable<byte[]> items)
	{
		using var provider = Aes.Create();
		using var encryptor = provider.CreateEncryptor(this._RgbKey, this._RgbIV);
		//var results = await items.Map(async data => await Transform(data, encryptor, cancellationToken)).AllAsync();
		var results = items.Map(data => encryptor.TransformFinalBlock(data, 0, data.Length)).ToArray();
		provider.Clear();
		return results;
	}
}
