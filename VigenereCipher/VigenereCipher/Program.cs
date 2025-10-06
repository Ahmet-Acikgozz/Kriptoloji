using System;

class Program
{
    static void Main()
    {
        string key = "cipher"; // Anahtar kelimeniz
        string originalText = "Merhaba Hosgeldiniz !";

        Console.WriteLine($"Orijinal Metin: {originalText}");
        Console.WriteLine($"Anahtar: {key}\n");

        // Şifreleme
        string encryptedText = VigenereCipher.Encrypt(originalText, key);
        Console.WriteLine($"Şifreli Metin:  {encryptedText}");
        
        // Deşifreleme
        string decryptedText = VigenereCipher.Decrypt(encryptedText, key);
        Console.WriteLine($"Deşifreli Metin: {decryptedText}");

    }
}