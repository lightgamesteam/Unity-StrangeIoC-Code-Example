using UnityEngine;
using System.Text;
using System;

public static class PasswordsHelper 
{
    public const byte MinPasswordLength = 6;

    /// <summary>
    /// To base64 encode
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public static string PasswordEncode(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            Debug.LogError("PasswordsHelper -> PasswordEncode -> password empty or null");
            return password;
        }

        password = password.Trim();
        byte[] textBytes = Encoding.UTF8.GetBytes(password);

        return Convert.ToBase64String(textBytes);
    }

    /// <summary>
    /// To base64 encode
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public static void PasswordEncode(ref string password)
    {
        password = PasswordEncode(password);
    }

    /// <summary>
    /// To base64 decode
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public static string PasswordDecode(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            Debug.LogError("PasswordsHelper -> PasswordDecode -> password empty or null");
            return password;
        }

        byte[] textBytes = Convert.FromBase64String(password);
        return Encoding.UTF8.GetString(textBytes);
    }

    /// <summary>
    /// To base64 decode
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public static void PasswordDecode(ref string password)
    {
        password = PasswordDecode(password);
    }
}