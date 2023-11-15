using System.ComponentModel;

namespace Assets.Scripts.Services.Analytics
{
    public enum Property
    {
        [Description("uuid")]
        Uuid,

        [Description("QuizId")]
        QuizId,
        
        [Description("BookId")]
        BookId,

        [Description("ClassId")]
        ClassId,

        [Description("ISBN")]
        ISBN,

        [Description("category")]
        Category,

        [Description("QuizQuestionsSeenCount")]
        QuizQuestionsSeenCount,

        [Description("Skip")]
        Skip,

        [Description("BookReadCount")]
        BookReadCount,

        [Description("WeeklyTimeSpent_s")]
        WeeklyTimeSpent_s,

        [Description("CompletedAssignmentCount")]
        CompletedAssignmentCount,
        
        [Description("SignOutTime_s")]
        SignOutTime_s,

        [Description("SignInTime_s")]
        SignInTime_s,

        [Description("LoginType")]
        LoginType,
        
        [Description("country")]
        Country,

        [Description("region")]
        Region,

        [Description("region/country")]
        RegionCountry,

        [Description("QuizCorrectAnswer")]
        QuizCorrectAnswer,

        [Description("QuizIncorrectAnswer")]
        QuizIncorrectAnswer,

        [Description("QuizScore/Coins")]
        QuizScoreCoins,

        [Description("translation")]
        Translation,

        [Description("BookLevel")]
        BookLevel,

        [Description("avatar tag")]
        AvatarTag,

        [Description("ReadingMode")]
        ReadingMode,

        [Description("PageSeenCount")]
        PageSeenCount,

        [Description("Seen")]
        Seen,

        [Description("profile")]
        Profile,

        [Description("theme")]
        Theme,

        [Description("Reading_Time_s")]
        ReadingTimeS,

        [Description("BookReadSpeed")]
        BookReadSpeed,

        [Description("AnimatedBook")]
        AnimatedBook,

        [Description("PageNumber")]
        PageNumber


    }

    public enum EventName
    {
        [Description("Action.BackToBooks")]
        ActionBackToBooks,

        [Description("Action.BookAddToFavourites")]
        ActionBookAddToFavourites,

        [Description("Action.BookCallout")]
        ActionBookCallout,

        [Description("Action.BookClose")]
        ActionBookClose,

        [Description("Action.BookClosePercentageSeen")]
        ActionBookClosePercentageSeen,

        [Description("Action.BookDownload")]
        ActionBookDownload,

        [Description("Action.BookDownloadError")]
        ActionBookDownloadError,

        [Description("Action.BookDownloadFinish")]
        ActionBookDownloadFinish,

        [Description("Action.BookMute")]
        ActionBookMute,

        [Description("Action.BookOpen")]
        ActionBookOpen,

        [Description("Action.BookPageTurn")]
        ActionBookPageTurn,

        [Description("Action.BookPause")]
        ActionBookPause,

        [Description("Action.BookPlay")]
        ActionBookPlay,

        [Description("Action.BookReadAgain")]
        ActionBookReadAgain,

        [Description("Action.BookReadSpeed")]
        ActionBookReadSpeed,

        [Description("Action.BookUnmute")]
        ActionBookUnmute,

        [Description("Action.ChooseAvatar")]
        ActionChooseAvatar,

        [Description("Action.CloseQuiz")]
        ActionCloseQuiz,

        [Description("Action.CloseTextZoomView")]
        ActionCloseTextZoomView,

        [Description("Action.DeleteBooksFromLibrary")]
        ActionDeleteBooksFromLibrary,

        [Description("Action.EnlargeTextSizeOnTextZoomView")]
        ActionEnlargeTextSizeOnTextZoomView,

        [Description("Action.JoinClass")]
        ActionJoinClass,

        [Description("Action.onQuizQuestionAnswer")]
        ActionOnQuizQuestionAnswer,

        [Description("Action.OnSignIn")]
        ActionOnSignIn,

        [Description("Action.onSignOut")]
        ActionOnSignOut,

        [Description("Action.onUserChangeTheme")]
        ActionOnUserChangeTheme,

        [Description("Action.PauseQuiz")]
        ActionPauseQuiz,

        [Description("Action.QuizCompletePercentageSeen")]
        ActionQuizCompletePercentageSeen,

        [Description("Action.ReduceTextSizeOnTextZoomView")]
        ActionReduceTextSizeOnTextZoomView,

        [Description("Action.ReplayQuizQuestion")]
        ActionReplayQuizQuestion,

        [Description("Action.Search")]
        ActionSearch,

        [Description("Action.SelectProfile")]
        ActionSelectProfile,

        [Description("Action.SignIn")]
        ActionSignIn,

        [Description("Action.SubscriptionSuccess")]
        ActionSubscriptionSuccess,

        [Description("Action.SwitchBookLanguage")]
        ActionSwitchBookLanguage,

        [Description("Action.SwitchUILanguage")]
        ActionSwitchUILanguage,

        [Description("Action.TextZoomView")]
        ActionTextZoomView,

        [Description("Action.WithSignUp")]
        ActionWithSignUp,

        [Description("Navigation.BookCoverExit")]
        NavigationBookCoverExit,

        [Description("Navigation.BookDetails")]
        NavigationBookDetails,

        [Description("Navigation.BookInventory")]
        NavigationBookInventory,

        [Description("Navigation.ConnectToNewClass")]
        NavigationConnectToNewClass,

        [Description("Navigation.Favourite")]
        NavigationFavourite,

        [Description("Navigation.Feedback")]
        NavigationFeedback,

        [Description("Navigation.ForgetPassword")]
        NavigationForgetPassword,

        [Description("Navigation.HomeProfile")]
        NavigationHomeProfile,

        [Description("Navigation.Library")]
        NavigationLibrary,

        [Description("Navigation.LogOut")]
        NavigationLogOut,

        [Description("Navigation.MostReadBooks")]
        NavigationMostReadBooks,

        [Description("Navigation.MyProfileDashboard")]
        NavigationMyProfileDashboard,

        [Description("Navigation.PrivacyPolicy")]
        NavigationPrivacyPolicy,

        [Description("Navigation.QuizOpen")]
        NavigationQuizOpen,

        [Description("Navigation.QuizScore")]
        NavigationQuizScore,

        [Description("Navigation.QuizSplashScreen")]
        NavigationQuizSplashScreen,

        [Description("Navigation.ReadProfile")]
        NavigationReadProfile,

        [Description("Navigation.SchoolProfile")]
        NavigationSchoolProfile,

        [Description("Navigation.Search")]
        NavigationSearch,

        [Description("Navigation.SelectAvatar")]
        NavigationSelectAvatar,

        [Description("Navigation.Settings")]
        NavigationSettings,

        [Description("Navigation.SignIn")]
        NavigationSignIn,

        [Description("Navigation.SignUp")]
        NavigationSignUp,

        [Description("Navigation.SubscribeOrJoinAClass")]
        NavigationSubscribeOrJoinAClass,

        [Description("Navigation.TermsAndConditions")]
        NavigationTermsAndConditions,

        [Description("Navigation.Welcome")]
        NavigationWelcome

    }
}

