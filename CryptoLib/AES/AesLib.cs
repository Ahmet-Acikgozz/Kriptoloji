using System;
using System.Security.Cryptography;
using System.Text;

namespace CryptoLib.AES
{
    public static class AesLib
    {
        public static string Encrypt(string plainText, byte[] key, byte[]? iv = null)
        {
            if (key.Length != 16)
                throw new ArgumentException("AES-128 için anahtar 16 byte olmalıdır.");

            using var aes = System.Security.Cryptography.Aes.Create();
            aes.Key = key;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            if (iv == null)
            {
                aes.GenerateIV();
                iv = aes.IV;
            }
            else
            {
                aes.IV = iv;
            }

            using var encryptor = aes.CreateEncryptor();
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            byte[] result = new byte[iv.Length + encryptedBytes.Length];
            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
            Buffer.BlockCopy(encryptedBytes, 0, result, iv.Length, encryptedBytes.Length);

            return Convert.ToBase64String(result);
        }

        public static string Decrypt(string cipherText, byte[] key)
        {
            if (key.Length != 16)
                throw new ArgumentException("AES-128 için anahtar 16 byte olmalıdır.");

            byte[] fullCipher = Convert.FromBase64String(cipherText);

            byte[] iv = new byte[16];
            byte[] cipher = new byte[fullCipher.Length - 16];
            Buffer.BlockCopy(fullCipher, 0, iv, 0, 16);
            Buffer.BlockCopy(fullCipher, 16, cipher, 0, cipher.Length);

            using var aes = System.Security.Cryptography.Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var decryptor = aes.CreateDecryptor();
            byte[] decryptedBytes = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);

            return Encoding.UTF8.GetString(decryptedBytes);
        }

        public static byte[] GenerateKey()
        {
            byte[] key = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(key);
            return key;
        }

        public static byte[] DeriveKey(string password)
        {
            using var sha = SHA256.Create();
            byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            byte[] key = new byte[16];
            Buffer.BlockCopy(hash, 0, key, 0, 16);
            return key;
        }
    }
}
