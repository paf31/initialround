using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace InitialRound.Common.Helpers
{
    public static class EncryptionHelper
    {
        public static byte[] EncryptBytes(byte[] bytes, string key, string vector)
        {
            byte[] keyBytes = Convert.FromBase64String(key);
            byte[] vectorBytes = Convert.FromBase64String(vector);

            using (RijndaelManaged rijndael = new RijndaelManaged())
            {
                rijndael.KeySize = 256;
                rijndael.BlockSize = 128;
                rijndael.Mode = CipherMode.CBC;
                rijndael.Padding = PaddingMode.ISO10126;

                return rijndael.CreateEncryptor(keyBytes, vectorBytes).TransformFinalBlock(bytes, 0, bytes.Length);
            }
        }

        public static string EncryptString(string text, string key, string vector)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);

            return Convert.ToBase64String(EncryptBytes(bytes, key, vector));
        }

        public static byte[] DecryptBytes(byte[] encryptedBytes, string key, string vector)
        {
            byte[] keyBytes = Convert.FromBase64String(key);
            byte[] vectorBytes = Convert.FromBase64String(vector);

            using (RijndaelManaged rijndael = new RijndaelManaged())
            {
                rijndael.KeySize = 256;
                rijndael.BlockSize = 128;
                rijndael.Mode = CipherMode.CBC;
                rijndael.Padding = PaddingMode.ISO10126;

                return rijndael.CreateDecryptor(keyBytes, vectorBytes).TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            }
        }

        public static string DecryptString(string encryptedText, string key, string vector)
        {
            byte[] bytes = Convert.FromBase64String(encryptedText);

            return Encoding.UTF8.GetString(DecryptBytes(bytes, key, vector));
        }
    }
}
