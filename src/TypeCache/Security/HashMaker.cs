// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Security
{
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public long[]? Decrypt(string[] hashIds)
			=> hashIds?.To(this.Decrypt).ToArray(hashIds.Length);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public long[]? Decrypt(IEnumerable<string> hashIds)
			=> hashIds?.To(this.Decrypt).ToArray();

		public string Encrypt(long id)
			=> new Guid(this.Encrypt(id.ToBytes())).ToString("N");

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string? Encrypt(long? id)
			=> id.HasValue ? this.Encrypt(id.Value) : null;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string[]? Encrypt(long[] ids)
			=> ids?.To(this.Encrypt).ToArray(ids.Length);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string[]? Encrypt(IEnumerable<long> ids)
			=> ids?.To(this.Encrypt).ToArray();

		public byte[] Decrypt(byte[] data)
		{
			using var aes = Aes.Create();
			using var decryptor = aes.CreateDecryptor(this._RgbKey, this._RgbIV);
			return Transform(data, decryptor);
		}

		public byte[] Encrypt(byte[] data)
		{
			using var aes = Aes.Create();
			using var encryptor = aes.CreateEncryptor(this._RgbKey, this._RgbIV);
			return Transform(data, encryptor);
		}

		private static byte[] Transform(byte[] data, ICryptoTransform transform)
		{
			using var memoryStream = new MemoryStream();
			using var cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);

			cryptoStream.Write(data, 0, data.Length);
			cryptoStream.FlushFinalBlock();

			memoryStream.Position = 0;
			var result = new byte[memoryStream.Length];
			memoryStream.Read(result, 0, result.Length);

			cryptoStream.Close();
			memoryStream.Close();

			return result;
		}
	}
}
