using System;
using System.Text;

namespace CryptoLib.DES
{
    public static class DesManual
    {
        private static readonly int[] IP = {
            58, 50, 42, 34, 26, 18, 10, 2,
            60, 52, 44, 36, 28, 20, 12, 4,
            62, 54, 46, 38, 30, 22, 14, 6,
            64, 56, 48, 40, 32, 24, 16, 8,
            57, 49, 41, 33, 25, 17,  9, 1,
            59, 51, 43, 35, 27, 19, 11, 3,
            61, 53, 45, 37, 29, 21, 13, 5,
            63, 55, 47, 39, 31, 23, 15, 7
        };

        private static readonly int[] FP = {
            40, 8, 48, 16, 56, 24, 64, 32,
            39, 7, 47, 15, 55, 23, 63, 31,
            38, 6, 46, 14, 54, 22, 62, 30,
            37, 5, 45, 13, 53, 21, 61, 29,
            36, 4, 44, 12, 52, 20, 60, 28,
            35, 3, 43, 11, 51, 19, 59, 27,
            34, 2, 42, 10, 50, 18, 58, 26,
            33, 1, 41,  9, 49, 17, 57, 25
        };

        private static readonly int[] E = {
            32,  1,  2,  3,  4,  5,
             4,  5,  6,  7,  8,  9,
             8,  9, 10, 11, 12, 13,
            12, 13, 14, 15, 16, 17,
            16, 17, 18, 19, 20, 21,
            20, 21, 22, 23, 24, 25,
            24, 25, 26, 27, 28, 29,
            28, 29, 30, 31, 32,  1
        };

        private static readonly int[] P = {
            16,  7, 20, 21,
            29, 12, 28, 17,
             1, 15, 23, 26,
             5, 18, 31, 10,
             2,  8, 24, 14,
            32, 27,  3,  9,
            19, 13, 30,  6,
            22, 11,  4, 25
        };

        private static readonly int[] PC1 = {
            57, 49, 41, 33, 25, 17,  9,
             1, 58, 50, 42, 34, 26, 18,
            10,  2, 59, 51, 43, 35, 27,
            19, 11,  3, 60, 52, 44, 36,
            63, 55, 47, 39, 31, 23, 15,
             7, 62, 54, 46, 38, 30, 22,
            14,  6, 61, 53, 45, 37, 29,
            21, 13,  5, 28, 20, 12,  4
        };

        private static readonly int[] PC2 = {
            14, 17, 11, 24,  1,  5,
             3, 28, 15,  6, 21, 10,
            23, 19, 12,  4, 26,  8,
            16,  7, 27, 20, 13,  2,
            41, 52, 31, 37, 47, 55,
            30, 40, 51, 45, 33, 48,
            44, 49, 39, 56, 34, 53,
            46, 42, 50, 36, 29, 32
        };

        private static readonly int[] LeftShifts = {
            1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1
        };

        private static readonly int[,,] SBox = {
            {
                {14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7},
                {0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8},
                {4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0},
                {15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13}
            },
            {
                {15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10},
                {3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5},
                {0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15},
                {13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9}
            },
            {
                {10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8},
                {13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1},
                {13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7},
                {1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12}
            },
            {
                {7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15},
                {13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9},
                {10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4},
                {3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14}
            },
            {
                {2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9},
                {14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6},
                {4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14},
                {11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3}
            },
            {
                {12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11},
                {10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8},
                {9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6},
                {4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13}
            },
            {
                {4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1},
                {13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6},
                {1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2},
                {6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2, 3, 12}
            },
            {
                {13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7},
                {1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2},
                {7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 3, 5, 8},
                {2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5, 6, 11}
            }
        };

        private static int[] BytesToBits(byte[] bytes)
        {
            int[] bits = new int[bytes.Length * 8];
            for (int i = 0; i < bytes.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    bits[i * 8 + j] = (bytes[i] >> (7 - j)) & 1;
                }
            }
            return bits;
        }

        private static byte[] BitsToBytes(int[] bits)
        {
            byte[] bytes = new byte[bits.Length / 8];
            for (int i = 0; i < bytes.Length; i++)
            {
                byte b = 0;
                for (int j = 0; j < 8; j++)
                {
                    b = (byte)((b << 1) | bits[i * 8 + j]);
                }
                bytes[i] = b;
            }
            return bytes;
        }

        private static int[] Permute(int[] input, int[] table)
        {
            int[] output = new int[table.Length];
            for (int i = 0; i < table.Length; i++)
            {
                output[i] = input[table[i] - 1];
            }
            return output;
        }

        private static int[] LeftShift(int[] bits, int count)
        {
            int[] result = new int[bits.Length];
            for (int i = 0; i < bits.Length; i++)
            {
                result[i] = bits[(i + count) % bits.Length];
            }
            return result;
        }

        private static int[] XOR(int[] a, int[] b)
        {
            int[] result = new int[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                result[i] = a[i] ^ b[i];
            }
            return result;
        }

