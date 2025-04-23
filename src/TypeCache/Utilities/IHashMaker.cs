// Copyright (c) 2021 Samuel Abraham

using System.Security.Cryptography;

namespace TypeCache.Utilities;

public interface IHashMaker : IDisposable
{
	byte[] Decrypt(ReadOnlySpan<byte> data, byte[]? rgbIV = null, PaddingMode paddingMode = PaddingMode.PKCS7);

	byte[] Decrypt(byte[] data, byte[]? rgbIV = null, PaddingMode paddingMode = PaddingMode.PKCS7);

	long Decrypt(ReadOnlySpan<char> hashId, byte[]? rgbIV = null, PaddingMode paddingMode = PaddingMode.PKCS7);

	byte[] Encrypt(ReadOnlySpan<byte> data, byte[]? rgbIV = null, PaddingMode paddingMode = PaddingMode.PKCS7);

	byte[] Encrypt(byte[] data, byte[]? rgbIV = null, PaddingMode paddingMode = PaddingMode.PKCS7);

	ReadOnlySpan<char> Encrypt(long id, byte[]? rgbIV = null, PaddingMode paddingMode = PaddingMode.PKCS7);
}
