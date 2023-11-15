using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncryptedPlayerPrefsStrategy : IPlayerPrefsStrategy
{
    public string cryptorKey { get; set; }

    public float GetFloat(string pref, float defaultValue)
    {
        return float.Parse(Cryptor.DecryptString(cryptorKey, PlayerPrefs.GetString(pref, defaultValue.ToString()).ToString()));
    }

    public int GetInt(string pref, int defaultValue)
    {
        return int.Parse(Cryptor.DecryptString(cryptorKey, PlayerPrefs.GetString(pref, defaultValue.ToString()).ToString()));
    }

    public string GetString(string pref, string defaultValue)
    {
        return Cryptor.DecryptString(cryptorKey, PlayerPrefs.GetString(pref, defaultValue).ToString());
    }

    public void SetFloat(string pref, float value)
    {
        PlayerPrefs.SetString(pref, Cryptor.EncryptString(cryptorKey, value.ToString())); 
    }

    public void SetInt(string pref, int value)
    {
        PlayerPrefs.SetString(pref, Cryptor.EncryptString(cryptorKey, value.ToString()));
    }

    public void SetString(string pref, string value)
    {
        PlayerPrefs.SetString(pref, Cryptor.EncryptString(cryptorKey, value.ToString()));
    }
}
