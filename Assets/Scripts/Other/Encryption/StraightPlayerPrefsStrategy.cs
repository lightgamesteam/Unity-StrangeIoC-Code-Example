using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightPlayerPrefsStrategy : IPlayerPrefsStrategy
{

   

    public string cryptorKey { get; set; }

    public float GetFloat(string pref, float defaultValue)
    {
        return PlayerPrefs.GetFloat(pref, defaultValue);
    }

    public int GetInt(string pref, int defaultValue)
    {
        return PlayerPrefs.GetInt(pref, defaultValue);
    }

    public string GetString(string pref, string defaultValue)
    {
        return PlayerPrefs.GetString(pref, defaultValue);
    }

    public void SetFloat(string pref, float value)
    {
        PlayerPrefs.SetFloat(pref, value);
    }

    public void SetInt(string pref, int value)
    {
        PlayerPrefs.SetInt(pref, value);
    }

    public void SetString(string pref, string value)
    {
        PlayerPrefs.SetString(pref, value);
    }
}
