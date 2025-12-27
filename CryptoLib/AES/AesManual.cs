using System;
using System.Text;

namespace CryptoLib.AES
{
    public static class AesManual
    {
        private const int Nb = 4;
        private const int Nk = 4;
        private const int Nr = 10;

        private static byte[]? _sBox;
        private static byte[]? _invSBox;
        private static bool _initialized = false;

        private static readonly byte[] Rcon = new byte[]
        {
            0x00, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x1B, 0x36
        };

        public static byte GFMul(byte a, byte b)
        {
            byte p = 0;
            for (int i = 0; i < 8; i++)
            {
                if ((b & 1) != 0)
                    p ^= a;

                bool hiBitSet = (a & 0x80) != 0;
                a <<= 1;

                if (hiBitSet)
                    a ^= 0x1B;

                b >>= 1;
            }
            return p;
        }

        public static byte GFInverse(byte a)
        {
            if (a == 0) return 0;

            byte result = a;
            for (int i = 0; i < 6; i++)
            {
                result = GFMul(result, result);
                result = GFMul(result, a);
            }
            result = GFMul(result, result);
            return result;
        }

        private static byte AffineTransform(byte b)
        {
            byte result = 0;
            byte c = 0x63;

            for (int i = 0; i < 8; i++)
            {
                int bit = ((b >> i) & 1) ^
                          ((b >> ((i + 4) % 8)) & 1) ^
                          ((b >> ((i + 5) % 8)) & 1) ^
                          ((b >> ((i + 6) % 8)) & 1) ^
                          ((b >> ((i + 7) % 8)) & 1) ^
                          ((c >> i) & 1);

                result |= (byte)(bit << i);
            }
            return result;
        }

        private static byte InverseAffineTransform(byte b)
        {
            byte result = 0;
            byte c = 0x05;

            for (int i = 0; i < 8; i++)
            {
                int bit = ((b >> ((i + 2) % 8)) & 1) ^
                          ((b >> ((i + 5) % 8)) & 1) ^
                          ((b >> ((i + 7) % 8)) & 1) ^
                          ((c >> i) & 1);

                result |= (byte)(bit << i);
            }
            return result;
        }

        public static void InitializeSBoxes()
        {
            if (_initialized) return;

            _sBox = new byte[256];
            _invSBox = new byte[256];

            for (int i = 0; i < 256; i++)
            {
                byte inv = GFInverse((byte)i);
                _sBox[i] = AffineTransform(inv);

                _invSBox[_sBox[i]] = (byte)i;
            }

            _initialized = true;
        }

        public static byte[] GetSBox()
        {
            InitializeSBoxes();
            return (byte[])_sBox!.Clone();
        }

        public static byte[] GetInvSBox()
        {
            InitializeSBoxes();
            return (byte[])_invSBox!.Clone();
        }

        private static void SubBytes(byte[,] state)
        {
            for (int r = 0; r < 4; r++)
                for (int c = 0; c < 4; c++)
                    state[r, c] = _sBox![state[r, c]];
        }

        private static void InvSubBytes(byte[,] state)
        {
            for (int r = 0; r < 4; r++)
                for (int c = 0; c < 4; c++)
                    state[r, c] = _invSBox![state[r, c]];
        }

        private static void ShiftRows(byte[,] state)
        {
            byte temp = state[1, 0];
            state[1, 0] = state[1, 1];
            state[1, 1] = state[1, 2];
            state[1, 2] = state[1, 3];
            state[1, 3] = temp;

            byte temp1 = state[2, 0];
            byte temp2 = state[2, 1];
            state[2, 0] = state[2, 2];
            state[2, 1] = state[2, 3];
            state[2, 2] = temp1;
            state[2, 3] = temp2;

            temp = state[3, 3];
            state[3, 3] = state[3, 2];
            state[3, 2] = state[3, 1];
            state[3, 1] = state[3, 0];
            state[3, 0] = temp;
        }

        private static void InvShiftRows(byte[,] state)
        {
            byte temp = state[1, 3];
            state[1, 3] = state[1, 2];
            state[1, 2] = state[1, 1];
            state[1, 1] = state[1, 0];
            state[1, 0] = temp;

            byte temp1 = state[2, 0];
            byte temp2 = state[2, 1];
            state[2, 0] = state[2, 2];
            state[2, 1] = state[2, 3];
            state[2, 2] = temp1;
            state[2, 3] = temp2;

            temp = state[3, 0];
            state[3, 0] = state[3, 1];
            state[3, 1] = state[3, 2];
            state[3, 2] = state[3, 3];
            state[3, 3] = temp;
        }

