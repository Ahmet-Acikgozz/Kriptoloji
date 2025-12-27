using System;
using System.Security.Cryptography;
using System.Text;

namespace CryptoLib.ECC
{
    public class EccLib : IDisposable
    {
        private ECDiffieHellman _ecdh;
        private ECDsa _ecdsa;
        private bool _disposed = false;

        public string PublicKey => Convert.ToBase64String(_ecdh.PublicKey.ExportSubjectPublicKeyInfo());

        public string SigningPublicKey => Convert.ToBase64String(_ecdsa.ExportSubjectPublicKeyInfo());

        public EccLib(ECCurve? curve = null)
        {
            var selectedCurve = curve ?? ECCurve.NamedCurves.nistP256;
            _ecdh = ECDiffieHellman.Create(selectedCurve);
            _ecdsa = ECDsa.Create(selectedCurve);
        }

        public EccLib(string ecdhPrivateKeyBase64, string ecdsaPrivateKeyBase64)
        {
            _ecdh = ECDiffieHellman.Create();
            _ecdh.ImportPkcs8PrivateKey(Convert.FromBase64String(ecdhPrivateKeyBase64), out _);

            _ecdsa = ECDsa.Create();
            _ecdsa.ImportPkcs8PrivateKey(Convert.FromBase64String(ecdsaPrivateKeyBase64), out _);
        }

        public string GetPrivateKey()
        {
            return Convert.ToBase64String(_ecdh.ExportPkcs8PrivateKey());
        }

        public string GetSigningPrivateKey()
        {
            return Convert.ToBase64String(_ecdsa.ExportPkcs8PrivateKey());
        }

        public byte[] DeriveSharedSecret(string otherPublicKeyBase64)
        {
            byte[] otherKeyBytes = Convert.FromBase64String(otherPublicKeyBase64);
            
            using var otherKey = ECDiffieHellman.Create();
            otherKey.ImportSubjectPublicKeyInfo(otherKeyBytes, out _);

            return _ecdh.DeriveKeyMaterial(otherKey.PublicKey);
        }

        public string Encrypt(string plainText, string recipientPublicKeyBase64)
        {
            byte[] sharedSecret = DeriveSharedSecret(recipientPublicKeyBase64);
            
            byte[] aesKey = new byte[16];
            Array.Copy(sharedSecret, aesKey, Math.Min(16, sharedSecret.Length));

            return CryptoLib.AES.AesLib.Encrypt(plainText, aesKey);
        }

        public string Decrypt(string cipherText, string senderPublicKeyBase64)
        {
            byte[] sharedSecret = DeriveSharedSecret(senderPublicKeyBase64);
            
            byte[] aesKey = new byte[16];
            Array.Copy(sharedSecret, aesKey, Math.Min(16, sharedSecret.Length));

            return CryptoLib.AES.AesLib.Decrypt(cipherText, aesKey);
        }

        public string Sign(byte[] data)
        {
            byte[] signature = _ecdsa.SignData(data, HashAlgorithmName.SHA256);
            return Convert.ToBase64String(signature);
        }

        public string SignString(string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            return Sign(data);
        }

        public bool Verify(byte[] data, string signatureBase64)
        {
            byte[] signature = Convert.FromBase64String(signatureBase64);
            return _ecdsa.VerifyData(data, signature, HashAlgorithmName.SHA256);
        }

        public bool VerifyString(string text, string signatureBase64)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            return Verify(data, signatureBase64);
        }

        public static EccLib CreateFromPublicKey(string ecdhPublicKeyBase64)
        {
            var ecc = new EccLib();
            
            using var tempEcdh = ECDiffieHellman.Create();
            tempEcdh.ImportSubjectPublicKeyInfo(Convert.FromBase64String(ecdhPublicKeyBase64), out _);
            
            return ecc;
        }

        public (string encryptedKey, string senderPublicKey) EncryptSymmetricKey(byte[] symmetricKey, string recipientPublicKeyBase64)
        {
            byte[] sharedSecret = DeriveSharedSecret(recipientPublicKeyBase64);
            
            byte[] encrypted = new byte[symmetricKey.Length];
            for (int i = 0; i < symmetricKey.Length; i++)
            {
                encrypted[i] = (byte)(symmetricKey[i] ^ sharedSecret[i % sharedSecret.Length]);
            }
            
            return (Convert.ToBase64String(encrypted), PublicKey);
        }

        public byte[] DecryptSymmetricKey(string encryptedKeyBase64, string senderPublicKeyBase64)
        {
            byte[] encryptedKey = Convert.FromBase64String(encryptedKeyBase64);
            byte[] sharedSecret = DeriveSharedSecret(senderPublicKeyBase64);
            
            byte[] decrypted = new byte[encryptedKey.Length];
            for (int i = 0; i < encryptedKey.Length; i++)
            {
                decrypted[i] = (byte)(encryptedKey[i] ^ sharedSecret[i % sharedSecret.Length]);
            }
            
            return decrypted;
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
                    _ecdh?.Dispose();
                    _ecdsa?.Dispose();
                }
                _disposed = true;
            }
        }
    }

    public static class EccHybridCrypto
    {
        public static (string encryptedMessage, string senderPublicKey) EncryptWithKeyExchange(
            string message, string recipientPublicKeyBase64)
        {
            using var senderEcc = new EccLib();
            
            byte[] sharedSecret = senderEcc.DeriveSharedSecret(recipientPublicKeyBase64);
            byte[] aesKey = new byte[16];
            Array.Copy(sharedSecret, aesKey, Math.Min(16, sharedSecret.Length));

            string encryptedMessage = CryptoLib.AES.AesLib.Encrypt(message, aesKey);

            return (encryptedMessage, senderEcc.PublicKey);
        }

        public static string DecryptWithKeyExchange(
            string encryptedMessage, string senderPublicKeyBase64, EccLib recipientEcc)
        {
            byte[] sharedSecret = recipientEcc.DeriveSharedSecret(senderPublicKeyBase64);
            byte[] aesKey = new byte[16];
            Array.Copy(sharedSecret, aesKey, Math.Min(16, sharedSecret.Length));

            return CryptoLib.AES.AesLib.Decrypt(encryptedMessage, aesKey);
        }
    }
}
