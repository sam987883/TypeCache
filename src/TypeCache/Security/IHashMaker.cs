﻿// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;

namespace TypeCache.Security;

public interface IHashMaker
{
	byte[] Decrypt(byte[] data);

	long Decrypt(string hashId);

	long[] Decrypt(IEnumerable<string> hashIds);

	byte[] Encrypt(byte[] data);

	string Encrypt(long id);

	string[] Encrypt(IEnumerable<long> ids);
}
