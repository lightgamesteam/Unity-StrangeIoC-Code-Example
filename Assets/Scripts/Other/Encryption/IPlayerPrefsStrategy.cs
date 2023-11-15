public interface IPlayerPrefsStrategy 
{
    int GetInt(string pref, int defaultValue);
    void SetInt(string pref, int value);
    float GetFloat(string pref, float defaultValue);
    void SetFloat(string pref, float value);
    string GetString(string pref, string defaultValue);
    void SetString(string pref, string value);

    string cryptorKey { get; set; }
}
