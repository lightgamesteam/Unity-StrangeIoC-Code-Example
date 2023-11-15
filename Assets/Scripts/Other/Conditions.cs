using System.ComponentModel;

namespace Conditions
{
    public enum SoundType
    {
        Music = 0,
        Sound
    }

    public enum Languages
    {
        [Description("None")]
        None = -1,

        //American
        [Description("English")]
        English = 0,

        //Britain
        [Description("British")]
        British,

        [Description("Spanish")]
        Spanish,

        [Description("Portuguese")]
        Portuguese,

        [Description("Ukrainian")]
        Ukrainian,

        [Description("Russian")]
        Russian,

        [Description("Polish")]
        Polish,

        [Description("German")]
        German,

        [Description("Italian")]
        Italian,

        [Description("Norwegian")]
        Norwegian,

        [Description("Chinese")]
        Chinese,

        [Description("Danish")]
        Danish,

        [Description("Swedish")]
        Swedish,

        [Description("Swiss")]
        Swiss,

        [Description("Dutch")]
        Dutch,

        [Description("Thai")]
        Thai,

        [Description("NyNorsk")]
        NyNorsk,

        [Description("KeyLanguage")]
        KeyLanguage
    }

    public enum CountryCodes
    {
        [Description("NO")]
        Norway,

        [Description("GB")]
        GreatBritain,

        [Description("CN")]
        China,

        [Description("TH")]
        Thailand,

        [Description("IN")]
        India,

        [Description("DK")]
        Denmark,

        [Description("NN-NO")]
        NyNorsk,

        [Description("KEYCODE")]
        KeyCode
    }

    public enum SimplifiedLevels
    {
        None = -1,

        [Description("stage1")]
        S1 = 0,

        [Description("stage2")]
        S2,

        [Description("stage3")]
        S3,

        [Description("stage4")]
        S4,

        [Description("stage5")]
        S5,

        [Description("stage6")]
        S6,

        [Description("stage7")]
        S7,

        [Description("stage8")]
        S8
    }

    public enum BookLibraryType
    {
        [Description("FullLibrary")]
        FullLibrary,

        [Description("SchoolLibrary")]
        SchoolLibrary,

        [Description("ClassLibrary")]
        ClassLibrary
    }

    public enum BookContentType
    {
        [Description("regularbook")]
        Native = 0,
        [Description("animatedBooks")]
        Animated,
        [Description("songBook")]
        Song,
        [Description("newspaper")]
        Newspaper
    }

    public enum BooksRequestType
    {
        BooksByCategory = 0,
        SearchResult,
        FurturedBooks,
        DownloadedBooks,
        FavoritesBooks
    }

    public enum CustomizationType
    {
        Male = 0,
        Female,
        All
    }

    public enum CustomizationCategories
    {
        Hair = 0,
        Hat,
        Glasses,
        Body,
        Legs,
        Shoes,
        Color
    }

    public enum BookTracking
    {
        bookDownload,
        bookOpened,
        pageFlip,
        wordClick,
        heartBeat,
        bookFinished
    }

    public enum DisconnectEvents
    {
        NoInternet = 0,
        SlowInternet,
        ServerProblem
    }

    public enum GameModes
    {
        None = 0,
        SchoolModeForChildLogin,
        SchoolModeForChildFeide,
        SchoolModeForChildDeepLink,

        SchoolModeForTeacherLogin,
        SchoolModeForTeacherFeide,
    }

    public enum QuizType
    {
        /// <summary>
        /// Tap conveyor
        /// </summary>
        [Description("QuizType1")]
        Type1 = 0,

        /// <summary>
        /// Drag and drop line
        /// </summary>
        [Description("QuizType2")]
        Type2,

        /// <summary>
        /// Answer ninja
        /// </summary>
        [Description("QuizType3")]
        Type3,

        /// <summary>
        /// Memory game
        /// </summary>
        [Description("QuizType4")]
        Type4,

        /// <summary>
        /// Complex quiz
        /// </summary>
        [Description("QuizType5")]
        Type5
    }

    public enum QuizQuestionsType
    {
        /// <summary>
        /// Only visible questions
        /// </summary>
        [Description("visible")]
        Visible = 0,

        /// <summary>
        /// Only invisible questions
        /// </summary>
        [Description("invisible")]
        Invisible,

        /// <summary>
        /// All questions
        /// </summary>
        [Description("all")]
        All
    }

    public enum HomeworkStatus
    {
        /// <summary>
        /// New homework
        /// </summary>
        [Description("New")]
        New = 0,

        /// <summary>
        /// Checked homework
        /// </summary>
        [Description("Checked")]
        Checked
    }

    public enum BookReadingType
    {
        [Description("None")]
        None = 0,

        [Description("readMyself")]
        ReadMyself,

        [Description("readToMe")]
        ReadToMe,

        [Description("autoRead")]
        AutoRead,
    }
}