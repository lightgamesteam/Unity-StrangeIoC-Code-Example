using PFS.Assets.Scripts.Models.BooksLibraryModels;
using PFS.Assets.Scripts.Models.Requests;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.Network.BooksLibraryCommands
{
    public class LoadBooksCommand : BaseNetworkCommand
    {
        private GetBooksRequestModel request;

        public override void Execute()
        {
            Retain();
            if (EventData.data == null)
            {
                Debug.LogError("LoadBooksCommand => data --- NULL");
                Fail();
                return;
            }

            request = EventData.data as GetBooksRequestModel;
            if (request == null)
            {
                Debug.LogError("LoadBooksCommand => request --- NULL");
                Fail();
                return;
            }

            ConvertEnums();

            Dispatcher.Dispatch(EventGlobal.E_GetBooksCommand, request);

            Release();
        }

        private void ConvertEnums()
        {
            request.categoryIds = GetCategoriesIds(request.categories);
            request.simplifiedLevels = ConverEnumToStringArray(request.simplifiedLevelsEnums);
            request.languages = ConverEnumToStringArray(request.languagesEnums);
        }

        private string[] ConverEnumToStringArray<T>(List<T> value) where T : Enum
        {
            string[] res = new string[value.Count];

            for (int i = 0; i < value.Count; i++)
            {
                res[i] = value[i].ToDescription();
            }

            return res;
        }

        private string[] GetCategoriesIds(List<BooksCategory> categories)
        {
            List<string> ids = new List<string>();

            foreach (var item in categories)
            {
                ids.Add(item.Id);
            }

            return ids.ToArray();
        }
    }
}