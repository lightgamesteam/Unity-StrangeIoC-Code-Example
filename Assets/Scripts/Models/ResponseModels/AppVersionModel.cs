namespace PFS.Assets.Scripts.Models.Responses
{

    public class AppVersionsModel
    {
        public AppVersionModel currentVersion;
        public AppVersionModel latestVersion;
    }

    public class AppVersionModel
    {
        public string version;
        //public string expireDate;
        public bool isForceUpdate;

        //---overload operators to compare version as objects
        public static bool operator >(AppVersionModel version1, AppVersionModel version2) => CompareVersions(ParseVersion(version1.version), ParseVersion(version2.version));
        public static bool operator <(AppVersionModel version1, AppVersionModel version2) => !CompareVersions(ParseVersion(version1.version), ParseVersion(version2.version));
        public static bool operator ==(AppVersionModel version1, AppVersionModel version2) => version1.version == version2.version;
        public static bool operator !=(AppVersionModel version1, AppVersionModel version2) => version1.version != version2.version;

        /// <summary>
        /// Convert version from string to integer array
        /// </summary>
        /// <param name="tempVersion"></param>
        /// <returns></returns>
        private static int[] ParseVersion(string tempVersion)
        {
            int[] versions = new int[3];

            for (int i = 0; i < 3; i++)
            {
                int lenght = tempVersion.IndexOf(".");
                string v = "0";
                if (lenght > 0)
                {
                    v = tempVersion.Substring(0, lenght);
                }
                else
                {
                    v = tempVersion;
                }

                versions[i] = int.Parse(v);

                if ((lenght + 1) < tempVersion.Length && lenght > -1)
                {
                    tempVersion = tempVersion.Substring(lenght + 1);
                }
                else
                {
                    break;
                }
            }
            return versions;
        }

        /// <summary>
        /// Compare is higher version1 then version2
        /// </summary>
        /// <param name="version1"></param>
        /// <param name="version2"></param>
        /// <returns></returns>
        private static bool CompareVersions(int[] version1, int[] version2)
        {
            for (int i = 0; i < version1.Length; i++)
            {
                if (version1[i] > version2[i])
                {
                    return true;
                }
                else if (version1[i] < version2[i])
                {
                    return false;
                }
            }
            return false;
        }
    }
}