        private static int[][] GenerateRoundKeys(byte[] key)
        {
            int[] keyBits = BytesToBits(key);

            int[] permutedKey = Permute(keyBits, PC1);

            int[] C = new int[28];
            int[] D = new int[28];
            Array.Copy(permutedKey, 0, C, 0, 28);
            Array.Copy(permutedKey, 28, D, 0, 28);

            int[][] roundKeys = new int[16][];

            for (int round = 0; round < 16; round++)
            {
                C = LeftShift(C, LeftShifts[round]);
                D = LeftShift(D, LeftShifts[round]);

                int[] CD = new int[56];
                Array.Copy(C, 0, CD, 0, 28);
                Array.Copy(D, 0, CD, 28, 28);

                roundKeys[round] = Permute(CD, PC2);
            }

            return roundKeys;
        }

        private static int[] FeistelFunction(int[] R, int[] roundKey)
        {
            int[] expanded = Permute(R, E);

            int[] xored = XOR(expanded, roundKey);

            int[] sBoxOutput = new int[32];
            for (int i = 0; i < 8; i++)
            {
                int offset = i * 6;
                int row = (xored[offset] << 1) | xored[offset + 5];
                int col = (xored[offset + 1] << 3) | (xored[offset + 2] << 2) |
                          (xored[offset + 3] << 1) | xored[offset + 4];

                int sVal = SBox[i, row, col];

                for (int j = 0; j < 4; j++)
                {
                    sBoxOutput[i * 4 + j] = (sVal >> (3 - j)) & 1;
                }
            }

            return Permute(sBoxOutput, P);
        }

        private static byte[] ProcessBlock(byte[] block, int[][] roundKeys, bool decrypt)
        {
            int[] bits = BytesToBits(block);

            int[] permuted = Permute(bits, IP);

            int[] L = new int[32];
            int[] R = new int[32];
            Array.Copy(permuted, 0, L, 0, 32);
            Array.Copy(permuted, 32, R, 0, 32);

            for (int round = 0; round < 16; round++)
            {
                int keyIndex = decrypt ? (15 - round) : round;

                int[] newL = (int[])R.Clone();
                int[] f = FeistelFunction(R, roundKeys[keyIndex]);
                int[] newR = XOR(L, f);

                L = newL;
                R = newR;
            }

            int[] preOutput = new int[64];
            Array.Copy(R, 0, preOutput, 0, 32);
            Array.Copy(L, 0, preOutput, 32, 32);

            int[] output = Permute(preOutput, FP);

            return BitsToBytes(output);
        }

        public static string Encrypt(string plainText, byte[] key)
        {
            if (key.Length != 8)
                throw new ArgumentException("DES için anahtar 8 byte olmalıdır.");

            int[][] roundKeys = GenerateRoundKeys(key);
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

            int paddingLength = 8 - (plainBytes.Length % 8);
            byte[] paddedBytes = new byte[plainBytes.Length + paddingLength];
            Buffer.BlockCopy(plainBytes, 0, paddedBytes, 0, plainBytes.Length);
            for (int i = plainBytes.Length; i < paddedBytes.Length; i++)
                paddedBytes[i] = (byte)paddingLength;

            byte[] encrypted = new byte[paddedBytes.Length];
            for (int i = 0; i < paddedBytes.Length; i += 8)
            {
                byte[] block = new byte[8];
                Buffer.BlockCopy(paddedBytes, i, block, 0, 8);
                byte[] encBlock = ProcessBlock(block, roundKeys, false);
                Buffer.BlockCopy(encBlock, 0, encrypted, i, 8);
            }

            return Convert.ToBase64String(encrypted);
        }

        public static string Decrypt(string cipherText, byte[] key)
        {
            if (key.Length != 8)
                throw new ArgumentException("DES için anahtar 8 byte olmalıdır.");

            int[][] roundKeys = GenerateRoundKeys(key);
            byte[] encryptedBytes = Convert.FromBase64String(cipherText);
            byte[] decrypted = new byte[encryptedBytes.Length];

            for (int i = 0; i < encryptedBytes.Length; i += 8)
            {
                byte[] block = new byte[8];
                Buffer.BlockCopy(encryptedBytes, i, block, 0, 8);
                byte[] decBlock = ProcessBlock(block, roundKeys, true);
                Buffer.BlockCopy(decBlock, 0, decrypted, i, 8);
            }

            int paddingLength = decrypted[decrypted.Length - 1];
            if (paddingLength > 0 && paddingLength <= 8)
            {
                byte[] result = new byte[decrypted.Length - paddingLength];
                Buffer.BlockCopy(decrypted, 0, result, 0, result.Length);
                return Encoding.UTF8.GetString(result);
            }

            return Encoding.UTF8.GetString(decrypted);
        }

        public static int[,,] GetSBoxes()
        {
            return (int[,,])SBox.Clone();
        }
    }
}
