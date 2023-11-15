using Assets.Scripts.Services.Analytics;
//using LightGames.Purchase;
using PFS.Assets.Scripts.Commands.AssetBundles;
using PFS.Assets.Scripts.Commands.Authorization;
using PFS.Assets.Scripts.Commands.BooksLoading;
using PFS.Assets.Scripts.Commands.Download;
using PFS.Assets.Scripts.Commands.Encryption;
using PFS.Assets.Scripts.Commands.Localization;
using PFS.Assets.Scripts.Commands.Network;
using PFS.Assets.Scripts.Commands.Network.Authorization;
using PFS.Assets.Scripts.Commands.Network.BooksLibraryCommands;
using PFS.Assets.Scripts.Commands.Network.Statistics;
using PFS.Assets.Scripts.Commands.ScreenManagerCommands;
using PFS.Assets.Scripts.Commands.SoundManagerCommands;
using PFS.Assets.Scripts.Commands.StartGame;
using PFS.Assets.Scripts.Commands.UI;
using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.Authorization;
using PFS.Assets.Scripts.Models.BooksLibraryModels;
using PFS.Assets.Scripts.Models.Pool;
using PFS.Assets.Scripts.Models.Responses;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Models.SoundManagerModels;
using PFS.Assets.Scripts.Models.Statistics;
using PFS.Assets.Scripts.Views.Avatar;
using PFS.Assets.Scripts.Views.BookPage;
using PFS.Assets.Scripts.Views.BooksGrid;
using PFS.Assets.Scripts.Views.Library;
using PFS.Assets.Scripts.Views.BooksSearch;
using PFS.Assets.Scripts.Views.Buttons;
using PFS.Assets.Scripts.Views.DebugScreen;
using PFS.Assets.Scripts.Views.Homeworks;
using PFS.Assets.Scripts.Views.LoadingScreen;
using PFS.Assets.Scripts.Views.MainMenu;
using PFS.Assets.Scripts.Views.Pool;
using PFS.Assets.Scripts.Views.Popups;
using PFS.Assets.Scripts.Views.MyProfile;
using PFS.Assets.Scripts.Views.Quizzes;
using PFS.Assets.Scripts.Views.QuizStats;
using PFS.Assets.Scripts.Views.Login;
using PFS.Assets.Scripts.Views.Sounds;
using PFS.Assets.Scripts.Views.SplashScreen;
using PFS.Assets.Scripts.Views.SwitchScreen;
using PFS.Assets.Scripts.Views.TopPanel;
using strange.extensions.command.api;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using UnityEngine;
using PFS.Assets.Scripts.Views.BookDetails;
using PFS.Assets.Scripts.Views.BookLoading;
using PFS.Assets.Scripts.Views.DownloadedBooks;
using PFS.Assets.Scripts.Views.Purchase;
using PFS.Assets.Scripts.Views.Components;
using PFS.Assets.Scripts.Services.Localization;

public class MainContextRoot : MVCSContext
{
    public MainContextRoot(MonoBehaviour contextView) : base(contextView, ContextStartupFlags.MANUAL_MAPPING)
    {
    }

    // CoreComponents
    protected override void AddCoreComponents()
    {
        base.AddCoreComponents();
        injectionBinder.Unbind<ICommandBinder>(); //Unbind to avoid a conflict!
        injectionBinder.Bind<ICommandBinder>().To<EventCommandBinder>().ToSingleton();

        injectionBinder.Bind<IExecutor>().To<CoroutineExecutor>().ToSingleton();
        injectionBinder.Bind<SoundManagerModel>().ToSingleton();
        injectionBinder.Bind<ScreenManager>().ToSingleton();
        injectionBinder.Bind<BooksLibrary>().ToSingleton();
        injectionBinder.Bind<ChildModel>().ToSingleton();
        injectionBinder.Bind<PoolModel>().ToSingleton();
//        injectionBinder.Bind<ShopModel>().ToSingleton();
        injectionBinder.Bind<LocalizationManager>().ToSingleton();
        injectionBinder.Bind<QuizStatisticsModel>().ToSingleton();
        injectionBinder.Bind<ChildStatsModel>().ToSingleton();
        injectionBinder.Bind<DeepLinkModel>().ToSingleton();
        injectionBinder.Bind<Analytics>().ToSingleton();
        injectionBinder.Bind<AppVersionsModel>().ToSingleton();
        injectionBinder.Bind<IPlayerPrefsStrategy>().To<StraightPlayerPrefsStrategy>().ToSingleton().ToName("Straight");
        injectionBinder.Bind<IPlayerPrefsStrategy>().To<EncryptedPlayerPrefsStrategy>().ToSingleton().ToName("Encrypted");
        injectionBinder.Bind<IAnalyticsService>().To<AmplitudeAnalytics>().ToSingleton().ToName(AnalyticsServices.Amplitude);

       
    }

