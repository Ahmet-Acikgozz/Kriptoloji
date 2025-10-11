using System;
using System.Text;

public static class AffineCipher
{
    private const int AlphabetSize = 26;
    private const int LowercaseA = 'a';
    private const int UppercaseA = 'A';

  


    
    private static int FindInverse(int a)
    {
        
        for (int i = 1; i < AlphabetSize; i++)
        {
            if ((a * i) % AlphabetSize == 1)
            {
                return i;
            }
        }

        
        throw new ArgumentException("Geçersiz anahtar 'a'. Anahtar 26 ile aralarında asal olmalıdır.");
    }

    
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
            
            aInverse = FindInverse(a);
        }

        foreach (char inputChar in inputText)
        {
            if (char.IsLetter(inputChar))
            {
                char offset = char.IsUpper(inputChar) ? (char)UppercaseA : (char)LowercaseA;

                int x = inputChar - offset;
                int y; 

                if (encrypt)
                {
                    y = (a * x + b) % AlphabetSize;
                }
                else
                {
                    
                    int shifted = x - b;
                    y = (aInverse * (shifted % AlphabetSize + AlphabetSize)) % AlphabetSize;
                }

                char newChar = (char)(y + offset);
                output.Append(newChar);
            }
            else
            {
                
                output.Append(inputChar);
            }
        }

        return output.ToString();
    }

    
    public static string Encrypt(string plaintext, int a, int b)
    {
        return Cipher(plaintext, a, b, true);
    }

    
    public static string Decrypt(string ciphertext, int a, int b)
    {
        return Cipher(ciphertext, a, b, false);
    }
}
