using System;
using System.Security.Cryptography;
using System.Text;

namespace CryptoLib.DES
{
    public static class DesLib
    {
        public static string Encrypt(string plainText, byte[] key, byte[]? iv = null)
        {
            if (key.Length != 8)
                throw new ArgumentException("DES için anahtar 8 byte olmalıdır.");

#pragma warning disable SYSLIB0021
            using var des = System.Security.Cryptography.DES.Create();
#pragma warning restore SYSLIB0021

            des.Key = key;
            des.Mode = CipherMode.CBC;
            des.Padding = PaddingMode.PKCS7;

            if (iv == null)
            {
                des.GenerateIV();
                iv = des.IV;
            }
            else
            {
                des.IV = iv;
            }

            using var encryptor = des.CreateEncryptor();
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            byte[] result = new byte[iv.Length + encryptedBytes.Length];
            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
            Buffer.BlockCopy(encryptedBytes, 0, result, iv.Length, encryptedBytes.Length);

            return Convert.ToBase64String(result);
        }

        public static string Decrypt(string cipherText, byte[] key)
        {
            if (key.Length != 8)
                throw new ArgumentException("DES için anahtar 8 byte olmalıdır.");

            byte[] fullCipher = Convert.FromBase64String(cipherText);

            byte[] iv = new byte[8];
            byte[] cipher = new byte[fullCipher.Length - 8];
            Buffer.BlockCopy(fullCipher, 0, iv, 0, 8);
            Buffer.BlockCopy(fullCipher, 8, cipher, 0, cipher.Length);

#pragma warning disable SYSLIB0021
            using var des = System.Security.Cryptography.DES.Create();
#pragma warning restore SYSLIB0021

            des.Key = key;
            des.IV = iv;
            des.Mode = CipherMode.CBC;
            des.Padding = PaddingMode.PKCS7;

            using var decryptor = des.CreateDecryptor();
            byte[] decryptedBytes = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);

            return Encoding.UTF8.GetString(decryptedBytes);
        }

        public static byte[] GenerateKey()
        {
            byte[] key = new byte[8];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(key);
            for (int i = 0; i < 8; i++)
            {
                int parity = 0;
                for (int j = 1; j < 8; j++)
                    parity ^= (key[i] >> j) & 1;
                key[i] = (byte)((key[i] & 0xFE) | (parity ^ 1));
            }
            return key;
        }

        public static byte[] DeriveKey(string password)
        {
            using var sha = SHA256.Create();
            byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            byte[] key = new byte[8];
            Buffer.BlockCopy(hash, 0, key, 0, 8);
            return key;
        }
    }
}