    // Commands and Bindings
    protected override void MapBindings()
    {
        base.MapBindings();


        //-----TopPanel screen
        mediationBinder.BindView<UITopPanelScreenView>().ToMediator<UITopPanelScreenMediator>();
        mediationBinder.BindView<UIMenuPanelView>().ToMediator<UIMenuPanelMediator>();
        mediationBinder.BindView<UISearchPanelView>().ToMediator<UISearchPanelMediator>();
        mediationBinder.BindView<UICategoryPanelView>().ToMediator<UICategoryPanelMediator>();
        mediationBinder.BindView<StageItemView>().ToMediator<StageItemMediator>();
        mediationBinder.BindView<LanguageItemView>().ToMediator<LanguageItemMediator>();
        mediationBinder.BindView<InterestItemView>().ToMediator<InterestItemMediator>();
        mediationBinder.BindView<MenuLanguageButtonView>().ToMediator<MenuLanguageButtonMediator>();
        //--------------------

        //-----Home/MainMenu screen
        mediationBinder.BindView<UIMainMenuView>().ToMediator<UIMainMenuMediator>();
        mediationBinder.BindView<UIBooksCategoryScrollView>().ToMediator<UIBooksCategoryScrollMediator>();
        mediationBinder.BindView<UIBooksCategoryScrollsView>().ToMediator<UIBooksCategoryScrollsMediator>();
        //-------------------------

        //-----MyProfile screen
        mediationBinder.BindView<UIMyProfileView>().ToMediator<UIMyProfileMediator>();
        mediationBinder.BindView<UIQuizStatsView>().ToMediator<UIQuizStatsMediator>();
        mediationBinder.BindView<UIHomeworkStatisticsView>().ToMediator<UIHomeworkStatisticsMediator>();
        mediationBinder.BindView<UIChooseAvatarPopupView>().ToMediator<UIChooseAvatarPopupMediator>();
        mediationBinder.BindView<UIAvatarItemView>().ToMediator<UIAvatarItemMediator>();
        mediationBinder.BindView<UIClassInfoItemView>().ToMediator<UIClassInfoItemMediator>();
        //---------------------

        //-----Library screen
        mediationBinder.BindView<UIBookView>().ToMediator<UIBookMediator>();
        mediationBinder.BindView<UILibraryView>().ToMediator<UILibraryMediator>();
        mediationBinder.BindView<UILibraryButtonView>().ToMediator<UILibraryButtonMediator>();
        mediationBinder.BindView<UINavigationLibraryView>().ToMediator<UINavigationLibraryMediator>();
        mediationBinder.BindView<UIMovePanelView>().ToMediator<UIMovePanelMediator>();
        mediationBinder.BindView<UISwipePanelView>().ToMediator<UISwipePanelMediator>();
        //-------------------

        //-----BookDetail screen
        mediationBinder.BindView<UIBookDetailsView>().ToMediator<UIBookDetailsMediator>();
        mediationBinder.BindView<UIBookDetailsItemView>().ToMediator<UIBookDetailsItemMediator>();
        mediationBinder.BindView<UIBookDetailsInfoView>().ToMediator<UIBookDetailsInfoMediator>();
        mediationBinder.BindView<UIMoreInfoButtonView>().ToMediator<UIMoreInfoButtonMediator>();
        //---------------------

        //-----BookLoading screen
        mediationBinder.BindView<UIBookLoadingView>().ToMediator<UIBookLoadingMediator>();
        //-------------------------

        //-----Animated and Song books screen
        mediationBinder.BindView<UIBookPageView>().ToMediator<UIBookPageMediator>();
        //-----------------------------------

        //-----Debug screen
        mediationBinder.BindView<DebugView>().ToMediator<DebugMediator>();
        //-----------------

        //-----Login
        mediationBinder.BindView<UILoginScreenView>().ToMediator<UILoginScreenMediator>();
        mediationBinder.BindView<UIJoinClassScreenView>().ToMediator<UIJoinClassScreenMediator>();
        //----------------------

        //-----SerchResult screen
        mediationBinder.BindView<UIBooksSearchResultView>().ToMediator<UIBooksSearchResultMediator>();
        //-----------------------

        //-----UISwitchScreensAnimation screen
        mediationBinder.BindView<UISwitchScreensAnimationView>().ToMediator<UISwitchScreensAnimationMediator>();
        //------------------------------------

        //-----DownloadedBooks screen
        mediationBinder.BindView<UIDownloadedBooksView>().ToMediator<UIDownloadedBooksMediator>();
        mediationBinder.BindView<UIFavoriteBooksView>().ToMediator<UIFavoriteBooksMediator>();
        //---------------------------

        //-----Splash screen
        mediationBinder.BindView<UISplashScreenView>().ToMediator<UISplashScreenMediator>();
        //------------------

        //-----Loading screen
        mediationBinder.BindView<UILoadingScreenView>().ToMediator<UILoadingScreenMediator>();
        //-----

        //-----Quizzes
        mediationBinder.BindView<UIQuizView>().ToMediator<UIQuizMediator>();
        mediationBinder.BindView<UIQuizSplashScreenView>().ToMediator<UIQuizSplashScreenMediator>();
        mediationBinder.BindView<UIQuizQuestionPartContentView>().ToMediator<UIQuizQuestionPartContentMediator>();
        mediationBinder.BindView<UIQuizQuestionBaseView>().ToMediator<UIQuizQuestionBaseMediator>();
        mediationBinder.BindView<UIQuizQuestionType1View>().ToMediator<UIQuizQuestionType1Mediator>();
        mediationBinder.BindView<UIQuizQuestionType2View>().ToMediator<UIQuizQuestionType2Mediator>();
        mediationBinder.BindView<UIQuizQuestionType3View>().ToMediator<UIQuizQuestionType3Mediator>();
        mediationBinder.BindView<UIQuizQuestionType4View>().ToMediator<UIQuizQuestionType4Mediator>();
        mediationBinder.BindView<UIQuizQuestionType5View>().ToMediator<UIQuizQuestionType5Mediator>();
        mediationBinder.BindView<UIQuizQuestionPartBaseView>().ToMediator<UIQuizQuestionPartBaseMediator>();
        mediationBinder.BindView<UIQuizQuestionPartType1View>().ToMediator<UIQuizQuestionPartType1Mediator>();
        mediationBinder.BindView<UIQuizQuestionPartType2View>().ToMediator<UIQuizQuestionPartType2Mediator>();
        mediationBinder.BindView<UIQuizQuestionPartType3View>().ToMediator<UIQuizQuestionPartType3Mediator>();
        mediationBinder.BindView<UIQuizQuestionPartType4View>().ToMediator<UIQuizQuestionPartType4Mediator>();
        mediationBinder.BindView<UIQuizQuestionPartType5View>().ToMediator<UIQuizQuestionPartType5Mediator>();
        mediationBinder.BindView<UIFinalQuizPopupView>().ToMediator<UIFinalQuizPopupMediator>();
        mediationBinder.BindView<UIQuizConveyorRoadView>().ToMediator<UIQuizConveyorRoadMediator>();
        mediationBinder.BindView<UIQuizProgressView>().ToMediator<UIQuizProgressMediator>();
        mediationBinder.BindView<UIQuizPausePopupView>().ToMediator<UIQuizPausePopupMediator>();
        //------------

        //-----Homeworks
        mediationBinder.BindView<UINewHomeworkItemView>().ToMediator<UINewHomeworkItemMediator>();
        mediationBinder.BindView<UIDoneHomeworkItemView>().ToMediator<UIDoneHomeworkItemMediator>();
        mediationBinder.BindView<UINewHomeworksScrollView>().ToMediator<UINewHomeworksScrollMediator>();
        mediationBinder.BindView<UIDoneHomeworksScrollView>().ToMediator<UIDoneHomeworksScrollMediator>();
        mediationBinder.BindView<UIDetailedHomeworkView>().ToMediator<UIDetailedHomeworkMediator>();
        mediationBinder.BindView<UIHomeworksView>().ToMediator<UIHomeworksMediator>();
        //--------------

        //-----Managers
        mediationBinder.BindView<SoundManagerView>().ToMediator<SoundManagerMediator>();
        mediationBinder.BindView<PoolObjectsView>().ToMediator<PoolObjectsMediator>();
        //-------------

        //-----Purchase
        //mediationBinder.BindView<PurchaserView>().ToMediator<PurchaserMediator>();
        mediationBinder.BindView<UISubscriptionView>().ToMediator<UISubscriptionMediator>();
        //-------------

        //-----Popups
        mediationBinder.BindView<UISettingsPopupView>().ToMediator<UISettingsPopupMediator>();
        mediationBinder.BindView<UIErrorPopupView>().ToMediator<UIErrorPopupMediator>();
        mediationBinder.BindView<UIUniversalPopupView>().ToMediator<UIUniversalPopupMediator>();
        mediationBinder.BindView<UIConnectToClassPopupView>().ToMediator<UIConnectToClassPopupMediator>();
        mediationBinder.BindView<UIFeedbackPopupView>().ToMediator<UIFeedbackPopupMediator>();
        //-----------

        //-----Components
        mediationBinder.BindView<UIBooksGridLoadingView>().ToMediator<UIBooksGridLoadingMediator>(); //base view of all grids view 
        mediationBinder.BindView<UIBooksGridView>().ToMediator<UIBooksGridMediator>();
        mediationBinder.BindView<UIBooksGridItemView>().ToMediator<UIBooksGridItemMediator>();
        mediationBinder.BindView<UIControlContentSizeView>().ToMediator<UIControlContentSizeMediator>();
        mediationBinder.BindView<UILoaderView>().ToMediator<UILoaderMediator>();
        mediationBinder.BindView<UIChangeLanguageButtonView>().ToMediator<UIChangeLanguageButtonMediator>();
        mediationBinder.BindView<UIFeaturedBooksItemView>().ToMediator<UIFeaturedBooksItemMediator>();
        mediationBinder.BindView<StartBookButtonView>().ToMediator<StartBookButtonMediator>();
        mediationBinder.BindView<UIDropdownItemView>().ToMediator<UIDropdownItemMediator>();
        mediationBinder.BindView<UILikeButtonView>().ToMediator<UILikeButtonMediator>();
        mediationBinder.BindView<UIDeleteButtonView>().ToMediator<UIDeleteButtonMediator>();
        //---------------



        //System Commands
        commandBinder.Bind(ContextEvent.START)
#if DEVELOP
        .To<OpenDebugScreenCommand>()
#endif
        .To<CheckEncryptionCommand>()
        .To<AppStartedCommand>()
        .To<DeepLinkCommand>()
        .To<DeepLinkCommandByFeide>()
        .To<StartGameCommand>()
        .To<ReinitLocalizationCommand>()
        .To<CheckAppVersionCommand>()
        .To<CheckDownloadDateBooksCommand>()
        .InSequence()
        .Once();

        //Localization
        commandBinder.Bind(EventGlobal.E_Reinitlocalization).To<ReinitLocalizationCommand>();

        //UI
        commandBinder.Bind(EventGlobal.E_ShowScreen).To<UISwitchScreensAnimationCommand>().To<UIScreenShowCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_HideScreen).To<UISwitchScreensAnimationCommand>().To<UIScreenHideCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_ShowBlocker).To<UIBlockerShowCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_HideBlocker).To<UIBlockerHideCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_ScreenManagerBack).To<ScreenManagerBackCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_HideCurrentScreen).To<ScreenManagerDeleteCurrentScreen>().Pooled();
        commandBinder.Bind(EventGlobal.E_HideAllScreens).To<ScreenManagerDeleteAllScreen>().Pooled();

        //Asset bundles
        commandBinder.Bind(EventGlobal.E_LoadAssetBundle).To<CheckAssetBundle>().To<DownloadAssetBundle>().To<LoadAssetBundle>().Pooled().InSequence();
        commandBinder.Bind(EventGlobal.E_ClearUnusedAssetBundles).To<ClearUnusedAssetsCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_DeleteAssetBundle).To<DeleteAssetBundle>().Pooled();

        //Load unity books
        commandBinder.Bind(EventGlobal.E_DownloadUnityBook).To<LoadUnityBookCommand>().Pooled().InSequence();

        //Load Other prefabs to sceen
        commandBinder.Bind(EventGlobal.E_InitManager).To<InitManagerCommand>();

        //Neetwork 
        commandBinder.Bind(EventGlobal.E_NetworkCommand).To<NetworkCommand>().Pooled();

        commandBinder.Bind(EventGlobal.E_SendFeedback).To<SendFeedbackCommand>();
        commandBinder.Bind(EventGlobal.E_AppBackButton).To<BackButtonCommand>().Pooled();

        //Get Child
        commandBinder.Bind(EventGlobal.E_GetChildData).            To<GetChildDataCommand>().   To<CheckDeepLinkUserTokenCommand>().To<InitializeCategoriesCommand>().To<GetDownloadedBookIdsCommand>().To<LoadButtonAndTitleImagesCommand>().InSequence();
        commandBinder.Bind(EventGlobal.E_GetChildDataByDeepLink).  To<GetChildDataByDeepLink>().To<CheckDeepLinkUserTokenCommand>().To<InitializeCategoriesCommand>().To<GetDownloadedBookIdsCommand>().To<LoadButtonAndTitleImagesCommand>().InSequence();
        commandBinder.Bind(EventGlobal.E_GetChildDataByFeide).     To<GetChildDataByFeideCommand>().                                To<InitializeCategoriesCommand>().To<GetDownloadedBookIdsCommand>().To<LoadButtonAndTitleImagesCommand>().InSequence();
        commandBinder.Bind(EventGlobal.E_GetChildDataByPassword).  To<GetChildDataByPasswordCommand>().                             To<InitializeCategoriesCommand>().To<GetDownloadedBookIdsCommand>().To<LoadButtonAndTitleImagesCommand>().InSequence();
        commandBinder.Bind(EventGlobal.E_GetTeacherDataByPassword).To<GetTeacherDataByPasswordCommand>().                           To<InitializeCategoriesCommand>().To<GetDownloadedBookIdsCommand>().To<LoadButtonAndTitleImagesCommand>().InSequence();
        commandBinder.Bind(EventGlobal.E_GetChildDataByClassCode). To<UIBlockerShowCommand>().To<GetChildDataByClassCode>().        To<InitializeCategoriesCommand>().To<GetDownloadedBookIdsCommand>().To<LoadButtonAndTitleImagesCommand>().To<UIBlockerHideCommand>().InSequence();
        commandBinder.Bind(EventGlobal.E_EditChild).To<EditChildCommand>().Pooled();

        commandBinder.Bind(EventGlobal.E_TextToSpeech).To<TextToSpeechCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_GetUserCountryCode).To<GetUserCountryCodeCommand>().Pooled();

        //Autorization
        commandBinder.Bind(EventGlobal.E_StartAutorizationByLogin).To<UIBlockerShowCommand>().To<StartAutorizationByLoginCommand>().To<UIBlockerHideCommand>().InSequence();
        commandBinder.Bind(EventGlobal.E_StartAutorizationByFeide).To<UIBlockerShowCommand>().To<GetFeideLinkCommand>().To<StartAutorizationByFeideCommand>().To<UIBlockerHideCommand>().InSequence();
        commandBinder.Bind(EventGlobal.E_CheckUsernameCommand).To<UIBlockerShowCommand>().To<CheckUsernameCommand>().To<UIBlockerHideCommand>().InSequence();

        //Purchase
        commandBinder.Bind(EventGlobal.E_ConfirmReceipt).To<ConfirmPurchaseCommand>();

        //books
        commandBinder.Bind(EventGlobal.E_GetBooksCommand).To<GetBooksCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_GetFeaturedBooksCommand).To<GetFeaturedBooks>();
        commandBinder.Bind(EventGlobal.E_LoadBooks).To<LoadBooksCommand>().Pooled().InSequence();
        commandBinder.Bind(EventGlobal.E_RemoveBookFromDownloadedCommand).To<RemoveBookFromDownloadedCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_StartNativeBookTracking).To<NativeBookTrackingCommand>().Pooled();

        commandBinder.Bind(EventGlobal.E_StatsDownloadBookCommand).To<DownloadBookCommand>().To<GetDownloadedBookIdsCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_StatsOpenedBookCommand).To<OpenedBookCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_StatsFlipPageBookCommand).To<FlipPageBookCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_StatsWordClickBookCommand).To<WordClickBookCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_StatsFinishedBookCommand).To<FinishedBookCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_SetLikeForBookToServer).To<SetLikeForBookCommand>().Pooled();

        //Statistics
        commandBinder.Bind(EventGlobal.E_ApplicationQuittedCommand).To<AppQuittedCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_QuizStartedCommand).To<QuizStartedCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_QuizQuittedCommand).To<QuizQuittedCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_GetStatsChild).To<GetMyProfileCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_GetQuizStatsChild).To<GetQuizStatisticsCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_GetChildClasses).To<GetChildClassesCommand>().Pooled();

        //Quizzes
        commandBinder.Bind(EventGlobal.E_ShowQuizSplashScreen).To<UIShowQuizSplashCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_GetQuizFromServer).To<GetQuizCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_SetQuizResultToServer).To<UIBlockerShowCommand>().To<SetQuizResultCommand>().To<UIBlockerHideCommand>().Pooled().InSequence();

        //Homeworks
        commandBinder.Bind(EventGlobal.E_CheckHomeworks).To<CheckHomeworksCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_GetHomeworks).To<GetHomeworksCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_GetHomeworksStatsCommand).To<GetHomeworksStatsCommand>().Pooled();

        //Downloas Audio/Image universal commands
        commandBinder.Bind(EventGlobal.E_DownloadAudioClip).To<DownloadAudioClipCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_DownloadImage).To<DownloadImageCommand>().Pooled();

        //Sounds
        commandBinder.Bind(EventGlobal.E_SoundPlay).To<SoundPlayCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_SoundStop).To<SoundStopCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_SoundStopAll).To<SoundStopAllCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_SoundStopNotAll).To<SoundStopNotAllCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_SoundMuteMusic).To<SoundMuteMusicCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_SoundMuteSound).To<SoundMuteSoundCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_SoundRemuteMusic).To<SoundRemuteMusicCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_SoundRemuteSound).To<SoundRemuteSoundCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_SoundUpdateMusicVolume).To<SoundUpdateMusicVolumeCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_SoundUpdateSoundVolume).To<SoundUpdateSoundVolumeCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_SoundPause).To<SoundPauseCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_SoundUnPause).To<SoundUnPauseCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_SoundPauseAll).To<SoundPauseAllCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_SoundUnPauseAll).To<SoundUnPauseAllCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_SoundPauseSound).To<SoundPauseSoundCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_SoundUnPauseSound).To<SoundUnPauseSoundCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_SoundPauseMusic).To<SoundPauseMusicCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_SoundUnPauseMusic).To<SoundUnPauseMusicCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_SoundClick).To<SoundClickPlayCommand>().Pooled();
        commandBinder.Bind(EventGlobal.E_SoundMainTheme).To<SoundPlayMainCommand>().Pooled();

        //Modes
        commandBinder.Bind(EventGlobal.E_StartSchoolModeForChildUniversal).To<StartSchoolModeForChildUniversalCommand>();

        //CheckAcces
        commandBinder.Bind(EventGlobal.E_CheckSchoolAccess).To<CheckSchoolAccessCommand>();
        commandBinder.Bind(EventGlobal.E_ValidateChildSubscribtion).To<ValidateChildAppleSubscribtionCommand>();

        //reset user
        commandBinder.Bind(EventGlobal.E_ResetUser).To<DebugResetUserCommand>();
    }
}