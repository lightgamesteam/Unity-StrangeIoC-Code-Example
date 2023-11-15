using PFS.Assets.Scripts.Models.BooksLibraryModels;
using PFS.Assets.Scripts.Models.Requests;
using System;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.Network.BooksLibraryCommands
{
    //top 5 from all recommended books =>> use v4/child/library with perpage 5 , without category filter (or empty)
    //top 5 per each Category =>> v4/child/library with perpage 5 with category filter
    public class GetFeaturedBooks : BaseNetworkCommand
    {
        private GetFeaturedBooksModel getFurtheredBooksModel = null;
        [Inject] public BooksLibrary BooksLibrary { get; set; }

        public override void Execute()
        {
            Retain();

            if (EventData.data == null)
            {
                Debug.LogError("GetFurtheredBooks => data --- NULL");
                Fail();
                return;
            }
            getFurtheredBooksModel = EventData.data as GetFeaturedBooksModel;


            //Create model to send to server
            GetBooksRequestModel getBooksRequestModel = new GetBooksRequestModel();
            getBooksRequestModel.booksRequestType = Conditions.BooksRequestType.FurturedBooks;
            getBooksRequestModel.page = 1;
            getBooksRequestModel.perpage = 5;
            if (!string.IsNullOrEmpty(getFurtheredBooksModel.category.Id))
            {
                getBooksRequestModel.categories.Add(getFurtheredBooksModel.category);
            }
            getBooksRequestModel.requestTrueAction = getFurtheredBooksModel.basicRequestModel.requestTrueAction;
            getBooksRequestModel.requestFalseAction = getFurtheredBooksModel.basicRequestModel.requestFalseAction;

            if (getBooksRequestModel == null)
            {
                Debug.LogError("GetBooksCommand => request --- NULL");
                Fail();
                return;
            }
            BooksLibrary.featuredBooks.Clear();
            //   GetDownloadedBookIdsCommand.NeedBackToUIDownloadedBooks = false;
            Dispatcher.Dispatch(EventGlobal.E_LoadBooks, getBooksRequestModel);
            Release();
        }
    }

    public class GetFeaturedBooksModel
    {
        public BooksCategory category;
        public BasicRequestModel basicRequestModel;

        public GetFeaturedBooksModel(Action successAction, Action failAction)
        {
            basicRequestModel = new BasicRequestModel(successAction, failAction);
        }
        public GetFeaturedBooksModel(BooksCategory category, Action successAction, Action failAction)
        {
            this.category = category;
            basicRequestModel = new BasicRequestModel(successAction, failAction);
        }
    }
}