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
	/// <exception cref="FormatException"/>
	public long Decrypt(ReadOnlySpan<char> hashId, PaddingMode paddingMode = PaddingMode.PKCS7)
		=> this._SymmetricAlgorithm
			.DecryptCbc(Base64Url.DecodeFromChars(hashId), this._SymmetricAlgorithm.IV, paddingMode)
			.ToInt64();

	public byte[] Encrypt(ReadOnlySpan<byte> data, PaddingMode paddingMode = PaddingMode.PKCS7)
		=> this._SymmetricAlgorithm
			.EncryptCbc(data, this._SymmetricAlgorithm.IV, paddingMode);

	/// <exception cref="ArgumentNullException"/>
	public byte[] Encrypt(byte[] data, PaddingMode paddingMode = PaddingMode.PKCS7)
	{
		data.ThrowIfNull();

		return this._SymmetricAlgorithm
			.EncryptCbc(data, this._SymmetricAlgorithm.IV, paddingMode);
	}

	/// <inheritdoc cref="SymmetricAlgorithm.EncryptCbc(byte[], byte[], PaddingMode)"/>
	/// <remarks>Returns a base 64 encoded string.</remarks>
	public ReadOnlySpan<char> Encrypt(long id, PaddingMode paddingMode = PaddingMode.PKCS7)
		=> this._SymmetricAlgorithm
			.EncryptCbc(id.GetBytes(), this._SymmetricAlgorithm.IV, paddingMode)
			.ToBase64UrlChars()
			.AsSpan();

	public void Dispose()
	{
		this._SymmetricAlgorithm.Clear();
		this._SymmetricAlgorithm.Dispose();
	}
}
