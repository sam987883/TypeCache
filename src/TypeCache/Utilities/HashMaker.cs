// Copyright (c) 2021 Samuel Abraham

using System.Security.Cryptography;
using TypeCache.Extensions;

namespace TypeCache.Utilities;

public class HashMaker : IHashMaker
{
	private readonly Aes _SymmetricAlgorithm = Aes.Create();

	/// <exception cref="ArgumentNullException"/>
	public HashMaker(byte[] rgbKey, byte[] rgbIV)
	{
		this._SymmetricAlgorithm.Key = rgbKey;
		this._SymmetricAlgorithm.IV = rgbIV;
	}

	/// <inheritdoc cref="SymmetricAlgorithm.DecryptCbc(ReadOnlySpan{byte}, ReadOnlySpan{byte}, PaddingMode)"/>
	public byte[] Decrypt(ReadOnlySpan<byte> data, PaddingMode paddingMode = PaddingMode.PKCS7)
		=> this._SymmetricAlgorithm
			.DecryptCbc(data, this._SymmetricAlgorithm.IV, paddingMode);

	/// <inheritdoc cref="SymmetricAlgorithm.DecryptCbc(byte[], byte[], PaddingMode)"/>
	public byte[] Decrypt(byte[] data, PaddingMode paddingMode = PaddingMode.PKCS7)
		=> this._SymmetricAlgorithm
			.DecryptCbc(data, this._SymmetricAlgorithm.IV, paddingMode);

	/// <inheritdoc cref="SymmetricAlgorithm.DecryptCbc(byte[], byte[], PaddingMode)"/>
	/// <param name="hashId">A base 64 encoded string.</param>
	public long Decrypt(ReadOnlySpan<char> hashId, PaddingMode paddingMode = PaddingMode.PKCS7)
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
		return this._SymmetricAlgorithm
			.DecryptCbc(data, this._SymmetricAlgorithm.IV, paddingMode)
			.ToInt64();
	}

	public byte[] Encrypt(ReadOnlySpan<byte> data, PaddingMode paddingMode = PaddingMode.PKCS7)
		=> this._SymmetricAlgorithm
			.EncryptCbc(data, this._SymmetricAlgorithm.IV, paddingMode);

	/// <exception cref="ArgumentNullException"/>
	public byte[] Encrypt(byte[] data, PaddingMode paddingMode = PaddingMode.PKCS7)
	{
		data.AssertNotNull();

		return this._SymmetricAlgorithm
			.EncryptCbc(data, this._SymmetricAlgorithm.IV, paddingMode);
	}

	/// <inheritdoc cref="SymmetricAlgorithm.EncryptCbc(byte[], byte[], PaddingMode)"/>
	/// <remarks>Returns a base 64 encoded string.</remarks>
	public ReadOnlySpan<char> Encrypt(long id, PaddingMode paddingMode = PaddingMode.PKCS7)
	{
		var chars = this._SymmetricAlgorithm
			.EncryptCbc(id.GetBytes(), this._SymmetricAlgorithm.IV, paddingMode)
			.ToBase64Chars()
			.AsSpan()
			.Replace('+', '-')
			.Replace('/', '_');
		return chars.Slice(0, chars.Length - 2);
	}

	public void Dispose()
	{
		this._SymmetricAlgorithm.Clear();
		this._SymmetricAlgorithm.Dispose();
	}
}
