// Copyright (c) 2021 Samuel Abraham

using System.Security.Cryptography;
using TypeCache.Extensions;

namespace TypeCache.Utilities;

public class HashMaker : IHashMaker
{
	private readonly PaddingMode _PaddingMode;
	private readonly Aes _Provider = Aes.Create();

	/// <exception cref="ArgumentNullException"/>
	public HashMaker(byte[] rgbKey, byte[] rgbIV, PaddingMode paddingMode = PaddingMode.PKCS7)
	{
		this._Provider.Key = rgbKey;
		this._Provider.IV = rgbIV;
		this._PaddingMode = paddingMode;
	}

	/// <inheritdoc cref="SymmetricAlgorithm.DecryptCbc(ReadOnlySpan{byte}, ReadOnlySpan{byte}, PaddingMode)"/>
	public byte[] Decrypt(ReadOnlySpan<byte> data)
		=> this._Provider
			.DecryptCbc(data, this._Provider.IV, this._PaddingMode);

	/// <inheritdoc cref="SymmetricAlgorithm.DecryptCbc(byte[], byte[], PaddingMode)"/>
	public byte[] Decrypt(byte[] data)
		=> this._Provider
			.DecryptCbc(data, this._Provider.IV, this._PaddingMode);

	/// <inheritdoc cref="SymmetricAlgorithm.DecryptCbc(byte[], byte[], PaddingMode)"/>
	/// <param name="hashId">A base 64 encoded string.</param>
	public long Decrypt(ReadOnlySpan<char> hashId)
	{
		var length = (hashId[^2], hashId[^1]) switch
		{
			('=', '=') => hashId.Length,
			_ => hashId.Length + 2
		};
		Span<char> span = stackalloc char[length];
		hashId.CopyTo(span);
		span[^1] = '=';
		span[^2] = '=';

		var data = span
			.Replace('-', '+')
			.Replace('_', '/')
			.ToArray()
			.FromBase64();
		return this._Provider
			.DecryptCbc(data, this._Provider.IV, this._PaddingMode)
			.ToInt64();
	}

	public byte[] Encrypt(ReadOnlySpan<byte> data)
		=> this._Provider
			.EncryptCbc(data, this._Provider.IV, this._PaddingMode);

	/// <exception cref="ArgumentNullException"/>
	public byte[] Encrypt(byte[] data)
	{
		data.AssertNotNull();

		return this._Provider
			.EncryptCbc(data, this._Provider.IV, this._PaddingMode);
	}

	/// <inheritdoc cref="SymmetricAlgorithm.EncryptCbc(byte[], byte[], PaddingMode)"/>
	/// <remarks>Returns a base 64 encoded string.</remarks>
	public ReadOnlySpan<char> Encrypt(long id)
	{
		var data = id.GetBytes();
		var chars = this._Provider
			.EncryptCbc(data, this._Provider.IV, this._PaddingMode)
			.ToBase64Chars()
			.AsSpan()
			.Replace('+', '-')
			.Replace('/', '_');
		return chars.Slice(0, chars.Length - 2);
	}

	public void Dispose()
	{
		this._Provider.Clear();
		this._Provider.Dispose();
	}
}
