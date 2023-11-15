using Assets.Scripts.Services.Analytics;
using BooksPlayer;
using Conditions;
using PFS.Assets.Scripts.Commands;
using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.BooksLibraryModels;
using PFS.Assets.Scripts.Models.Requests;
using strangeBooksPlayer.extensions.dispatcher.eventdispatcher.api;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NativeBookTrackingCommand : BaseCommand
{
    [Inject] public Analytics Analytics { get; set; }

    public override void Execute()
    {
        Retain();
        Debug.Log(nameof(NativeBookTrackingCommand));
        SubscribeOnEvents();
    }

    private void SubscribeOnEvents()
    {
        BooksPlayerMainContextView.AddListenerStrangeEvent(ExternalTrackingEvents.BookStartDownload, BookStartDownloadEvent);
        BooksPlayerMainContextView.AddListenerStrangeEvent(ExternalTrackingEvents.BookDownloaded, BookDownloadedEvent);
        BooksPlayerMainContextView.AddListenerStrangeEvent(ExternalTrackingEvents.BookDeleted, BookDeletedEvent);
        BooksPlayerMainContextView.AddListenerStrangeEvent(ExternalTrackingEvents.BookStarted, BookStartedEvent);

        BooksPlayerMainContextView.AddListenerStrangeEvent(ExternalTrackingEvents.BookClosed, BookClosedTrackingEvent);
        BooksPlayerMainContextView.AddListenerStrangeEvent(ExternalTrackingEvents.PageFlipped, PageFlippedEvent);
        BooksPlayerMainContextView.AddListenerStrangeEvent(ExternalTrackingEvents.NarrationWordClicked, NarrationWordClickedEvent);
        BooksPlayerMainContextView.AddListenerStrangeEvent(ExternalTrackingEvents.ClickableObjectClicked, ClickableObjectClickedEvent);
        BooksPlayerMainContextView.AddListenerStrangeEvent(ExternalTrackingEvents.ReadAgain, ReadAgainTrackingEvent);
        BooksPlayerMainContextView.AddListenerStrangeEvent(ExternalTrackingEvents.BackToBooks, BackToBooksTrackingEvent);

        BooksPlayerMainContextView.AddListenerStrangeEvent(ExternalTrackingEvents.BookSpeed, StatBookReadSpeedEvent);
        BooksPlayerMainContextView.AddListenerStrangeEvent(ExternalTrackingEvents.BookPlayStatus, StatBookPausePlayEvent);
        BooksPlayerMainContextView.AddListenerStrangeEvent(ExternalTrackingEvents.BookDownloadError, StatBookDownloadErrorEvent);
        BooksPlayerMainContextView.AddListenerStrangeEvent(ExternalTrackingEvents.BookFullTextPanelStatus, StatTextZoomViewEvent);
        BooksPlayerMainContextView.AddListenerStrangeEvent(ExternalTrackingEvents.PageBackGroundMusicStatus, StatBookMuteEvent);
        BooksPlayerMainContextView.AddListenerStrangeEvent(ExternalTrackingEvents.FullTextPanelMode, UserChangeThemeEvent);
        BooksPlayerMainContextView.AddListenerStrangeEvent(ExternalTrackingEvents.TextSizeOnFullTextPanel, EnlargeTextSizeOnTextZoomViewEvent);
        BooksPlayerMainContextView.AddListenerStrangeEvent(ExternalEvents.CloseBooksPlayer, CloseBooksPlayer);
    }

    private void BookStartDownloadEvent(IEventBooksPlayer e)
    {
        if (e.data == null)
        {
            Debug.LogError("UBP Tracking => BookStartDownloadEvent - data null");
            return;
        }

        BookStartDownloadTrackingModel model = e.data as BookStartDownloadTrackingModel;
        BookModel bookModel = model.BookModel as BookModel;

        Analytics.LogEvent(EventName.ActionBookDownload,
            new Dictionary<Property, object>()
            {
                { Property.Uuid, PlayerPrefsModel.CurrentChildId},
                { Property.ISBN, bookModel.GetTranslation().Isbn},
                { Property.Category, bookModel.GetInterests()},
                { Property.BookId, model.BookId}
            });
    }

    private void BookDownloadedEvent(IEventBooksPlayer e)
    {
        if (e.data == null)
        {
            Debug.LogError("UBP Tracking => BookDownloadedEvent - data null");
            return;
        }

        BookDownloadedTrackingModel model = e.data as BookDownloadedTrackingModel;

        Debug.Log($"UBP Tracking => BookDownloadedEvent => book id - {model.BookId}");

        BookModel bookModel = model.BookModel as BookModel;
        if (bookModel == null)
        {
            bookModel = new BookModel();
        }

        Dispatcher.Dispatch(EventGlobal.E_StatsDownloadBookCommand, new DownloadedOpenedBookRequestModel(bookId: model.BookId,
                                                                                            homeworkId: bookModel.HomeworkId,
                                                                                            languageName: bookModel.CurrentTranslation.ToDescription(),
                                                                                            requestTrueAction: () => Debug.Log("Book download - done"),
                                                                                            requstFalseAction: () => Debug.LogError("Book download - fail"),
                                                                                            waitResponse: true));

        Analytics.LogEvent(EventName.ActionBookDownloadFinish,
            new Dictionary<Property, object>()
            {
                { Property.Uuid, PlayerPrefsModel.CurrentChildId},
                { Property.BookId, bookModel.Id },
                { Property.ISBN, bookModel.GetTranslation().Isbn},
                { Property.Category, bookModel.GetInterests()}
            });
    }

    private void BookDeletedEvent(IEventBooksPlayer e)
    {
        if (e.data == null)
        {
            Debug.LogError("UBP Tracking => BookDeletedEvent - data null");
            return;
        }

        BookDeletedTrackingModel model = e.data as BookDeletedTrackingModel;

        Debug.Log($"UBP Tracking => BookDeletedEvent => book id - {model.BookId}");
    }

    private void BookStartedEvent(IEventBooksPlayer e)
    {
        if (e.data == null)
        {
            Debug.LogError("UBP Tracking => PlayBookEvent - data null");
            return;
        }

        BookStartedTrackingModel model = e.data as BookStartedTrackingModel;

        Debug.Log($"UBP Tracking => PlayBookEvent => book id - {model.BookId} | book regime - {model.BookRegime}");

        BookModel bookModel = model.BookModel as BookModel;
        if (bookModel == null)
        {
            bookModel = new BookModel();
        }

        Dispatcher.Dispatch(EventGlobal.E_StatsOpenedBookCommand,
            new DownloadedOpenedBookRequestModel(bookId: model.BookId,
                                                 languageName: bookModel.CurrentTranslation.ToDescription(),
                                                 homeworkId: bookModel.HomeworkId,
                                                 requestTrueAction: () => Debug.Log("Book opened - done"),
                                                 requstFalseAction: () => Debug.LogError("Book opened - fail"),
                                                 waitResponse: true));
    }

    private void BookClosedTrackingEvent(IEventBooksPlayer e)
    {
        if (e.data == null)
        {
            Debug.LogError("UBP Tracking => BookClosedTrackingEvent - data null");
            return;
        }

        BookClosedTrackingModel model = e.data as BookClosedTrackingModel;
        BookModel bookModel = model.BookModel as BookModel;

        double allPages = model.TotalPages;
        double seenPagesPercent = Math.Round(model.PagePosition / allPages * 100, 3);

        Analytics.LogEvent(EventName.ActionBookClosePercentageSeen,
            new Dictionary<Property, object>()
            {
                { Property.ISBN, bookModel.GetTranslation().Isbn},
                { Property.Category, bookModel.GetInterests()},
                { Property.PageSeenCount, model.PagePosition },
                { Property.Seen, seenPagesPercent + " % "},
                { Property.Uuid, PlayerPrefsModel.CurrentChildId},
                { Property.ReadingTimeS, model.BookTime}
            });
    }

    private void PageFlippedEvent(IEventBooksPlayer e)
    {
        if (e.data == null)
        {
            Debug.LogError("UBP Tracking => PageFlippedEvent - data null");
            return;
        }

        PageFlippedTrackingModel model = e.data as PageFlippedTrackingModel;
        BookModel bookModel = model.BookModel as BookModel;
        if (bookModel == null)
        {
            bookModel = new BookModel();
        }

        Analytics.LogEvent(EventName.ActionBookPageTurn,
            new Dictionary<Property, object>()
            {
                { Property.ISBN, bookModel.GetTranslation().Isbn},
                { Property.Category, bookModel.GetInterests()},
                { Property.Uuid, PlayerPrefsModel.CurrentChildId}
            });

        Debug.Log($"UBP Tracking => PageFlippedEvent => book id - {model.BookId} | page position - {model.PagePosition} | total pages - {model.TotalPages} | page time - {model.PageTime}");

        int timeOnPage = (int)Mathf.Round(model.PageTime / 1000f);

        Dispatcher.Dispatch(EventGlobal.E_StatsFlipPageBookCommand,
            new FlipPageBookRequestModel(bookId: model.BookId,
                                         pageNo: model.PagePosition + 1,
                                         pagewords: 20,
                                         totalPages: model.TotalPages,
                                         timeOnPage: timeOnPage,
                                         homeworkId: bookModel.HomeworkId,
                                         requestTrueAction: () => Debug.Log("Page flipped - done"),
                                         requstFalseAction: () => Debug.LogError("Page flipped - fail"),
                                         waitResponse: true));
    }

    private void NarrationWordClickedEvent(IEventBooksPlayer e)
    {
        if (e.data == null)
        {
            Debug.LogError("UBP Tracking => NarrationWordClickedEvent - data null");
            return;
        }

        NarrationWordClickedTrackingModel model = e.data as NarrationWordClickedTrackingModel;

        Debug.Log($"UBP Tracking => NarrationWordClickedEvent => book id - {model.BookId} | page position - {model.PagePosition} | word - {model.NarrationWord}");

        BookModel bookModel = model.BookModel as BookModel;
        if (bookModel == null)
        {
            bookModel = new BookModel();
        }

        Dispatcher.Dispatch(EventGlobal.E_StatsWordClickBookCommand,
            new WordClickBookRequestModel(bookId: model.BookId,
                                          pageNo: model.PagePosition + 1,
                                          word: model.NarrationWord,
                                          homeworkId: bookModel.HomeworkId,
                                          requestTrueAction: () => Debug.Log("Word clicked - done"),
                                          requstFalseAction: () => Debug.LogError("Word clicked - fail"),
                                          waitResponse: true));


        Analytics.LogEvent(EventName.ActionBookCallout,
            new Dictionary<Property, object>()
            {
                { Property.Uuid, PlayerPrefsModel.CurrentChildId},
                { Property.ISBN, bookModel.GetTranslation().Isbn},
                { Property.Category, bookModel.GetInterests()}
            });
    }

    private void ClickableObjectClickedEvent(IEventBooksPlayer e)
    {
        if (e.data == null)
        {
            Debug.LogError("UBP Tracking => ClickableObjectClickedEvent - data null");
            return;
        }

        ClickableObjectClickedTrackingModel model = e.data as ClickableObjectClickedTrackingModel;

        Debug.Log($"UBP Tracking => ClickableObjectClickedEvent => book id - {model.BookId} | page position - {model.PagePosition} | total pages - {model.TotalPages} | word - {model.ClickableObjectWord}");

        BookModel bookModel = model.BookModel as BookModel;
        if (bookModel == null)
        {
            bookModel = new BookModel();
        }

        Dispatcher.Dispatch(EventGlobal.E_StatsWordClickBookCommand,
            new WordClickBookRequestModel(bookId: model.BookId,
                                          pageNo: model.PagePosition + 1,
                                          word: model.ClickableObjectWord,
                                          homeworkId: bookModel.HomeworkId,
                                          requestTrueAction: () => Debug.Log("Word clicked - done"),
                                          requstFalseAction: () => Debug.LogError("Word clicked - fail"),
                                          waitResponse: true));

        Analytics.LogEvent(EventName.ActionBookCallout,
            new Dictionary<Property, object>()
            {
                { Property.Uuid, PlayerPrefsModel.CurrentChildId},
                { Property.ISBN, bookModel.GetTranslation().Isbn},
                { Property.Category, bookModel.GetInterests()}
            });
    }

    private void ReadAgainTrackingEvent(IEventBooksPlayer e)
    {
        if (e.data == null)
        {
            Debug.LogError("UBP Tracking => ReadAgainTrackingEvent - data null");
            return;
        }
        ReadAgainTrackingModel model = e.data as ReadAgainTrackingModel;
        BookModel bookModel = model.BookModel as BookModel;

        if (bookModel == null)
        {
            bookModel = new BookModel();
        }

        Analytics.LogEvent(EventName.ActionBookReadAgain,
            new Dictionary<Property, object>()
            {
                { Property.Uuid, PlayerPrefsModel.CurrentChildId},
                { Property.ISBN, bookModel.GetTranslation().Isbn},
                { Property.Category, bookModel.GetInterests()}
            });

        Debug.Log($"UBP Tracking => ReadAgainTrackingEvent => book id - {model.BookId}");
    }

    private void BackToBooksTrackingEvent(IEventBooksPlayer e)
    {
        if (e.data == null)
        {
            Debug.LogError("UBP Tracking => BackToBooksTrackingEvent - data null");
            return;
        }

        BackToBooksTrackingModel model = e.data as BackToBooksTrackingModel;
        BookModel bookModel = model.BookModel as BookModel;

        if (bookModel == null)
        {
            bookModel = new BookModel();
        }

        Debug.Log($"UBP Tracking => BackToBooksTrackingEvent => book id - {model.BookId}");

        Analytics.LogEvent(EventName.ActionBackToBooks,
            new Dictionary<Property, object>()
            {
                { Property.ISBN, bookModel.GetTranslation().Isbn},
                { Property.Category, bookModel.GetInterests()},
                { Property.Uuid, PlayerPrefsModel.CurrentChildId}
            });
    }

    private void StatBookReadSpeedEvent(IEventBooksPlayer e)
    {
        if (e.data == null)
        {
            Debug.LogError("UBP Tracking => StatBookReadSpeedEvent - data null");
            return;
        }

        BookSpeedTrackingModel model = e.data as BookSpeedTrackingModel;

        BookModel bookModel = model.BookModel as BookModel;
        if (bookModel == null)
        {
            bookModel = new BookModel();
        }

        Debug.Log($"UBP Tracking => StatBookReadSpeedEvent => book id - {model.BookId} | book speed - {model.Speed}");

        Analytics.LogEvent(EventName.ActionBookReadSpeed,
            new Dictionary<Property, object>()
            {
                { Property.ISBN, bookModel.GetTranslation().Isbn},
                { Property.Category, bookModel.GetInterests()},
                { Property.Uuid, PlayerPrefsModel.CurrentChildId},
                { Property.BookReadSpeed, model.Speed.ToDescription()}
            });
    }

    private void StatBookPausePlayEvent(IEventBooksPlayer e)
    {
        Debug.Log("PAUSE---------------------------------");
        if (e.data == null)
        {
            Debug.LogError("UBP Tracking => StatBookPausePlayEvent - data null");
            return;
        }

        BookPlayStatusTrackingModel model = e.data as BookPlayStatusTrackingModel;
        BookModel bookModel = model.BookModel as BookModel;
        if (bookModel == null)
        {
            bookModel = new BookModel();
        }

        Debug.Log($"UBP Tracking => StatBookPausePlayEvent => book id - {model.BookId} | book play - {model.IsPlay}");

        EventName actionName = EventName.ActionBookPlay;
        if (!model.IsPlay)
        {
            actionName = EventName.ActionBookPause;
        }

        Analytics.LogEvent(actionName,
            new Dictionary<Property, object>()
            {
                { Property.ISBN, bookModel.GetTranslation().Isbn},
                { Property.Category, bookModel.GetInterests()},
                { Property.Uuid, PlayerPrefsModel.CurrentChildId}
            });
    }

    private void StatBookDownloadErrorEvent(IEventBooksPlayer e)
    {
        if (e.data == null)
        {
            Debug.LogError("UBP Tracking => StatBookDownloadErrorEvent - data null");
            return;
        }

        BookDownloadErrorTrackingModel model = e.data as BookDownloadErrorTrackingModel;
        BookModel bookModel = model.BookModel as BookModel;
        if (bookModel == null)
        {
            bookModel = new BookModel();
        }

        Debug.Log($"UBP Tracking => StatBookDownloadErrorEvent => book id - {model.BookId} | book error - {model.ErrorMessage}");

        Analytics.LogEvent(EventName.ActionBookDownloadError,
            new Dictionary<Property, object>()
            {
                { Property.ISBN, bookModel.GetTranslation().Isbn},
                { Property.Category, bookModel.GetInterests()},
                { Property.BookId, bookModel.Id},
                { Property.Uuid, PlayerPrefsModel.CurrentChildId}
            });
    }

    private void StatTextZoomViewEvent(IEventBooksPlayer e)
    {
        if (e.data == null)
        {
            Debug.LogError("UBP Tracking => StatTextZoomViewEvent - data null");
            return;
        }

        BookFullTextPanelStatusTrackingModel model = e.data as BookFullTextPanelStatusTrackingModel;
        BookModel bookModel = model.BookModel as BookModel;
        if (bookModel == null)
        {
            bookModel = new BookModel();
        }

        Debug.Log($"UBP Tracking => StatTextZoomViewEvent => book id - {model.BookId} | book full panel is opened - {model.IsOpened}");

        EventName actionName = EventName.ActionTextZoomView;
        if (!model.IsOpened)
        {
            actionName = EventName.ActionCloseTextZoomView;
        }

        Analytics.LogEvent(actionName,
            new Dictionary<Property, object>()
            {
                { Property.ISBN, bookModel.GetTranslation().Isbn},
                { Property.Category, bookModel.GetInterests()},
                { Property.Uuid, PlayerPrefsModel.CurrentChildId}
            });
    }

    private void StatBookMuteEvent(IEventBooksPlayer e)
    {
        if (e.data == null)
        {
            Debug.LogError("UBP Tracking => StatBookMuteEvent - data null");
            return;
        }

        PageBackGroundMusicStatusTrackingModel model = e.data as PageBackGroundMusicStatusTrackingModel;
        BookModel bookModel = model.BookModel as BookModel;
        if (bookModel == null)
        {
            bookModel = new BookModel();
        }

        Debug.Log($"UBP Tracking => StatBookMuteEvent => book id - {model.BookId} | book bg music play - {model.IsPlay}");

        EventName actionName = EventName.ActionBookMute;
        if (model.IsPlay)
        {
            actionName = EventName.ActionBookUnmute;
        }

        Analytics.LogEvent(actionName,
            new Dictionary<Property, object>()
            {
                { Property.ISBN, bookModel.GetTranslation().Isbn},
                { Property.Category, bookModel.GetInterests()},
                { Property.Uuid, PlayerPrefsModel.CurrentChildId}
            });
    }

    private void UserChangeThemeEvent(IEventBooksPlayer e)
    {
        if (e.data == null)
        {
            Debug.LogError("UBP Tracking => UserChangeThemeEvent - data null");
            return;
        }

        FullTextPanelModeTrackingModel model = e.data as FullTextPanelModeTrackingModel;
        BookModel bookModel = model.BookModel as BookModel;
        if (bookModel == null)
        {
            bookModel = new BookModel();
        }

        Debug.Log($"UBP Tracking => UserChangeThemeEvent => book id - {model.BookId} | book full panel night mode - {model.IsNightMode}");

        Analytics.LogEvent(EventName.ActionOnUserChangeTheme,
            new Dictionary<Property, object>()
            {
                { Property.ISBN, bookModel.GetTranslation().Isbn},
                { Property.Category, bookModel.GetInterests()},
                { Property.Uuid, PlayerPrefsModel.CurrentChildId},
                { Property.Theme, model.IsNightMode ? "Night" : "Day" }
            });
    }

    private void EnlargeTextSizeOnTextZoomViewEvent(IEventBooksPlayer e)
    {
        if (e.data == null)
        {
            Debug.LogError("UBP Tracking => EnlargeTextSizeOnTextZoomViewEvent - data null");
            return;
        }

        TextSizeOnFullTextPanelTrackingModel model = e.data as TextSizeOnFullTextPanelTrackingModel;
        BookModel bookModel = model.BookModel as BookModel;
        if (bookModel == null)
        {
            bookModel = new BookModel();
        }

        Debug.Log($"UBP Tracking => EnlargeTextSizeOnTextZoomViewEvent => book id - {model.BookId} | book full panel text size - {model.TextSize}");

        EventName actionName = EventName.ActionEnlargeTextSizeOnTextZoomView;

        if (model.TextSize == BookFullPanelTextSize.Small)
        {
            actionName = EventName.ActionReduceTextSizeOnTextZoomView;
        }

        Analytics.LogEvent(actionName,
            new Dictionary<Property, object>()
            {
                { Property.ISBN, bookModel.GetTranslation().Isbn},
                { Property.Category, bookModel.GetInterests()},
                { Property.Uuid, PlayerPrefsModel.CurrentChildId}
            });
    }

    private void CloseBooksPlayer()
    {
        UnsubscribeEvents();
        Release();
    }

    public void UnsubscribeEvents()
    {
        BooksPlayerMainContextView.RemoveListenerStrangeEvent(ExternalTrackingEvents.BookStartDownload, BookStartDownloadEvent);
        BooksPlayerMainContextView.RemoveListenerStrangeEvent(ExternalTrackingEvents.BookDownloaded, BookDownloadedEvent);
        BooksPlayerMainContextView.RemoveListenerStrangeEvent(ExternalTrackingEvents.BookDeleted, BookDeletedEvent);
        BooksPlayerMainContextView.RemoveListenerStrangeEvent(ExternalTrackingEvents.BookStarted, BookStartedEvent);

        BooksPlayerMainContextView.RemoveListenerStrangeEvent(ExternalTrackingEvents.BookClosed, BookClosedTrackingEvent);
        BooksPlayerMainContextView.RemoveListenerStrangeEvent(ExternalTrackingEvents.PageFlipped, PageFlippedEvent);
        BooksPlayerMainContextView.RemoveListenerStrangeEvent(ExternalTrackingEvents.NarrationWordClicked, NarrationWordClickedEvent);
        BooksPlayerMainContextView.RemoveListenerStrangeEvent(ExternalTrackingEvents.ClickableObjectClicked, ClickableObjectClickedEvent);
        BooksPlayerMainContextView.RemoveListenerStrangeEvent(ExternalTrackingEvents.ReadAgain, ReadAgainTrackingEvent);
        BooksPlayerMainContextView.RemoveListenerStrangeEvent(ExternalTrackingEvents.BackToBooks, BackToBooksTrackingEvent);

        BooksPlayerMainContextView.RemoveListenerStrangeEvent(ExternalTrackingEvents.BookSpeed, StatBookReadSpeedEvent);
        BooksPlayerMainContextView.RemoveListenerStrangeEvent(ExternalTrackingEvents.BookPlayStatus, StatBookPausePlayEvent);
        BooksPlayerMainContextView.RemoveListenerStrangeEvent(ExternalTrackingEvents.BookDownloadError, StatBookDownloadErrorEvent);
        BooksPlayerMainContextView.RemoveListenerStrangeEvent(ExternalTrackingEvents.BookFullTextPanelStatus, StatTextZoomViewEvent);
        BooksPlayerMainContextView.RemoveListenerStrangeEvent(ExternalTrackingEvents.PageBackGroundMusicStatus, StatBookMuteEvent);
        BooksPlayerMainContextView.RemoveListenerStrangeEvent(ExternalTrackingEvents.FullTextPanelMode, UserChangeThemeEvent);
        BooksPlayerMainContextView.RemoveListenerStrangeEvent(ExternalTrackingEvents.TextSizeOnFullTextPanel, EnlargeTextSizeOnTextZoomViewEvent);
    }
}

