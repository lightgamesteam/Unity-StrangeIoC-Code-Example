using Newtonsoft.Json;
using PFS.Assets.Scripts.Commands.Download;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PFS.Assets.Scripts.Models.BooksLibraryModels
{
    public class BooksCategory
    {
        public string Id { get; private set; }
        public string TechnicalName { get; private set; }
        public int Position { get; set; }
        public string ButtonImageUrl { get; private set; } = string.Empty;
        public string TitleImageUrl { get; private set; } = string.Empty;
        public bool IsDefault { get; private set; }
        public Dictionary<string, string> Translations { get; private set; }

        public Sprite ButtonImage { get; private set; }
        public Sprite TitleImage { get; private set; }

        public BooksCategory()
        { }

        [JsonConstructor]
        public BooksCategory(string _id, string name, string image, string largeImage, bool @default, Dictionary<string, string> translations)
        {
            this.Id = _id;
            this.TechnicalName = name;
            this.ButtonImageUrl = image;
            this.TitleImageUrl = largeImage;
            this.IsDefault = @default;
            this.Translations = translations;
        }

        #region Category images methods
        public void SetImageForButton(Image buttonImage)
        {
            if (ButtonImage)
            {
                buttonImage.sprite = ButtonImage;
            }
            else
            {
                CoroutineExecutor.instance.Execute(WaitForImageLoaded(buttonImage));
            }
        }

        private IEnumerator WaitForImageLoaded(Image buttonImage)
        {
            yield return new WaitUntil(() => ButtonImage != null);
            if (buttonImage != null)
            {
                buttonImage.sprite = ButtonImage;
            }
        }

        public void SetImageForTitle(Image titleImage)
        {
            if (TitleImage)
            {
                titleImage.sprite = TitleImage;
            }
            else
            {
                CoroutineExecutor.instance.Execute(WaitForTitleLoaded(titleImage));
            }
        }

        private IEnumerator WaitForTitleLoaded(Image titleImage)
        {
            yield return new WaitUntil(() => TitleImage != null);
            if (titleImage != null)
            {
                titleImage.sprite = TitleImage;
            }
        }

        public void DownloadTitleImageForPool()
        {
            if (!TitleImage)
            {
                Action<Sprite> setSpriteAction = resultSprite =>
                {
                    TitleImage = resultSprite;
                };

                Action failAction = () =>
                {
                    Debug.LogError($"Can't download title for category {TechnicalName}");
                };

                MainContextView.DispatchStrangeEvent(EventGlobal.E_DownloadImage, new DownloadImageParams(TitleImageUrl, setSpriteAction, failAction));
            }
        }


        public void DownloadButtonImageForPool()
        {
            if (!ButtonImage)
            {
                Action<Sprite> setSpriteAction = resultSprite =>
                {
                    ButtonImage = resultSprite;
                };

                Action failAction = () =>
                {
                    Debug.LogError($"Can't download button for category {TechnicalName}");
                };

                MainContextView.DispatchStrangeEvent(EventGlobal.E_DownloadImage, new DownloadImageParams(ButtonImageUrl, setSpriteAction, failAction));
            }
        }
        #endregion

        public string GetCategoryName()
        {
            string language = LanguagesModel.SelectedLanguage.ToDescription();
            if (Translations.ContainsKey(language))
            {
                return Translations[language];
            }

            return Translations.FirstOrDefault().Value;
        }
    }
}