using System.Linq;

public static partial class Extension
{
    public static string RemoveWhiteSpace(this string self)
    {
        return new string(self.Where(c => !char.IsWhiteSpace(c)).ToArray());
    }
}
