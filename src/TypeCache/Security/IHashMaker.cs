// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Security;

public interface IHashMaker : IDisposable
{
	byte[] Decrypt(byte[] data);

	long Decrypt(string hashId);

	byte[] Encrypt(byte[] data);

	string Encrypt(long id);
}
