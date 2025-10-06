using System;
using System.Text;
using System.Linq;

public static class SubstitutionCipher
{
    private const string StandardAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";


    /// <param name="inputText">Şifrelenecek veya deşifre edilecek metin.</param>
    /// <param name="keyAlphabet">Şifreleme için kullanılacak 26 harflik karışık alfabe.</param>
    /// <param name="encrypt">Şifreleme için true, deşifre için false.</param>
    /// <returns>Şifreli veya deşifreli metin.</returns>
    private static string Cipher(string inputText, string keyAlphabet, bool encrypt)
    {
        // Anahtarı standart hale getir: Büyük harf ve 26 harf kontrolü.
        string key = keyAlphabet.ToUpper();
        if (key.Length != 26 || key.Distinct().Count() != 26)
        {
            throw new ArgumentException("Anahtar alfabe 26 harf uzunluğunda olmalı ve tüm harfler benzersiz olmalıdır.");
        }

        StringBuilder output = new StringBuilder();

        // Hangi eşleşmeyi kullanacağımızı belirliyoruz.
        // Şifreleme (Encrypt): Standart Alfabeden Key Alfabeye
        // Deşifreleme (Decrypt): Key Alfabeden Standart Alfabeye
        string source = encrypt ? StandardAlphabet : key;
        string target = encrypt ? key : StandardAlphabet;

        foreach (char inputChar in inputText)
        {
            if (char.IsLetter(inputChar))
            {
                // Harfin büyük/küçük harf durumunu korumak için orijinal durumu kaydet.
                bool isUpper = char.IsUpper(inputChar);
                char upperChar = char.ToUpper(inputChar);

                // Kaynak alfabedeki harfin indeksini bul.
                int index = source.IndexOf(upperChar);

                if (index != -1)
                {
                    // Hedef alfabedeki karşılık gelen harfi al.
                    char newChar = target[index];

                    // Orijinal büyük/küçük harf durumunu uygula.
                    output.Append(isUpper ? newChar : char.ToLower(newChar));
                }
                else
                {
                    // Bir nedenden dolayı (olmamalı) bulunamazsa orijinal harfi koru.
                    output.Append(inputChar);
                }
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
    /// Metni Substitution Şifresi ile şifreler.
    /// </summary>
    public static string Encrypt(string plaintext, string keyAlphabet)
    {
        return Cipher(plaintext, keyAlphabet, true);
    }

    /// <summary>
    /// Şifreli metni Substitution Şifresi ile deşifre eder.
    /// </summary>
    public static string Decrypt(string ciphertext, string keyAlphabet)
    {
        return Cipher(ciphertext, keyAlphabet, false);
    }
}