using BooksPlayer;
using Conditions;

namespace PFS.Assets.Scripts.Models
{
    public static class PFSforUBP
    {
        public static BookPlayRegime GetReadType(BookReadingType readType, bool readOnly)
        {
            BookPlayRegime type = BookPlayRegime.SilentMode;

            if (!readOnly)
            {
                switch (readType)
                {
                    case BookReadingType.AutoRead:
                        type = BookPlayRegime.AutoPlay;
                        break;
                    case BookReadingType.ReadMyself:
                        type = BookPlayRegime.ReadMySelf;
                        break;
                    default:
                        type = BookPlayRegime.AutoPlay;
                        break;
                }
            }

            return type;
        }

        public static BookLanguage GetBookLanguage(Languages language)
        {
            BookLanguage lang = BookLanguage.English;

            switch (language)
            {
                case Languages.English:
                    lang = BookLanguage.English;
                    break;
                case Languages.British:
                    lang = BookLanguage.British;
                    break;
                case Languages.Norwegian:
                    lang = BookLanguage.Norwegian;
                    break;
                case Languages.NyNorsk:
                    lang = BookLanguage.NyNorsk;
                    break;
                case Languages.Swiss:
                    lang = BookLanguage.Swiss;
                    break;
                case Languages.Chinese:
                    lang = BookLanguage.Chinese;
                    break;
                case Languages.Danish:
                    lang = BookLanguage.Danish;
                    break;
                default:
                    lang = BookLanguage.English;
                    break;
            }

            return lang;
        }

        public static BookLanguage GetBookLanguage(string countryCode)
        {
            BookLanguage lang = BookLanguage.English;

            if (countryCode == BookLanguage.English.ToDescription())
            {
                lang = BookLanguage.English;
            }
            else if (countryCode == BookLanguage.British.ToDescription())
            {
                lang = BookLanguage.British;
            }
            else if (countryCode == BookLanguage.Norwegian.ToDescription())
            {
                lang = BookLanguage.Norwegian;
            }
            else if (countryCode == BookLanguage.NyNorsk.ToDescription())
            {
                lang = BookLanguage.NyNorsk;
            }
            else if (countryCode == BookLanguage.Swiss.ToDescription())
            {
                lang = BookLanguage.Swiss;
            }
            else if (countryCode == BookLanguage.Chinese.ToDescription())
            {
                lang = BookLanguage.Chinese;
            }
            else if (countryCode == BookLanguage.Danish.ToDescription())
            {
                lang = BookLanguage.Danish;
            }

            return lang;
        }
    }
}