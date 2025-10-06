using System;

class Program
{
    static void Main()
    {
       
        string keyAlphabet = "ZYXWVUTSRQPONMLKJIHGFEDCBA";
        string originalText = "Merhaba , Hosgeldiniz !";

        Console.WriteLine($"Orijinal Metin: {originalText}");
        Console.WriteLine($"Anahtar Alfabe: {keyAlphabet}\n");

        try
        {
            // Şifreleme
            string encryptedText = SubstitutionCipher.Encrypt(originalText, keyAlphabet);
            Console.WriteLine($"Şifreli Metin:  {encryptedText}");

            // Deşifreleme
            string decryptedText = SubstitutionCipher.Decrypt(encryptedText, keyAlphabet);
            Console.WriteLine($"Deşifreli Metin: {decryptedText}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Hata: {ex.Message}");
        }

    }
}