using System;
using System.Numerics;
using System.Text;

namespace CryptoLib.ECC
{
    public class EccManual
    {
        private static readonly BigInteger P = BigInteger.Parse("115792089237316195423570985008687907853269984665640564039457584007908834671663");
        private static readonly BigInteger A = BigInteger.Zero;
        private static readonly BigInteger B = BigInteger.Parse("7");
        
        private static readonly BigInteger Gx = BigInteger.Parse("55066263022277343669578718895168534326250603453777594175500187360389116729240");
        private static readonly BigInteger Gy = BigInteger.Parse("32670510020758816978083085130507043184471273380659243275938904335757337482424");
        
        private static readonly BigInteger N = BigInteger.Parse("115792089237316195423570985008687907852837564279074904382605163141518161494337");

        public class ECPoint
        {
            public BigInteger X { get; set; }
            public BigInteger Y { get; set; }
            public bool IsInfinity { get; set; }

            public ECPoint(BigInteger x, BigInteger y)
            {
                X = x;
                Y = y;
                IsInfinity = false;
            }

            public static ECPoint Infinity => new ECPoint(0, 0) { IsInfinity = true };

            public override string ToString()
            {
                if (IsInfinity) return "O (Infinity)";
                return $"({X}, {Y})";
            }

            public string ToBase64()
            {
                if (IsInfinity) return "INFINITY";
                byte[] xBytes = X.ToByteArray();
                byte[] yBytes = Y.ToByteArray();
                byte[] combined = new byte[xBytes.Length + yBytes.Length + 8];
                BitConverter.GetBytes(xBytes.Length).CopyTo(combined, 0);
                BitConverter.GetBytes(yBytes.Length).CopyTo(combined, 4);
                xBytes.CopyTo(combined, 8);
                yBytes.CopyTo(combined, 8 + xBytes.Length);
                return Convert.ToBase64String(combined);
            }

            public static ECPoint FromBase64(string base64)
            {
                if (base64 == "INFINITY") return Infinity;
                byte[] data = Convert.FromBase64String(base64);
                int xLen = BitConverter.ToInt32(data, 0);
                int yLen = BitConverter.ToInt32(data, 4);
                byte[] xBytes = new byte[xLen];
                byte[] yBytes = new byte[yLen];
                Array.Copy(data, 8, xBytes, 0, xLen);
                Array.Copy(data, 8 + xLen, yBytes, 0, yLen);
                return new ECPoint(new BigInteger(xBytes), new BigInteger(yBytes));
            }
        }

        public BigInteger PrivateKey { get; private set; }

        public ECPoint PublicKey { get; private set; }

        public static ECPoint G => new ECPoint(Gx, Gy);

        public EccManual()
        {
            var random = new Random();
            byte[] bytes = new byte[32];
            random.NextBytes(bytes);
            PrivateKey = new BigInteger(bytes);
            PrivateKey = BigInteger.Remainder(BigInteger.Abs(PrivateKey), N - 2) + 1;

            PublicKey = ScalarMultiply(G, PrivateKey);
        }

        public EccManual(BigInteger privateKey)
        {
            PrivateKey = privateKey;
            PublicKey = ScalarMultiply(G, PrivateKey);
        }

        private static BigInteger ModInverse(BigInteger a, BigInteger m)
        {
            if (a < 0) a = ((a % m) + m) % m;
            
            BigInteger g = BigInteger.GreatestCommonDivisor(a, m);
            if (g != 1) throw new Exception("Modüler ters mevcut değil");

            return BigInteger.ModPow(a, m - 2, m);
        }

        public static ECPoint PointAdd(ECPoint p, ECPoint q)
        {
            if (p.IsInfinity) return q;
            if (q.IsInfinity) return p;

            if (p.X == q.X && (p.Y + q.Y) % P == 0)
                return ECPoint.Infinity;

            BigInteger lambda;
            if (p.X == q.X && p.Y == q.Y)
            {
                BigInteger numerator = (3 * BigInteger.ModPow(p.X, 2, P) + A) % P;
                BigInteger denominator = (2 * p.Y) % P;
                lambda = (numerator * ModInverse(denominator, P)) % P;
            }
            else
            {
                BigInteger numerator = (q.Y - p.Y) % P;
                BigInteger denominator = (q.X - p.X) % P;
                if (numerator < 0) numerator += P;
                if (denominator < 0) denominator += P;
                lambda = (numerator * ModInverse(denominator, P)) % P;
            }

            if (lambda < 0) lambda += P;

            BigInteger x3 = (BigInteger.ModPow(lambda, 2, P) - p.X - q.X) % P;
            if (x3 < 0) x3 += P;

            BigInteger y3 = (lambda * (p.X - x3) - p.Y) % P;
            if (y3 < 0) y3 += P;

            return new ECPoint(x3, y3);
        }

