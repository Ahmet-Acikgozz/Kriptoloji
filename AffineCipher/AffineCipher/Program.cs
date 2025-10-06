using System;

class Program
{
    static void Main()
    {
        // Anahtar seçimi:
        // 'a' 26 ile aralarında asal olmalı (Örn: 5)
        // 'b' 0-25 arasında herhangi bir tam sayı olabilir (Örn: 8)
        int keyA = 5;
        int keyB = 8;
        string originalText = "Merhaba Hosgeldiniz !";

        Console.WriteLine($"Orijinal Metin: {originalText}");
        Console.WriteLine($"Anahtarlar: a={keyA}, b={keyB}\n");

        try
        {
            // Şifreleme
            string encryptedText = AffineCipher.Encrypt(originalText, keyA, keyB);
            Console.WriteLine($"Şifreli Metin:  {encryptedText}");

            // Deşifreleme
            string decryptedText = AffineCipher.Decrypt(encryptedText, keyA, keyB);
            Console.WriteLine($"Deşifreli Metin: {decryptedText}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Hata: {ex.Message}");
        }

       
    }
}