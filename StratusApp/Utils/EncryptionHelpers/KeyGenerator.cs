﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Utils.EncryptionHelpers
{
    public sealed class KeyGenerator
    {
        public static byte[] GenerateRandomKey(int keySizeInBytes)
        {
            using (var randomNumberGenerator = new RNGCryptoServiceProvider())
            {
                var key = new byte[keySizeInBytes];
                randomNumberGenerator.GetBytes(key);
                return key;
            }
        }

        public static string GetBase64EncodedKey(byte[] key)
        {
            return Convert.ToBase64String(key);
        }

        public static byte[] GetBytesFromBase64EncodedKey(string base64EncodedKey)
        {
            return Convert.FromBase64String(base64EncodedKey);
        }
    }
}