using System;
using System.Text;

public static class VigenereCipher
{
    private const int AlphabetSize = 26;
    private const int LowercaseA = 'a';
    private const int UppercaseA = 'A';

    
    private static string Cipher(string inputText, string key, bool encrypt)
    {
        
        string standardKey = key.ToLower().Replace(" ", "");
        if (string.IsNullOrEmpty(standardKey))
        {
            
            return inputText;
        }

        StringBuilder output = new StringBuilder();
        int keyIndex = 0; 

        foreach (char inputChar in inputText)
        {
            if (char.IsLetter(inputChar))
            {
                
                int keyShift = standardKey[keyIndex % standardKey.Length] - LowercaseA;

                char offset = char.IsUpper(inputChar) ? (char)UppercaseA : (char)LowercaseA;

                int charIndex = inputChar - offset;

                int newCharIndex;

                if (encrypt)
                {
                    newCharIndex = (charIndex + keyShift) % AlphabetSize;
                }
                else
                {
                    
                    newCharIndex = (charIndex - keyShift + AlphabetSize) % AlphabetSize;
                }

                char newChar = (char)(newCharIndex + offset);
                output.Append(newChar);

                keyIndex++;
            }
            else
            {
                output.Append(inputChar);
            }
        }

        return output.ToString();
    }

   
    public static string Encrypt(string plaintext, string key)
    {
        return Cipher(plaintext, key, true);
    }

   
    public static string Decrypt(string ciphertext, string key)
    {
        return Cipher(ciphertext, key, false);
    }
}
