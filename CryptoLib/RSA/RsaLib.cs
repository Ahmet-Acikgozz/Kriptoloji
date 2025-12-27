using System;
using System.Security.Cryptography;
using System.Text;

namespace CryptoLib.RsaCrypto
{
    public class RsaLib : IDisposable
    {
        private System.Security.Cryptography.RSA _rsa;
        private bool _disposed = false;

        public string PublicKey => Convert.ToBase64String(_rsa.ExportRSAPublicKey());

        public string PrivateKey => Convert.ToBase64String(_rsa.ExportRSAPrivateKey());

        public RsaLib(int keySize = 2048)
        {
            _rsa = System.Security.Cryptography.RSA.Create(keySize);
        }

        public RsaLib(string privateKeyBase64)
        {
            _rsa = System.Security.Cryptography.RSA.Create();
            byte[] keyBytes = Convert.FromBase64String(privateKeyBase64);
            _rsa.ImportRSAPrivateKey(keyBytes, out _);
        }

        public string GetPublicKeyXml()
        {
            return _rsa.ToXmlString(false);
        }

        public string GetPrivateKeyXml()
        {
            return _rsa.ToXmlString(true);
        }

        public void ImportPublicKeyXml(string xml)
        {
            _rsa.FromXmlString(xml);
        }

        public string Encrypt(byte[] data)
        {
            byte[] encrypted = _rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
            return Convert.ToBase64String(encrypted);
        }

        public string EncryptString(string plainText)
        {
            byte[] data = Encoding.UTF8.GetBytes(plainText);
            return Encrypt(data);
        }

        public byte[] Decrypt(string cipherText)
        {
            byte[] encrypted = Convert.FromBase64String(cipherText);
            return _rsa.Decrypt(encrypted, RSAEncryptionPadding.OaepSHA256);
        }

        public string DecryptString(string cipherText)
        {
            byte[] decrypted = Decrypt(cipherText);
            return Encoding.UTF8.GetString(decrypted);
        }

        public string EncryptSymmetricKey(byte[] symmetricKey)
        {
            return Encrypt(symmetricKey);
        }

        public byte[] DecryptSymmetricKey(string encryptedKey)
        {
            return Decrypt(encryptedKey);
        }

        public static RsaLib CreateFromPublicKey(string publicKeyBase64)
        {
            var rsa = new RsaLib();
            byte[] keyBytes = Convert.FromBase64String(publicKeyBase64);
            rsa._rsa.ImportRSAPublicKey(keyBytes, out _);
            return rsa;
        }

        public static RsaLib CreateFromPublicKeyXml(string xml)
        {
            var rsa = new RsaLib();
            rsa._rsa.FromXmlString(xml);
            return rsa;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _rsa?.Dispose();
                }
                _disposed = true;
            }
        }
    }

    public static class HybridCrypto
    {
        public static (string encryptedKey, string encryptedMessage) EncryptWithKeyExchange(
            string message, string rsaPublicKeyXml)
        {
            byte[] aesKey = CryptoLib.AES.AesLib.GenerateKey();

            string encryptedMessage = CryptoLib.AES.AesLib.Encrypt(message, aesKey);

            using var rsa = RsaLib.CreateFromPublicKeyXml(rsaPublicKeyXml);
            string encryptedKey = rsa.EncryptSymmetricKey(aesKey);

            return (encryptedKey, encryptedMessage);
        }

        public static string DecryptWithKeyExchange(
            string encryptedKey, string encryptedMessage, RsaLib rsaWithPrivateKey)
        {
            byte[] aesKey = rsaWithPrivateKey.DecryptSymmetricKey(encryptedKey);

            return CryptoLib.AES.AesLib.Decrypt(encryptedMessage, aesKey);
        }
    }
}
