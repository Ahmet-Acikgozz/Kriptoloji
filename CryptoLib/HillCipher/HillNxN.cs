using System;
using System.Text;

namespace CryptoLib.HillCipher
{
    public static class HillNxN
    {
        public static int Determinant(int[,] matrix, int n)
        {
            if (n == 1)
                return matrix[0, 0] % 26;

            if (n == 2)
            {
                int det = (matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0]) % 26;
                return det < 0 ? det + 26 : det;
            }

            int det3 = 0;
            for (int col = 0; col < n; col++)
            {
                int[,] subMatrix = GetSubMatrix(matrix, 0, col, n);
                int sign = (col % 2 == 0) ? 1 : -1;
                det3 += sign * matrix[0, col] * Determinant(subMatrix, n - 1);
            }

            det3 = det3 % 26;
            return det3 < 0 ? det3 + 26 : det3;
        }

        private static int[,] GetSubMatrix(int[,] matrix, int excludeRow, int excludeCol, int n)
        {
            int[,] sub = new int[n - 1, n - 1];
            int r = 0;
            for (int i = 0; i < n; i++)
            {
                if (i == excludeRow) continue;
                int c = 0;
                for (int j = 0; j < n; j++)
                {
                    if (j == excludeCol) continue;
                    sub[r, c] = matrix[i, j];
                    c++;
                }
                r++;
            }
            return sub;
        }

        public static int ModInverse(int a, int mod)
        {
            a = ((a % mod) + mod) % mod;
            for (int x = 1; x < mod; x++)
            {
                if ((a * x) % mod == 1)
                    return x;
            }
            return -1;
        }

        private static int[,] CofactorMatrix(int[,] matrix, int n)
        {
            int[,] cofactor = new int[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    int[,] sub = GetSubMatrix(matrix, i, j, n);
                    int sign = ((i + j) % 2 == 0) ? 1 : -1;
                    int det = Determinant(sub, n - 1);
                    cofactor[i, j] = (sign * det) % 26;
                    if (cofactor[i, j] < 0)
                        cofactor[i, j] += 26;
                }
            }

            return cofactor;
        }

        private static int[,] Transpose(int[,] matrix, int n)
        {
            int[,] trans = new int[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    trans[j, i] = matrix[i, j];
            return trans;
        }

        public static int[,] InverseMatrix(int[,] matrix, int n)
        {
            int det = Determinant(matrix, n);
            int detInv = ModInverse(det, 26);

            if (detInv == -1)
                throw new Exception($"Matrisin tersi yok! Determinant ({det}) ile 26 aralarında asal değil.");

            int[,] cofactor = CofactorMatrix(matrix, n);

            int[,] adjugate = Transpose(cofactor, n);

            int[,] inverse = new int[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    inverse[i, j] = (detInv * adjugate[i, j]) % 26;
                    if (inverse[i, j] < 0)
                        inverse[i, j] += 26;
                }
            }

            return inverse;
        }

        public static bool IsValidKey(int[,] matrix, int n)
        {
            int det = Determinant(matrix, n);
            if (det == 0 || det % 2 == 0 || det % 13 == 0)
                return false;
            return true;
        }

        public static int[,] ParseKey(string input, int n)
        {
            int[,] matrix = new int[n, n];
            input = input.Trim();

            if (char.IsDigit(input[0]) || input[0] == '-')
            {
                string[] parts = input.Split(new char[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != n * n)
                    throw new Exception($"Hill {n}x{n} matrisi için {n * n} sayı gerekli.");

                int idx = 0;
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                        matrix[i, j] = (int.Parse(parts[idx++]) % 26 + 26) % 26;
            }
            else
            {
                input = input.ToUpper().Replace(" ", "");
                if (input.Length != n * n)
                    throw new Exception($"Hill {n}x{n} matrisi için {n * n} harf gerekli.");

                int idx = 0;
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                        matrix[i, j] = input[idx++] - 'A';
            }

            return matrix;
        }

        public static string Encrypt(string text, string keyString, int n)
        {
            int[,] matrix = ParseKey(keyString, n);

            if (!IsValidKey(matrix, n))
                throw new Exception($"Matris determinantı 26 ile aralarında asal değil!");

            text = text.Replace(" ", "").ToUpper();
            while (text.Length % n != 0)
                text += 'X';

            StringBuilder result = new StringBuilder();

            for (int i = 0; i < text.Length; i += n)
            {
                int[] vector = new int[n];
                for (int j = 0; j < n; j++)
                    vector[j] = text[i + j] - 'A';

                int[] encrypted = new int[n];
                for (int row = 0; row < n; row++)
                {
                    int sum = 0;
                    for (int col = 0; col < n; col++)
                        sum += matrix[row, col] * vector[col];
                    encrypted[row] = sum % 26;
                }

                for (int j = 0; j < n; j++)
                    result.Append((char)(encrypted[j] + 'A'));
            }

            return result.ToString();
        }

        public static string Decrypt(string cipher, string keyString, int n)
        {
            int[,] matrix = ParseKey(keyString, n);
            int[,] invMatrix = InverseMatrix(matrix, n);

            StringBuilder result = new StringBuilder();

            for (int i = 0; i < cipher.Length; i += n)
            {
                int[] vector = new int[n];
                for (int j = 0; j < n; j++)
                    vector[j] = cipher[i + j] - 'A';

                int[] decrypted = new int[n];
                for (int row = 0; row < n; row++)
                {
                    int sum = 0;
                    for (int col = 0; col < n; col++)
                        sum += invMatrix[row, col] * vector[col];
                    decrypted[row] = sum % 26;
                    if (decrypted[row] < 0)
                        decrypted[row] += 26;
                }

                for (int j = 0; j < n; j++)
                    result.Append((char)(decrypted[j] + 'A'));
            }

            return result.ToString().TrimEnd('X');
        }

        public static string Encrypt2x2(string text, string key) => Encrypt(text, key, 2);

        public static string Decrypt2x2(string cipher, string key) => Decrypt(cipher, key, 2);

        public static string Encrypt3x3(string text, string key) => Encrypt(text, key, 3);

        public static string Decrypt3x3(string cipher, string key) => Decrypt(cipher, key, 3);

        public static string Encrypt4x4(string text, string key) => Encrypt(text, key, 4);

        public static string Decrypt4x4(string cipher, string key) => Decrypt(cipher, key, 4);
    }
}