        private static void MixColumns(byte[,] state)
        {
            for (int c = 0; c < 4; c++)
            {
                byte s0 = state[0, c];
                byte s1 = state[1, c];
                byte s2 = state[2, c];
                byte s3 = state[3, c];

                state[0, c] = (byte)(GFMul(0x02, s0) ^ GFMul(0x03, s1) ^ s2 ^ s3);
                state[1, c] = (byte)(s0 ^ GFMul(0x02, s1) ^ GFMul(0x03, s2) ^ s3);
                state[2, c] = (byte)(s0 ^ s1 ^ GFMul(0x02, s2) ^ GFMul(0x03, s3));
                state[3, c] = (byte)(GFMul(0x03, s0) ^ s1 ^ s2 ^ GFMul(0x02, s3));
            }
        }

        private static void InvMixColumns(byte[,] state)
        {
            for (int c = 0; c < 4; c++)
            {
                byte s0 = state[0, c];
                byte s1 = state[1, c];
                byte s2 = state[2, c];
                byte s3 = state[3, c];

                state[0, c] = (byte)(GFMul(0x0e, s0) ^ GFMul(0x0b, s1) ^ GFMul(0x0d, s2) ^ GFMul(0x09, s3));
                state[1, c] = (byte)(GFMul(0x09, s0) ^ GFMul(0x0e, s1) ^ GFMul(0x0b, s2) ^ GFMul(0x0d, s3));
                state[2, c] = (byte)(GFMul(0x0d, s0) ^ GFMul(0x09, s1) ^ GFMul(0x0e, s2) ^ GFMul(0x0b, s3));
                state[3, c] = (byte)(GFMul(0x0b, s0) ^ GFMul(0x0d, s1) ^ GFMul(0x09, s2) ^ GFMul(0x0e, s3));
            }
        }

        private static void AddRoundKey(byte[,] state, byte[,] roundKey)
        {
            for (int r = 0; r < 4; r++)
                for (int c = 0; c < 4; c++)
                    state[r, c] ^= roundKey[r, c];
        }

        private static byte[,] KeyExpansion(byte[] key)
        {
            InitializeSBoxes();

            byte[,] w = new byte[4, 4 * (Nr + 1)];

            for (int i = 0; i < Nk; i++)
            {
                w[0, i] = key[4 * i];
                w[1, i] = key[4 * i + 1];
                w[2, i] = key[4 * i + 2];
                w[3, i] = key[4 * i + 3];
            }

            for (int i = Nk; i < 4 * (Nr + 1); i++)
            {
                byte[] temp = new byte[4];
                temp[0] = w[0, i - 1];
                temp[1] = w[1, i - 1];
                temp[2] = w[2, i - 1];
                temp[3] = w[3, i - 1];

                if (i % Nk == 0)
                {
                    byte t = temp[0];
                    temp[0] = temp[1];
                    temp[1] = temp[2];
                    temp[2] = temp[3];
                    temp[3] = t;

                    temp[0] = _sBox![temp[0]];
                    temp[1] = _sBox[temp[1]];
                    temp[2] = _sBox[temp[2]];
                    temp[3] = _sBox[temp[3]];

                    temp[0] ^= Rcon[i / Nk];
                }

                w[0, i] = (byte)(w[0, i - Nk] ^ temp[0]);
                w[1, i] = (byte)(w[1, i - Nk] ^ temp[1]);
                w[2, i] = (byte)(w[2, i - Nk] ^ temp[2]);
                w[3, i] = (byte)(w[3, i - Nk] ^ temp[3]);
            }

            return w;
        }

        private static byte[,] GetRoundKey(byte[,] expandedKey, int round)
        {
            byte[,] roundKey = new byte[4, 4];
            for (int c = 0; c < 4; c++)
            {
                roundKey[0, c] = expandedKey[0, round * 4 + c];
                roundKey[1, c] = expandedKey[1, round * 4 + c];
                roundKey[2, c] = expandedKey[2, round * 4 + c];
                roundKey[3, c] = expandedKey[3, round * 4 + c];
            }
            return roundKey;
        }