        public static ECPoint ScalarMultiply(ECPoint p, BigInteger k)
        {
            if (k == 0 || p.IsInfinity) return ECPoint.Infinity;
            if (k < 0)
            {
                k = -k;
                p = new ECPoint(p.X, (-p.Y % P + P) % P);
            }

            ECPoint result = ECPoint.Infinity;
            ECPoint addend = new ECPoint(p.X, p.Y);

            while (k > 0)
            {
                if ((k & 1) == 1)
                {
                    result = PointAdd(result, addend);
                }
                addend = PointAdd(addend, addend);
                k >>= 1;
            }

            return result;
        }

        public ECPoint DeriveSharedSecret(ECPoint otherPublicKey)
        {
            return ScalarMultiply(otherPublicKey, PrivateKey);
        }

        public ECPoint DeriveSharedSecret(string otherPublicKeyBase64)
        {
            ECPoint otherPublicKey = ECPoint.FromBase64(otherPublicKeyBase64);
            return DeriveSharedSecret(otherPublicKey);
        }

        public string GetPublicKeyBase64()
        {
            return PublicKey.ToBase64();
        }

        public string GetPrivateKeyBase64()
        {
            return Convert.ToBase64String(PrivateKey.ToByteArray());
        }

        public static string Encrypt(string plainText, string recipientPublicKeyBase64)
        {
            var ephemeral = new EccManual();
            
            ECPoint sharedSecret = ephemeral.DeriveSharedSecret(recipientPublicKeyBase64);
            
            byte[] keyMaterial = sharedSecret.X.ToByteArray();
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] cipherBytes = new byte[plainBytes.Length];
            
            for (int i = 0; i < plainBytes.Length; i++)
            {
                cipherBytes[i] = (byte)(plainBytes[i] ^ keyMaterial[i % keyMaterial.Length]);
            }
            
            string ephemeralPubKey = ephemeral.GetPublicKeyBase64();
            string cipherText = Convert.ToBase64String(cipherBytes);
            
            return $"{ephemeralPubKey}|{cipherText}";
        }

        public string Decrypt(string cipherTextWithKey)
        {
            string[] parts = cipherTextWithKey.Split('|');
            if (parts.Length != 2)
                throw new Exception("Geçersiz şifreli metin formatı");
            
            string ephemeralPubKeyBase64 = parts[0];
            string cipherTextBase64 = parts[1];
            
            ECPoint sharedSecret = DeriveSharedSecret(ephemeralPubKeyBase64);
            
            byte[] keyMaterial = sharedSecret.X.ToByteArray();
            byte[] cipherBytes = Convert.FromBase64String(cipherTextBase64);
            byte[] plainBytes = new byte[cipherBytes.Length];
            
            for (int i = 0; i < cipherBytes.Length; i++)
            {
                plainBytes[i] = (byte)(cipherBytes[i] ^ keyMaterial[i % keyMaterial.Length]);
            }
            
            return Encoding.UTF8.GetString(plainBytes);
        }

        public string Sign(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            
            BigInteger messageHash = 0;
            foreach (byte b in messageBytes)
            {
                messageHash = (messageHash * 256 + b) % N;
            }
            
            var random = new Random();
            byte[] kBytes = new byte[32];
            random.NextBytes(kBytes);
            BigInteger k = BigInteger.Remainder(BigInteger.Abs(new BigInteger(kBytes)), N - 2) + 1;
            
            ECPoint R = ScalarMultiply(G, k);
            BigInteger r = R.X % N;
            
            BigInteger kInv = ModInverse(k, N);
            BigInteger s = (kInv * (messageHash + r * PrivateKey)) % N;
            if (s < 0) s += N;
            
            return $"{r}|{s}";
        }

        public static bool Verify(string message, string signature, ECPoint publicKey)
        {
            string[] parts = signature.Split('|');
            if (parts.Length != 2) return false;
            
            BigInteger r = BigInteger.Parse(parts[0]);
            BigInteger s = BigInteger.Parse(parts[1]);
            
            if (r <= 0 || r >= N || s <= 0 || s >= N) return false;
            
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            BigInteger messageHash = 0;
            foreach (byte b in messageBytes)
            {
                messageHash = (messageHash * 256 + b) % N;
            }
            
            BigInteger w = ModInverse(s, N);
            
            BigInteger u1 = (messageHash * w) % N;
            
            BigInteger u2 = (r * w) % N;
            
            ECPoint point1 = ScalarMultiply(G, u1);
            ECPoint point2 = ScalarMultiply(publicKey, u2);
            ECPoint PResult = PointAdd(point1, point2);
            
            if (PResult.IsInfinity) return false;
            
            return PResult.X % N == r;
        }
    }
}
