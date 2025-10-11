using System;

class Program
{
    static void Main()
    {
        Console.Write("Kaydırma miktarını giriniz: ");
        int shift = int.Parse(Console.ReadLine());

        Console.Write("Şifrelemek istediğiniz metni giriniz: ");
        string text = Console.ReadLine();

        string encrypted = CaesarEncrypt(text, shift);
        Console.WriteLine("Şifrelenmiş metin: " + encrypted);

        string decrypted = CaesarDecrypt(encrypted, shift);
        Console.WriteLine("Çözülmüş metin: " + decrypted);
    }

    static string CaesarEncrypt(string input, int shift)
    {
        char[] result = new char[input.Length];

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            if (c >= 'a' && c <= 'z')
            {
                result[i] = (char)((((c - 'a') + shift) % 26) + 'a');
            }
            else if (c >= 'A' && c <= 'Z')
            {
                result[i] = (char)((((c - 'A') + shift) % 26) + 'A');
            }
            else
            {
                result[i] = c; 
            }
        }

        return new string(result);
    }

    static string CaesarDecrypt(string input, int shift)
    {
        return CaesarEncrypt(input, 26 - (shift % 26)); 
    }
}
