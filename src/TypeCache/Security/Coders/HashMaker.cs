// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using TypeCache.Extensions;

namespace TypeCache.Security.Coders
{
	internal class HashMaker : IHashMaker
    {
        private readonly byte[] _RgbKey;
        private readonly byte[] _RgbIV;
        //private static readonly byte[] rgbKey = new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53 };
        //private static readonly byte[] rgbIV = new byte[] { 53, 47, 43, 41, 37, 31, 29, 23, 19, 17, 13, 11, 7, 5, 3, 2 };

        public HashMaker(byte[] rgbKey, byte[] rgbIV)
        {
            this._RgbKey = rgbKey;
            this._RgbIV = rgbIV;
        }

        public HashMaker(decimal rgbKey, decimal rgbIV) : this(rgbKey.ToBytes(), rgbIV.ToBytes())
        {
        }

        public long Decrypt(string hashId)
        {
            if (Guid.TryParseExact(hashId, "N", out var guid))
            {
                var result = this.Decrypt(guid.ToByteArray());
                var id = BitConverter.ToInt64(result);
                return id;
            }
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long[]? Decrypt(string[] hashIds)
            => hashIds?.To(this.Decrypt).ToArrayOf(hashIds.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long[]? Decrypt(IEnumerable<string> hashIds)
            => hashIds?.To(this.Decrypt).ToArray();

        public string Encrypt(long id)
        {
            var data = BitConverter.GetBytes(id);
            var result = this.Encrypt(data);
            return new Guid(result).ToString("N");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string? Encrypt(long? id)
            => id.HasValue ? this.Encrypt(id.Value) : null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string[]? Encrypt(long[] ids)
            => ids?.To(this.Encrypt).ToArrayOf(ids.Length);

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
