using PFS.Assets.Scripts.Models.BooksLibraryModels;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace PFS.Assets.Scripts.Commands.BooksLoading
{
    public class LoadUnityBookCommand : BaseCommand
    {
        [Inject]
        public IExecutor Coroutine { get; private set; }

        private BookModel book;
        private BookModel.Translation translation;

        private UnityWebRequest webRequest;
        private bool aborted;

        public override void Execute()
        {
            Retain();
            Debug.Log("LoadUnityBookCommand");

            if (EventData.data == null)
            {
                Debug.LogError("LoadUnityBookCommand data --- error");
                Fail();
                return;
            }

            book = EventData.data as BookModel;
            translation = book.GetTranslation();

            Dispatcher.AddListener(EventGlobal.E_CancelDownloadUnityBook, ProcessUnityBookDownloadCancel);

            Coroutine.Execute(LoadBook());
        }

        public override void Release()
        {
            Dispatcher.RemoveListener(EventGlobal.E_CancelDownloadUnityBook, ProcessUnityBookDownloadCancel);

            base.Release();
        }

        private IEnumerator LoadBook()
        {
#if UNITY_EDITOR
            string address = translation.UrlAndroid;
#elif UNITY_IOS
        string address = translation.UrlIos;
#elif UNITY_ANDROID
        string address = translation.UrlAndroid;
#elif UNITY_STANDALONE_WIN || UNITY_WSA
        string address = translation.UrlWindows;
#elif UNITY_STANDALONE_OSX
        string address = translation.UrlMacOsX;
#endif
            DownloadUnityBookProcess downloadProcess = new DownloadUnityBookProcess { bookId = book.Id, downloadProcess = 0 };
            webRequest = UnityWebRequest.Get(address);
            webRequest.SendWebRequest();
            while (!webRequest.isDone)
            {
                downloadProcess.downloadProcess = webRequest.downloadProgress;
                Dispatcher.Dispatch(EventGlobal.E_DownloadUnityBookProgress, downloadProcess);
                yield return null;
            }

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                if (aborted)
                {
                    Debug.Log($"{GetType()} => Download Aborted");
                }
                else
                {
                    Debug.LogError(webRequest.error);
                }
            }
            else
            {
                byte[] bytes = webRequest.downloadHandler.data;
                File.WriteAllBytes(LoadBooksHelper.GetFullPath(book.Id, book.GetTranslation().CountryCode), bytes);
                LoadBooksHelper.SaveBook(book);
            }
            Release();
        }

        private void ProcessUnityBookDownloadCancel()
        {
            aborted = true;
            webRequest.Abort();
        }
    }
}