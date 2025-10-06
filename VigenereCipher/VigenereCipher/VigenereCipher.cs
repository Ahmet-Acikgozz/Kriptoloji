using System;
using System.Text;

public static class VigenereCipher
{
    private const int AlphabetSize = 26;
    private const int LowercaseA = 'a';
    private const int UppercaseA = 'A';

    /// <param name="inputText">Şifrelenecek veya deşifre edilecek metin.</param>
    /// <param name="key">Şifreleme anahtarı (kelime).</param>
    /// <param name="encrypt">Şifreleme için true, deşifre için false.</param>
    /// <returns>Şifreli veya deşifreli metin.</returns>
    private static string Cipher(string inputText, string key, bool encrypt)
    {
        // Anahtarı küçük harfe çevirip boşlukları temizleyerek standartlaştırıyoruz.
        string standardKey = key.ToLower().Replace(" ", "");
        if (string.IsNullOrEmpty(standardKey))
        {
            // Geçersiz anahtar durumunda orijinal metni döndür.
            return inputText;
        }

        StringBuilder output = new StringBuilder();
        int keyIndex = 0; // Anahtar kelimedeki mevcut harfin indeksi

        foreach (char inputChar in inputText)
        {
            if (char.IsLetter(inputChar))
            {
                // Mevcut anahtar harfinin alfabedeki sayısal değerini bul (0-25).
                // 'a' 0'a, 'b' 1'e denk gelir.
                int keyShift = standardKey[keyIndex % standardKey.Length] - LowercaseA;

                // Büyük veya küçük harf olup olmadığını kontrol et.
                char offset = char.IsUpper(inputChar) ? (char)UppercaseA : (char)LowercaseA;

                // Harfin alfabedeki 0-25 aralığındaki sayısal karşılığını bul.
                int charIndex = inputChar - offset;

                int newCharIndex;

                if (encrypt)
                {
                    // Şifreleme: (Harf + Anahtar kayması) mod 26
                    newCharIndex = (charIndex + keyShift) % AlphabetSize;
                }
                else
                {
                    // Deşifreleme: (Harf - Anahtar kayması + 26) mod 26
                    // +26, negatif sonuçları pozitif yapmak içindir (C# mod operatörü negatif sonuç verebilir).
                    newCharIndex = (charIndex - keyShift + AlphabetSize) % AlphabetSize;
                }

                // Yeni sayısal karşılığı tekrar karakter (harf) haline getir.
                char newChar = (char)(newCharIndex + offset);
                output.Append(newChar);

                // Sadece alfabe karakterleri şifrelendiğinde anahtar indeksini ilerlet.
                keyIndex++;
            }
            else
            {
                // Alfabe olmayan karakterleri (boşluk, noktalama vb.) olduğu gibi ekle.
                output.Append(inputChar);
            }
        }

        return output.ToString();
    }

    /// <summary>
    /// Metni Vigenère şifresi ile şifreler.
    /// </summary>
    public static string Encrypt(string plaintext, string key)
    {
        return Cipher(plaintext, key, true);
    }

    /// <summary>
    /// Şifreli metni Vigenère şifresi ile deşifre eder.
    /// </summary>
    public static string Decrypt(string ciphertext, string key)
    {
        return Cipher(ciphertext, key, false);
    }
}