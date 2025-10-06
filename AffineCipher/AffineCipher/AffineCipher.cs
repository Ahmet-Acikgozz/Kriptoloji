using System;
using System.Text;

public static class AffineCipher
{
    private const int AlphabetSize = 26;
    private const int LowercaseA = 'a';
    private const int UppercaseA = 'A';

    // (ax+b)(mod26) = ŞİFRELEME
    // a^−1  *(y−b)(mod26)  = DEŞİFRELEME


    /// <param name="a">Çarpımsal tersi aranacak anahtar (a anahtarı).</param>
    /// <returns>a'nın mod 26'daki çarpımsal tersi.</returns>
    private static int FindInverse(int a)
    {
        // Euclid'in genişletilmiş algoritmasını veya brute-force yöntemini kullanabiliriz.
        // Affine şifresi için brute-force daha basittir: (a * i) mod 26 == 1
        for (int i = 1; i < AlphabetSize; i++)
        {
            if ((a * i) % AlphabetSize == 1)
            {
                return i;
            }
        }

        // Bu, a anahtarının 26 ile aralarında asal olmadığı anlamına gelir.
        // Normalde burada bir hata fırlatılmalıdır.
        throw new ArgumentException("Geçersiz anahtar 'a'. Anahtar 26 ile aralarında asal olmalıdır.");
    }

    /// <param name="inputText">Şifrelenecek veya deşifre edilecek metin.</param>
    /// <param name="a">Çarpım anahtarı (26 ile aralarında asal olmalı).</param>
    /// <param name="b">Kaydırma anahtarı.</param>
    /// <param name="encrypt">Şifreleme için true, deşifre için false.</param>
    /// <returns>Şifreli veya deşifreli metin.</returns>
    private static string Cipher(string inputText, int a, int b, bool encrypt)
    {
        if (a % 2 == 0 || a == 13 || a < 0 || a >= AlphabetSize)
        {
            throw new ArgumentException("Anahtar 'a' 26 ile aralarında asal olmalıdır.");
        }

        StringBuilder output = new StringBuilder();
        int aInverse = 0;

        if (!encrypt)
        {
            // Deşifreleme için a^-1 (a'nın çarpımsal tersi) gereklidir.
            aInverse = FindInverse(a);
        }

        foreach (char inputChar in inputText)
        {
            if (char.IsLetter(inputChar))
            {
                // Büyük veya küçük harf için başlangıç ASCII değerini al.
                char offset = char.IsUpper(inputChar) ? (char)UppercaseA : (char)LowercaseA;

                // Harfin 0-25 aralığındaki sayısal karşılığını bul (x).
                int x = inputChar - offset;
                int y; // Yeni harfin sayısal karşılığı.

                if (encrypt)
                {
                    // Şifreleme: E(x) = (ax + b) mod 26
                    y = (a * x + b) % AlphabetSize;
                }
                else
                {
                    // Deşifreleme: D(y) = a^-1 (y - b) mod 26
                    // Mod alma işleminde negatif sayılardan kaçınmak için (x - b) işlemine 26 ekleyip mod alıyoruz.
                    int shifted = x - b;
                    y = (aInverse * (shifted % AlphabetSize + AlphabetSize)) % AlphabetSize;
                }

                // Yeni sayısal karşılığı tekrar karakter haline getir.
                char newChar = (char)(y + offset);
                output.Append(newChar);
            }
            else
            {
                // Alfabe olmayan karakterleri olduğu gibi ekle.
                output.Append(inputChar);
            }
        }

        return output.ToString();
    }

    /// <summary>
    /// Metni Affine şifresi ile şifreler.
    /// </summary>
    public static string Encrypt(string plaintext, int a, int b)
    {
        return Cipher(plaintext, a, b, true);
    }

    /// <summary>
    /// Şifreli metni Affine şifresi ile deşifre eder.
    /// </summary>
    public static string Decrypt(string ciphertext, int a, int b)
    {
        return Cipher(ciphertext, a, b, false);
    }
}