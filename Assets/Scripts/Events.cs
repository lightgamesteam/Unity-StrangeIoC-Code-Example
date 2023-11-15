public enum EventGlobal
{
    E_None = 0,
    E_AppUpdate,
    E_AppFixedUpdate,
    E_AppLateUpdate,
    E_AppBackButton,

    //UI
    E_ShowScreen,
    E_HideScreen,
    E_ShowBlocker,
    E_HideBlocker,
    E_ScreenManagerBack,
    E_HideCurrentScreen,
    E_HideAllScreens,
    E_ResetTopPanel,
    E_GetHomeworksStatsCommand,
    E_MoveLibraryPanel,
    E_MoveLibraryPanelInversion,
    E_LibraryCategoriesButtonClick,
    E_LibraryCategoryToTopPanel,
    E_HideLibraryPanelToTopPanel,
    E_HideMenuPanelInTopPanel,
    E_UpdateBooksLikeStatus,
    E_ChangeBookDetailsLanguage,
    E_SelectBookDetails,
    E_ChangeBookDetailsCover,
    E_ProcessBooksSearch,
    E_UpdateBooksSearchResults,
    E_ResetChildClasses,
    E_UpdateChildClasses,
    E_UpdateDownloadedBooks,
    E_LocalizationSelected,

    //UI //StartBookButton
    E_UnblockStartButtons,
    E_BlinkTextMesh,

    //UI //Avatar customization
    E_AvatarItemClick,
    E_UserAvatarUpdated,

    //UI //Books grid
    E_BooksGridScrollEnd,
    E_BooksGridSetLoader,
    E_BooksGridRemoveLoader,
    E_BooksGridClearContent,
    E_BooksGridBuildContent,
    E_BooksGridNoMoreBooks,
    E_BooksGridHideNoMoreBooks,
    E_BookDownloadStart,
    E_BookDownloadEnd,
    E_BookLoadProcessEnd,

    //UI //Quiz
    E_QuizPlayNaration,
    E_QuizNarationEnd,
    E_QuizPartChoice,
    E_QuizQuestionResult,
    E_QuizBlocker,
    E_QuizRightAnswer,
    E_QuizShowCorrect,
    E_QuizNextTry,
    E_QuizFadeContent,
    E_QuizType2VisualResult,
    E_QuizType4VisualResult,
    E_QuizType4ClearResults,

    //Asset Bundles
    E_LoadAssetBundle,
    E_DeleteAssetBundle,

    //Network
    E_NetworkCommand,

    //Network //GetChild
    E_GetChildData,
    E_GetChildDataByClassCode,
    E_GetChildDataByFeide,
    E_GetChildDataByPassword,
    E_GetChildDataByDeepLink,

    E_GetTeacherDataByFeide,
    E_GetTeacherDataByPassword,

    //Network //Autorization
    E_StartAutorizationByLogin,
    E_StartAutorizationByFeide,
    E_CheckUsernameCommand,

    //Network //Books
    E_GetBooksCommand,
    E_GetFeaturedBooksCommand,
    E_LoadBooks,
    E_RemoveBookFromDownloadedCommand,

    //Network //Edit child
    E_EditChild,

    //Network //Quiz
    E_ShowQuizSplashScreen,
    E_GetQuizFromServer,
    E_SetQuizResultToServer,
    E_SetLikeForBookToServer,

    //Network //Homeworks
    E_CheckHomeworks,
    E_GetHomeworks,
    E_UpdateHomeworks,

    //Get user country 
    E_GetUserCountryCode,

    //TextToSpeech
    E_TextToSpeech,

    //Support
    E_SendFeedback,
    //Download image/audio
    E_DownloadAudioClip,
    E_DownloadImage,

    //Analytics events
    E_ApplicationQuittedCommand,
    E_QuizStartedCommand,
    E_QuizQuittedCommand,
    E_GetStatsChild,
    E_GetChildClasses,
    E_GetQuizStatsChild,
    E_SetQuizStatsChild,

    //Books statistic events
    E_StatsDownloadBookCommand,
    E_StatsOpenedBookCommand,
    E_StatsFlipPageBookCommand,
    E_StatsWordClickBookCommand,
    E_StatsFinishedBookCommand,
    E_StatsStartTimerAnimatedBookCommand,
    E_StatsStopTimerAnimatedBookCommand,
    E_StatsSwitchActivatingViews,

    //Unity books
    E_DownloadUnityBook,
    E_DownloadUnityBookProgress,
    E_CancelDownloadUnityBook,
    E_DownloadBookCommand,

    //Native books
    E_DownloadNativeBook,
    E_OpenNativeBook,
    E_DeleteNativeBook,
    E_CancelDownloadNativeBook,
    E_ExistsDownloadNativeBook,
    E_CancelNativeBook,
    E_StartNativeBookTracking,

    //All books
    E_FinishedDownloadUnityBook,
    E_DeleteBookForDescription,

    //----Sound events---

    /// <summary>
    /// Play AudioSource
    /// </summary>
    E_SoundPlay,
    /// <summary>
    /// Stop AudioSource
    /// </summary>
    E_SoundStop,
    /// <summary>
    /// Stop All AudioSources
    /// </summary>
    E_SoundStopAll,
    /// <summary>
    /// Stop All AudioSources, but transmitted AudioSource list will continue to play
    /// </summary>
    E_SoundStopNotAll,
    /// <summary>
    /// Mute AudioPacks by type Music
    /// </summary>
    E_SoundMuteMusic,
    /// <summary>
    /// Mute AudioPacks by type Sound
    /// </summary>
    E_SoundMuteSound,
    /// <summary>
    /// Remute AudioPacks by type Music
    /// </summary>
    E_SoundRemuteMusic,
    /// <summary>
    /// Remute AudioPacks by type Sound
    /// </summary>
    E_SoundRemuteSound,
    /// <summary>
    /// Update music volume
    /// </summary>
    E_SoundUpdateMusicVolume,
    /// <summary>
    /// Update sound volume
    /// </summary>
    E_SoundUpdateSoundVolume,
    /// <summary>
    /// Pause audio sources
    /// </summary>
    E_SoundPause,
    /// <summary>
    /// UnPause audio sources
    /// </summary>
    E_SoundUnPause,
    /// <summary>
    /// Pause all audio sources
    /// </summary>
    E_SoundPauseAll,
    /// <summary>
    /// UnPause all audio sources
    /// </summary>
    E_SoundUnPauseAll,
    /// <summary>
    /// Pause sounds type
    /// </summary>
    E_SoundPauseSound,
    /// <summary>
    /// UnPause sounds type
    /// </summary>
    E_SoundUnPauseSound,
    /// <summary>
    /// Pause music type
    /// </summary>
    E_SoundPauseMusic,
    /// <summary>
    /// UnPause music type
    /// </summary>
    E_SoundUnPauseMusic,

    /// <summary>
    /// Play click sound
    /// </summary>
    E_SoundClick,
    /// <summary>
    /// Play main theme music
    /// </summary>
    E_SoundMainTheme,

    //Modes
    E_StartSchoolModeForChildUniversal,

    // Keyboard events
    E_ArrowKeyDown,
    E_TabKeyDown,
    E_EnterKeyDown,

    //DEVELOP
    E_ShowHideLoginButtons,
    E_ResetUser,
    E_ClearUnusedAssetBundles,

    //Manager
    E_InitManager,

    //Localization
    E_Reinitlocalization,

    //CheckSchool
    E_CheckSchoolAccess,

    //purchase
    E_ValidateChildSubscribtion,
    E_ConfirmReceipt,

    E_end
}