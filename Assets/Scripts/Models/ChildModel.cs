using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PFS.Assets.Scripts.Models.BooksLibraryModels;

namespace PFS.Assets.Scripts.Models
{
    public class ChildModel
    {
        public static ChildModel Instance { get; set; }

        public string Id { get; set; }
        public string AvatarId { get; set; }
        public string AccountId { get; private set; }
        public string Nickname { get; private set; }
        public string Surname { get; private set; }
        public int Stars { get; private set; }
        public int Coins { get; set; }
        public string Email { get; private set; }
        public string Token { get; private set; }
        public string DefaultLanguage { get; private set; } = "British";
        public string[] AdditionalLanguages { get; private set; }
        public string[] AllowedTranslations { get; private set; }
        public string[] GroupIds { get; private set; }
        public bool IsSubscriptionExpired { get; set; }
        public BooksCategory[] CategoriesForStrategy { get; private set; }
        public UserType UserType { get; private set; } = UserType.Child;

        public ChildModel currentChild;

        //fields for a inside needs
        public bool IsInClass { get => GroupIds != null && GroupIds.Length > 0; }
        public Dictionary<string, List<Conditions.Languages>> DownloadedBookIdsObjects { get; set; } // <id , list of languages>

        public ChildModel()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        [JsonConstructor]
        public ChildModel(string _id, string avatar, string accountId, string nickname, string surname, int stars, int coins, string email, string token, string defaultLanguage, string[] additionalLanguages, string[] allowedTranslations, string[] groupIds, bool isSubscriptionExpired, BooksCategory[] strategyCategories, string userType)
        {
            this.Id = _id;
            this.AvatarId = avatar;
            this.AccountId = accountId;
            this.Nickname = nickname;
            this.Surname = surname;
            this.Stars = stars;
            this.Coins = coins;
            this.Email = email;
            this.Token = token;
            if (!string.IsNullOrEmpty(defaultLanguage))
            {
                Debug.Log("DEFAULT LANGUAGE = " + defaultLanguage);
                this.DefaultLanguage = defaultLanguage;
            }
            else
            {
                Debug.LogError("ChildModel: Server didn't send default language");
            }
            this.AdditionalLanguages = additionalLanguages;
            this.AllowedTranslations = allowedTranslations;
            SetLocalizationByDefaultLanguage(this.DefaultLanguage);
            SetAdditionalLanguages(this.AdditionalLanguages);
            SetAllowedTranslations(this.AllowedTranslations);
            this.GroupIds = groupIds;
            this.IsSubscriptionExpired = isSubscriptionExpired;
            this.CategoriesForStrategy = strategyCategories;
            Enum.TryParse(userType, true, out UserType UserType);
            this.UserType = UserType;
        }

        public void SetChildInfo(ChildModel child)
        {
            Nickname = child.Nickname;
            Surname = child.Surname;
            Email = child.Email;
            Stars = child.Stars;
            Coins = child.Coins;
        }

        public void SetLocalizationByDefaultLanguage(string defaultLanguage)
        {
            if (Enum.TryParse(defaultLanguage, true, out LanguagesModel.DefaultLanguage))
            {
                if (LanguagesModel.SelectedLanguage == Conditions.Languages.None)
                {
                    LanguagesModel.SelectedLanguage = LanguagesModel.DefaultLanguage;
                }
            }
        }

        public void SetAdditionalLanguages(string[] additionalLanguages)
        {
            LanguagesModel.AdditionalLanguage.Clear();
            if (additionalLanguages != null && additionalLanguages.Length > 0)
            {
                foreach (var additionalLanguage in additionalLanguages)
                {
                    if (Enum.TryParse(additionalLanguage, true, out Conditions.Languages languageEnum))
                    {
                        LanguagesModel.AdditionalLanguage.Add(languageEnum);
                    }
                    else
                    {
                        Debug.LogError("ParentModel => SetAdditionalLanguages: Can't recognize language - " + additionalLanguage);
                    }
                }
            }
        }

        public void SetAllowedTranslations(string[] AllowedTranslations)
        {
            LanguagesModel.allowedTranslation.Clear();
            if (AllowedTranslations != null && AllowedTranslations.Length > 0)
            {
                foreach (string translation in AllowedTranslations)
                {
                    if (Enum.TryParse(translation, true, out Conditions.Languages languageEnum))
                    {
                        LanguagesModel.allowedTranslation.Add(languageEnum);
                    }
                    else
                    {
                        Debug.LogError($"{nameof(ChildModel)} => {nameof(SetAllowedTranslations)}: Can't recognize language - " + translation);
                    }
                }
            }
        }

        public ChildModel GetChild(string id)
        {
            if (id == currentChild.Id)
            {
                return currentChild;
            }
            else
            {
                return null;
            }

        }

        public void ReloadCurrentChild(ChildModel getChild)
        {
            currentChild = null;
            currentChild = getChild;
        }

        public bool IsBookLoadedByUser(BookModel book, Conditions.Languages language)
        {
            if (DownloadedBookIdsObjects != null && DownloadedBookIdsObjects.Count > 0 && DownloadedBookIdsObjects.Keys.Contains(book.Id) && DownloadedBookIdsObjects[book.Id].Contains(language))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #region Books categories
        public BooksCategory GetBookCategory(int position)
        {
            return CategoriesForStrategy.FirstOrDefault(category => category.Position == position);
        }

        public BooksCategory GetDefaultCategory()
        {
            var res = CategoriesForStrategy.FirstOrDefault(category => category.IsDefault);
            if (res == null)
            {
                return CategoriesForStrategy.First();
            }
            else
            {
                return res;
            }
        }
        #endregion
    }


    public class ChildStatsModel
    {
        public Stats Stats { get; set; }

        public ChildStatsModel(Stats stats)
        {
            Stats = stats;
        }

        public ChildStatsModel()
        {
        }
    }

    public class Stats
    {
        public int ReadBooks { get; private set; }
        public int HomeworksCount { get; private set; }
        public BookModel[] MostRead { get; private set; }
        public MostClickedWords[] MostClicked { get; private set; }
        public int ReadingTime { get; private set; }

        public Stats(int readBooks, int homeworksCount, BookModel[] mostRead, MostClickedWords[] mostClicked, int readingTime)
        {
            ReadBooks = readBooks;
            HomeworksCount = homeworksCount;
            MostRead = mostRead;
            MostClicked = mostClicked;
            ReadingTime = readingTime;
        }
    }

    public class MostClickedWords
    {
        public string Word { get; private set; }
        public int Count { get; private set; }

        public MostClickedWords(string word, int count)
        {
            Word = word;
            Count = count;
        }
    }

    public enum UserType
    {
        Child = 0,
        Teacher
    }
}