        private static byte[] EncryptBlock(byte[] block, byte[,] expandedKey)
        {
            byte[,] state = new byte[4, 4];
            for (int r = 0; r < 4; r++)
                for (int c = 0; c < 4; c++)
                    state[r, c] = block[c * 4 + r];

            AddRoundKey(state, GetRoundKey(expandedKey, 0));

            for (int round = 1; round < Nr; round++)
            {
                SubBytes(state);
                ShiftRows(state);
                MixColumns(state);
                AddRoundKey(state, GetRoundKey(expandedKey, round));
            }

            SubBytes(state);
            ShiftRows(state);
            AddRoundKey(state, GetRoundKey(expandedKey, Nr));

            byte[] output = new byte[16];
            for (int r = 0; r < 4; r++)
                for (int c = 0; c < 4; c++)
                    output[c * 4 + r] = state[r, c];

            return output;
        }

        private static byte[] DecryptBlock(byte[] block, byte[,] expandedKey)
        {
            byte[,] state = new byte[4, 4];
            for (int r = 0; r < 4; r++)
                for (int c = 0; c < 4; c++)
                    state[r, c] = block[c * 4 + r];

            AddRoundKey(state, GetRoundKey(expandedKey, Nr));

            for (int round = Nr - 1; round >= 1; round--)
            {
                InvShiftRows(state);
                InvSubBytes(state);
                AddRoundKey(state, GetRoundKey(expandedKey, round));
                InvMixColumns(state);
            }

            InvShiftRows(state);
            InvSubBytes(state);
            AddRoundKey(state, GetRoundKey(expandedKey, 0));

            byte[] output = new byte[16];
            for (int r = 0; r < 4; r++)
                for (int c = 0; c < 4; c++)
                    output[c * 4 + r] = state[r, c];

            return output;
        }

        public static string Encrypt(string plainText, byte[] key)
        {
            if (key.Length != 16)
                throw new ArgumentException("AES-128 için anahtar 16 byte olmalıdır.");

            InitializeSBoxes();
            byte[,] expandedKey = KeyExpansion(key);

            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

            int paddingLength = 16 - (plainBytes.Length % 16);
            byte[] paddedBytes = new byte[plainBytes.Length + paddingLength];
            Buffer.BlockCopy(plainBytes, 0, paddedBytes, 0, plainBytes.Length);
            for (int i = plainBytes.Length; i < paddedBytes.Length; i++)
                paddedBytes[i] = (byte)paddingLength;

            byte[] encrypted = new byte[paddedBytes.Length];
            for (int i = 0; i < paddedBytes.Length; i += 16)
            {
                byte[] block = new byte[16];
                Buffer.BlockCopy(paddedBytes, i, block, 0, 16);
                byte[] encBlock = EncryptBlock(block, expandedKey);
                Buffer.BlockCopy(encBlock, 0, encrypted, i, 16);
            }

            return Convert.ToBase64String(encrypted);
        }

        public static string Decrypt(string cipherText, byte[] key)
        {
            if (key.Length != 16)
                throw new ArgumentException("AES-128 için anahtar 16 byte olmalıdır.");

            InitializeSBoxes();
            byte[,] expandedKey = KeyExpansion(key);

            byte[] encryptedBytes = Convert.FromBase64String(cipherText);
            byte[] decrypted = new byte[encryptedBytes.Length];

            for (int i = 0; i < encryptedBytes.Length; i += 16)
            {
                byte[] block = new byte[16];
                Buffer.BlockCopy(encryptedBytes, i, block, 0, 16);
                byte[] decBlock = DecryptBlock(block, expandedKey);
                Buffer.BlockCopy(decBlock, 0, decrypted, i, 16);
            }

            int paddingLength = decrypted[decrypted.Length - 1];
            if (paddingLength > 0 && paddingLength <= 16)
            {
                byte[] result = new byte[decrypted.Length - paddingLength];
                Buffer.BlockCopy(decrypted, 0, result, 0, result.Length);
                return Encoding.UTF8.GetString(result);
            }

            return Encoding.UTF8.GetString(decrypted);
        }
    }
}
