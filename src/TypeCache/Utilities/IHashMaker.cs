// Copyright (c) 2021 Samuel Abraham

using System.Security.Cryptography;

namespace TypeCache.Utilities;

public interface IHashMaker : IDisposable
{
	byte[] Decrypt(ReadOnlySpan<byte> data, PaddingMode paddingMode = PaddingMode.PKCS7);

	byte[] Decrypt(byte[] data, PaddingMode paddingMode = PaddingMode.PKCS7);

	long Decrypt(ReadOnlySpan<char> hashId, PaddingMode paddingMode = PaddingMode.PKCS7);

	byte[] Encrypt(ReadOnlySpan<byte> data, PaddingMode paddingMode = PaddingMode.PKCS7);

	byte[] Encrypt(byte[] data, PaddingMode paddingMode = PaddingMode.PKCS7);

	ReadOnlySpan<char> Encrypt(long id, PaddingMode paddingMode = PaddingMode.PKCS7);
}
