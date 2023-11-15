using System.ComponentModel;

namespace PFS.Assets.Scripts.Models.NetworkPaths
{
    public enum APIPaths
    {
        //-----Set statistics
        [Description("v4/child/activity/app-exit")] 
        APP_QUITTED,

        [Description("v4/child/activity/app-open")] 
        APP_STARTED,

        [Description("v4/child/activity/book-download")] 
        BOOK_DOWNLOADED,

        [Description("v4/child/activity/book-opened")] 
        BOOK_OPENED,

        [Description("v4/child/activity/book-liked")] 
        BOOK_LIKED,

        [Description("v4/child/activity/page-flipped")]
        PAGE_FLIPPED,

        [Description("v4/child/activity/word-clicked")]
        WORD_CLICKED,

        [Description("v4/child/activity/book-finished")]
        BOOK_FINISHED,

        [Description("v4/child/activity/quiz-started")]
        QUIZ_STARTED,

        [Description("v4/child/activity/quiz-quit")]
        QUIZ_QUITTED,

        //------------------


        //-----Get/Update user (teacher or child) 
        [Description("v4/teacher/profile/login-as-child")]
        GET_TEACHER_BY_PASSWORD,

        [Description("v4/0.6/child/profile")]
        GET_CHILD_DATA,

        [Description("v4/0.6/child/connect-with-classcode")]
        GET_CHILD_BY_SCHOOL_CODE,

        [Description("v4/0.6/child/profile/login")]
        GET_CHILD_BY_PASSWORD,

        [Description("v4/0.6/child/feide/login/callback")]
        GET_CHILD_BY_FEIDE,

        [Description("v4/booktique/child/verify")]
        GET_CHILD_BY_DEEPLINK,

        [Description("v4/0.6/child/profile/update")]
        UPDATE_CHILD,
        //---------------

        //-----FEIDE
        [Description("v4/child/feide/login")]
        GET_FEIDE_LINK,
        //-----------

        //-----CHECK USER EXIST
        [Description("v4/child/profile/check-exists")]
        CHECK_USERNAME,

        [Description("v4/teacher/profile/check-exists")]
        CHECK_TEACHERNAME,
        //---------------------

        //-----Books
        [Description("v4/0.7/child/library")]
        GETBOOKS,

        [Description("v4/child/get-downloaded-full-books")]
        GET_DOWNLOADED_BOOKS,

        [Description("v4/0.8/child/get-downloaded-books")]
        GET_DOWNLOADED_BOOK_IDS,

        [Description("v4/child/delete-downloaded-books")]
        REMOVE_BOOK_FROM_DOWNLOADED,

        [Description("v4/0.7/child/library/liked")]
        GET_FAVORITES_BOOKS,

        [Description("v4/0.7/child/profile/stats")]
        GET_CHILD_STATS,

        [Description("v4/0.7/child/homeworks/get-by-status")]
        GET_HOMEWORK_BY_STATUS,
        //-----------

        //------Subscription
        [Description("v4/child/subscription/verify-apple-receipt")]
        CHILD_APPLE_RECEPT_CONFIRMATION,

        [Description("v4/child/subscription/verify-google-receipt")]
        CHILD_GOOGLE_RECEPT_CONFIRMATION,

        [Description("v4/child/subscription/validate-apple-receipt")]
        VALIDATE_CHILD_SUBSCRIBTION,
        //-------------------

        //------Quizes
        [Description("v4/child/quiz/complete")]
        SET_QUIZ_CHILD_RESULT,

        [Description("v4/child/quiz")]
        GET_QUIZ_FOR_CHILD,

        [Description("v4/child/profile/stats/quiz")]
        GET_STATISTIC_ABOUT_QUIZ_FOR_CHILD,
        //------------

        //-----Other
        [Description("v4/child/groups")]
        GET_CHILD_CLASSES,

        [Description("v4/child/homeworks/new-tasks")]
        CHECK_HOMEWORK,

        [Description("v4/child/homeworks/statistics")]
        GET_HOMEWORKS_STATS,

        [Description("v4/child/country-code")]
        GET_COUNTRY_CODE_BY_IP,

        [Description("v4/child/profile/drop")]
        RESET_USER,

        [Description("v4/booktique/child/ttl")]
        CHECK_DEEPLINK_USER_TOKEN_EXPIRED,
        //----------

        //-----Support
        [Description("v4/child/support")]
        SUPPORT,
        //------------

        [Description("v4/app/versions")]
        APPVERSION,

    }

    public enum RequestType
    {
        GET = 0,
        POST,
        PUT,
        DELETE
    }
}