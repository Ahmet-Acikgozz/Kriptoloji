using System;
using System.Text;
using System.Linq;

public static class SubstitutionCipher
{
    private const string StandardAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";


    
    private static string Cipher(string inputText, string keyAlphabet, bool encrypt)
    {
        
        string key = keyAlphabet.ToUpper();
        if (key.Length != 26 || key.Distinct().Count() != 26)
        {
            throw new ArgumentException("Anahtar alfabe 26 harf uzunluğunda olmalı ve tüm harfler benzersiz olmalıdır.");
        }

        StringBuilder output = new StringBuilder();

        
        string source = encrypt ? StandardAlphabet : key;
        string target = encrypt ? key : StandardAlphabet;

        foreach (char inputChar in inputText)
        {
            if (char.IsLetter(inputChar))
            {
                bool isUpper = char.IsUpper(inputChar);
                char upperChar = char.ToUpper(inputChar);

                int index = source.IndexOf(upperChar);

                if (index != -1)
                {
                    char newChar = target[index];

                    output.Append(isUpper ? newChar : char.ToLower(newChar));
                }
                else
                {
                    output.Append(inputChar);
                }
            }
            else
            {
                output.Append(inputChar);
            }
        }

        return output.ToString();
    }

  
    public static string Encrypt(string plaintext, string keyAlphabet)
    {
        return Cipher(plaintext, keyAlphabet, true);
    }

    
    public static string Decrypt(string ciphertext, string keyAlphabet)
    {
        return Cipher(ciphertext, keyAlphabet, false);
    }
}
