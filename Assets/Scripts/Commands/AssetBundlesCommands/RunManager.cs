namespace PFS.Assets.Scripts.Commands.AssetBundles
{
    public class RunManager
    {
        private static bool typeMode /*= true*/;
        private static string bookName;

        public static bool RunMode
        {
            get
            {
                return typeMode;
            }
            set
            {
                typeMode = value;
            }
        }

        public static string ControlBookTitle
        {
            get
            {
                return bookName;
            }
            set
            {
                bookName = value;
            }
        }
    }
}
