// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Utilities;

public interface IHashMaker : IDisposable
{
	byte[] Decrypt(ReadOnlySpan<byte> data);

	byte[] Decrypt(byte[] data);

	long Decrypt(ReadOnlySpan<char> hashId);

	byte[] Encrypt(ReadOnlySpan<byte> data);

	byte[] Encrypt(byte[] data);

	ReadOnlySpan<char> Encrypt(long id);
}
