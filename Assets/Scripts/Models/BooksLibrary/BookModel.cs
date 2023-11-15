using BooksPlayer;
using Conditions;
using Newtonsoft.Json;
using PFS.Assets.Scripts.Commands.BooksLoading;
using PFS.Assets.Scripts.Commands.Download;
using PFS.Assets.Scripts.Models.Pool;
using PFS.Assets.Scripts.Views.DebugScreen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace PFS.Assets.Scripts.Models.BooksLibraryModels
{
    public class BookModel
    {
        #region Fields and properties from server

        /// <summary>
        /// Book identifier in the collection. Server has few collection for ex. Liked collection, Downloaded collection, PublishedBooks collection(this is the gloabal collection)
        /// </summary>
        private string BookId { get; }

        /// <summary>
        /// Identifier in the PublishedBook collection. 
        /// It means that book with PublishedBookId = "911" in the PublishedBook collection has _id = "911" but in current collection it can be _id = "123". 
        /// So it's like a link to the book in the PublishedBooks collection.
        /// </summary>
        private string PublishedBookId { get; }

        /// <summary>
        /// Original book name (in English)
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Book type ("regularbook"/"animatedBooks"/"songBook"/"newspaper")
        /// </summary>
        private string Type { get; }

        /// <summary>
        /// Difficult level of book ("stage1"/"stage2"/"stage3"/"stage4"/"stage5"/"stage6"/"stage7"/"stage8")
        /// </summary>
        private string SimplifiedLevel { get; }

        /// <summary>
        /// Categories applied to the book
        /// </summary>
        private BookCategory[] CategoriesIds { get; }

        /// <summary>
        /// Identifier of Quiz applied to this book
        /// </summary>
        private readonly string quizId;

        /// <summary>
        /// Count of visible and invisible questions in quize for each language
        /// </summary>
        private Dictionary<string, QuizQuestionCounter> QuizCounter { get; }

        /// <summary>
        /// List of translations (<Language>: <object with translations>)
        /// </summary>
        private Dictionary<string, Translation> Translations { get; }

        /// <summary>
        /// Number identifier that numerate all books on the Pickatale server 
        /// This is a link that unites translations of one book in BCT. 
        /// At the same time it is an identifier for the Macedonians, by which they understand what kind of book it is between the systems
        /// </summary>
        public string Reference { get; private set; }
        /// <summary>
        /// Is Book liked
        /// </summary>
        public bool IsLiked { get; set; }
        #endregion

        /// <summary>
        /// return "PublishedBookId" if it's not empty anotherway return "BookId".
        /// if PublishedBookId is empty that means that this book comes to us from PublishedBooks collection and _id = PublishedBookId. And we dont need PublishedBookId in this case.
        /// </summary>
        public string Id { get => string.IsNullOrEmpty(PublishedBookId) ? BookId : PublishedBookId; }
        public string QuizId { get => IsAvailableQuiz(DebugView.QuizQuestionsType, QuizCounter) ? quizId : null; }
        public bool IsUnityBook { get => TypeEnum == BookContentType.Animated || TypeEnum == BookContentType.Song; }
        public BookContentType TypeEnum { get; private set; } = BookContentType.Native;
        public SimplifiedLevels SimplifiedLevelEnum { get; private set; }
        public Languages CurrentTranslation { get; set; }
        public List<Languages> AvailibleTranslations { get; private set; } = new List<Languages>();
        /// <summary>
        /// Get or set download state for a current book translation 
        /// </summary>
        public bool IsDownloaded { get => GetTranslation().IsDownloaded; set => GetTranslation().IsDownloaded = value; }
        public bool ReadOnly { get; set; }
        public string HomeworkId { get; set; }
        /// <summary>
        /// Qize can be hided in homwork for specific book
        /// </summary>
        public bool HideQuiz { get; set; } = false;

        public bool IsBookFromSearch { get; set; } = false;
        public bool IsBookFromMostRead { get; set; } = false;
        /// <summary>
        /// Link to books library where the book placed
        /// </summary>
        public Books BooksCollection { get; private set; }

        public BookModel()
        { }

        [JsonConstructor]
        public BookModel(string _id, string publishedBookId, string reference, string name, string type, string simplified_level, Dictionary<string, Translation> translations, bool liked, string quizId, Dictionary<string, QuizQuestionCounter> quizCounter, BookCategory[] categoryIds)
        {
            this.BookId = _id;
            this.PublishedBookId = publishedBookId;
            this.Reference = reference;
            this.Name = name;
            this.Type = type;
            this.SimplifiedLevel = simplified_level;
            this.Translations = translations;
            this.IsLiked = liked;
            this.quizId = quizId;
            this.QuizCounter = quizCounter;
            this.CategoriesIds = categoryIds;
        }

        public bool IsAllowedLanguageBook()
        {
            bool bookIsAllowed = false;
            foreach (Languages language in LanguagesModel.allowedTranslation)
            {
                if (this.AvailibleTranslations.Contains(language))
                {
                    bookIsAllowed = true;
                }
            }
            return bookIsAllowed;
        }

        public void InitializeBook(Languages forcedTranslationLanguage = Languages.None)
        {
            //-----------this part needed because server in his side added prefix "UNITY" and "BCT" to book reference, but iOS plugin expect only integer number 
            Reference = Reference.Replace("UNITY-", "");
            Reference = Reference.Replace("BCT-", "");
            //------------

            InitializeType();
            InitializeCountryCodes();
            InitializeSimplifiedLevel();
            InitializeAvailibleTranslationLanguages();
            InitializeCurrentTranslationLanguage(forcedTranslationLanguage);
            InitializeDownloadedTranslations();
        }

        #region Initializing
        private void InitializeType()
        {
            if (Type == BookContentType.Native.ToDescription())
            {
                TypeEnum = BookContentType.Native;
            }
            else if (Type == BookContentType.Animated.ToDescription())
            {
                TypeEnum = BookContentType.Animated;
            }
            else if (Type == BookContentType.Song.ToDescription())
            {
                TypeEnum = BookContentType.Song;
            }
            else if (Type == BookContentType.Newspaper.ToDescription())
            {
                TypeEnum = BookContentType.Native;
            }
            else
            {
                Debug.LogError("BookModel => InitializeType: Can't recognize type =" + Type);
            }
        }

        private void InitializeCountryCodes()
        {
            if (Translations != null)
            {
                foreach (var tr in Translations)
                {
                    if (tr.Value != null)
                    {
                        tr.Value.SetCountryCode(tr.Key);
                    }
                }
            }
        }

        private void InitializeSimplifiedLevel()
        {
            if (string.IsNullOrEmpty(SimplifiedLevel) || SimplifiedLevel == "normal")
            {
                SimplifiedLevelEnum = SimplifiedLevels.None;
            }
            else
            {
                string c = SimplifiedLevel.Substring(SimplifiedLevel.Length - 1, 1);
                bool res = int.TryParse(c, out int simplifiedPos);

                if (!res)
                {
                    SimplifiedLevelEnum = SimplifiedLevels.None;
                }
                else
                {
                    SimplifiedLevelEnum = (SimplifiedLevels)(simplifiedPos - 1);
                }
            }
        }

        private void InitializeAvailibleTranslationLanguages()
        {
            AvailibleTranslations.Clear();
            foreach (var translation in this.Translations)
            {
                if (Enum.TryParse(translation.Key, true, out Languages language))
                {
                    AvailibleTranslations.Add(language);
                }
            }
        }

        public void InitializeCurrentTranslationLanguage(Languages forcedTranslationLanguage = Languages.None)
        {
            Languages globalTranslation = forcedTranslationLanguage == Languages.None ? LanguagesModel.SelectedLanguage : forcedTranslationLanguage;
            if (AvailibleTranslations.Contains(globalTranslation))
            {
                CurrentTranslation = globalTranslation;
            }
            else
            {
                //Select language from allowed and available and nonGlobal
                CurrentTranslation = LanguagesModel.allowedTranslation.FirstOrDefault(y => y != globalTranslation && AvailibleTranslations.Contains(y));
            }
        }

        private void InitializeDownloadedTranslations()
        {
            foreach (var translation in Translations)
            {
                ChildModel child = ChildModel.Instance.GetChild(PlayerPrefsModel.CurrentChildId);
                Enum.TryParse(translation.Key, true, out Languages language);

                if (translation.Value != null)
                {
                    if (TypeEnum == BookContentType.Animated || TypeEnum == BookContentType.Song)
                    {
                        translation.Value.IsDownloaded = false;

                        string path = GetDirectoryPath(translation.Value);
                        if (Directory.Exists(path))//check phisically on device
                        {
                            if (File.Exists(path + LoadBooksHelper.GetBundleName()))
                            {
                                if (child.IsBookLoadedByUser(this, language)) //check also on server for this user
                                {
                                    translation.Value.IsDownloaded = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (BooksPlayerExternal.IsBookExists(new InitializeBookModel(id: this.Id, bookName: translation.Value.BookName, PFSforUBP.GetBookLanguage(translation.Value.CountryCode))) //check phisically on device
                            && child.IsBookLoadedByUser(this, language)) //check also on server for this user
                        {
                            translation.Value.IsDownloaded = true;
                        }
                    }
                }
                else
                {
                    Debug.LogErrorFormat($"BookModel => InitializeDownloadedTranslations: Book - { Name }, dont have a translation object for language - {translation.Key}");
                }
            }
        }
        #endregion

        public void SetLibraryBooksLink(Books booksLink)
        {
            if (BooksCollection == null)
            {
                if (booksLink == null)
                {
                    BooksCollection = new Books();
                }
                else
                {
                    BooksCollection = booksLink;
                }
            }
        }

        public Translation GetTranslation()
        {
            if (Translations == null)
            {
                Debug.LogErrorFormat("BookModel => GetTranslation: Book name: {1}. Book id: {2} has no any translation. <<Translations == null>>", Name, Id);
                return new Translation();
            }

            if (CurrentTranslation.ToDescription() == null)
            {
                Debug.LogErrorFormat("BookModel => GetTranslation: CurrentTranslation.ToDescription() is NULL");
                return new Translation();
            }

            if (Translations.ContainsKey(CurrentTranslation.ToDescription()) == false)
            {
                Debug.LogErrorFormat("BookModel => GetTranslation: No {0} translation. Book name: {1}. Book id: {2}", CurrentTranslation.ToDescription(), Name, Id);
                return new Translation();
            }

            if (Translations[CurrentTranslation.ToDescription()] == null)
            {
                Debug.LogErrorFormat("BookModel => GetTranslation: No object with {0} translation. Book name: {1}. Book id: {2}. <<Translations[CurrentTranslation.ToDescription()] == null>>", CurrentTranslation.ToDescription(), Name, Id);
                return new Translation();
            }
            return Translations[CurrentTranslation.ToDescription()];
        }

        public string GetInterests()
        {
            List<BooksCategory> allCategoriesAvailibleForChild = ChildModel.Instance.GetChild(PlayerPrefsModel.CurrentChildId)?.CategoriesForStrategy.ToList();

            if (allCategoriesAvailibleForChild.Count > 0 && CategoriesIds.Length > 0)
            {
                List<BookCategory> categoriesFromBookAndAvailibleForChild = new List<BookCategory>();
                foreach (var categoriesAvailibleForChild in allCategoriesAvailibleForChild)
                {
                    //get category id's from this book availible for child //NOTE: book can have categories not avalible for child
                    var availibleCategory = CategoriesIds?.ToList().Find(c => c.id == categoriesAvailibleForChild.Id);
                    if (availibleCategory != null)
                    {
                        categoriesFromBookAndAvailibleForChild.Add(availibleCategory);
                    }
                    else
                    {
                        Debug.LogWarning($"BookModel => GetInterests => Book don't have a category = {categoriesAvailibleForChild.TechnicalName}");
                    }
                }

                string res = string.Empty;
                if (categoriesFromBookAndAvailibleForChild.Count > 0)
                {
                    for (int i = 0; i < categoriesFromBookAndAvailibleForChild.Count; i++)
                    {
                        if (categoriesFromBookAndAvailibleForChild[i].categoryTransaltions.ContainsKey(CurrentTranslation.ToDescription())
                            && !string.IsNullOrEmpty(categoriesFromBookAndAvailibleForChild[i].categoryTransaltions[CurrentTranslation.ToDescription()]))
                        {
                            res += categoriesFromBookAndAvailibleForChild[i].categoryTransaltions[CurrentTranslation.ToDescription()];
                            if (i + 1 != categoriesFromBookAndAvailibleForChild.Count)
                            {
                                res += ", ";
                            }
                        }
                    }
                }

                return res;
            }

            return string.Empty;
        }

        private bool IsAvailableQuiz(QuizQuestionsType type, Dictionary<string, QuizQuestionCounter> quizCounter)
        {
            if (HideQuiz || quizCounter == null)
            {
                return false;
            }

            var counterByLanguage = quizCounter.ContainsKey(CurrentTranslation.ToDescription()) ? quizCounter[CurrentTranslation.ToDescription()] : new QuizQuestionCounter { visible = 0, invisible = 0 };

            if (type == QuizQuestionsType.Visible)
            {
                return counterByLanguage.visible > 0;
            }
            else if (type == QuizQuestionsType.Invisible)
            {
                return counterByLanguage.invisible > 0;
            }
            else
            {
                return counterByLanguage.visible > 0 || counterByLanguage.invisible > 0;
            }
        }

        private string GetDirectoryPath(Translation translation)
        {
            return LoadBooksHelper.GetDirectoryPath(Id, translation.CountryCode);
        }


        #region Images loading
        public void LoadCoverImage(System.Action<Sprite> finalAction)
        {
            System.Action<Sprite, string> action = (sprite, id) =>
            {
                finalAction?.Invoke(sprite);
            };
            LoadImage(action, null, true);
        }

        public void LoadCoverImage(System.Action<Sprite> finalAction, System.Action finalActionFail)
        {
            System.Action<Sprite, string> action = (sprite, id) =>
            {
                finalAction?.Invoke(sprite);
            };
            LoadImage(action, finalActionFail, true);
        }

        public void LoadBackgroundImage(System.Action<Sprite> finalAction)
        {
            System.Action<Sprite, string> action = (sprite, id) =>
            {
                finalAction?.Invoke(sprite);
            };
            LoadImage(action, null, false);
        }

        public void LoadBackgroundImage(System.Action<Sprite> finalAction, System.Action finalActionFail)
        {
            System.Action<Sprite, string> action = (sprite, id) =>
            {
                finalAction?.Invoke(sprite);
            };
            LoadImage(action, finalActionFail, false);
        }

        /// <summary>
        /// Sprite - book sprite | string - book id
        /// </summary>
        /// <param name="finalAction"></param>
        public void LoadCoverImage(System.Action<Sprite, string> finalAction)
        {
            LoadImage(finalAction, null, true);
        }

        public void LoadCoverImage(System.Action<Sprite, string> finalAction, System.Action finalActionFail)
        {
            LoadImage(finalAction, finalActionFail, true);
        }

        /// <summary>
        /// Sprite - book sprite | string - book id
        /// </summary>
        /// <param name="finalAction"></param>
        public void LoadBackgroundImage(System.Action<Sprite, string> finalAction)
        {
            LoadImage(finalAction, null, false);
        }

        public void LoadBackgroundImage(System.Action<Sprite, string> finalAction, System.Action finalActionFail)
        {
            LoadImage(finalAction, finalActionFail, false);
        }

        private void LoadImage(System.Action<Sprite, string> finalAction, System.Action finalActionFail, bool cover)
        {
            Translation tr = GetTranslation();

            string url = cover ? tr.UrlFlatCoverImg : tr.UrlBackgroundCover;

            Sprite sprite = cover ? tr.coverSpritePool : tr.backgroundCoverSpritePool;

            if (sprite != null)
            {
                finalAction?.Invoke(sprite, Id);
                return;
            }

            if (string.IsNullOrEmpty(url))
            {
                finalActionFail?.Invoke();
                return;
            }

            System.Action<Sprite> setImageAction = resultSprite =>
            {
                if (!resultSprite.texture.IsDownloadImageError())
                {
                    if (cover)
                    {
                        tr.coverSpritePool = resultSprite;
                    }
                    else
                    {
                        tr.backgroundCoverSpritePool = resultSprite;
                    }

                    finalAction?.Invoke(cover ? tr.coverSpritePool : tr.backgroundCoverSpritePool, Id);
                }
                else
                {
                    if (cover)
                    {
                    //tr.coverSpritePool = PoolModel.Instance.bookCoverDefault;
                }
                    else
                    {
                        tr.backgroundCoverSpritePool = PoolModel.Instance.BookBackgroundDefault;
                    }

                    finalActionFail?.Invoke();
                }
            };
            string address = url.RemoveWhiteSpace();
            MainContextView.DispatchStrangeEvent(EventGlobal.E_DownloadImage, new DownloadImageParams(address, setImageAction, finalActionFail));
        }
        #endregion

        public class BookCategory
        {
            [JsonProperty("_id")]
            public string id;
            public string name;
            public Dictionary<string, string> categoryTransaltions = new Dictionary<string, string>();
        }

        public class Translation
        {
            //short description of the book
            public string DescriptionShort { get; private set; }

            //the book writer
            public string Writer { get; private set; }

            //translatad book name 
            public string BookName { get; private set; }

            //link to the archive with images
            public string UrlCommunFile { get; private set; }

            //link to the archive with sounds 
            public string UrlLanguageFile { get; private set; }

            //link to small icon
            public string UrlFlatCoverImg { get; private set; }

            //ios asset bundle
            public string UrlIos { get; private set; }

            //android asset bundle
            public string UrlAndroid { get; private set; }

            //Windows asset bundle
            public string UrlWindows { get; private set; }

            //MacOsX asset bundle
            public string UrlMacOsX { get; private set; }

            //book info
            public string Atos { get; private set; }

            //book info
            public string Lexile { get; private set; }

            //international book id
            public string Isbn { get; private set; }

            //author of illustration
            public string Illustrator { get; private set; }

            //author of narration
            public string Narrator { get; private set; }

            //short country code
            public string CountryCode { get; private set; } = "EN";

            public string UrlBackgroundCover { get; private set; }

            public bool IsDownloaded { get; set; }

            public Sprite coverSpritePool;

            public Sprite backgroundCoverSpritePool;



            public Translation()
            { }

            [JsonConstructor]
            public Translation(string description_short, string writer, string book_name, string url_commun_file, string url_language_file,
                               string url_flat_cover_img, string url_windows, string url_macosx, string url_ios, string url_android, string atos, string lexile, string isbn, string illustrator, string narrator, string background_cover)
            {
                this.DescriptionShort = description_short;
                this.Writer = writer;
                this.BookName = book_name;
                this.UrlCommunFile = url_commun_file;
                this.UrlLanguageFile = url_language_file;
                this.UrlFlatCoverImg = url_flat_cover_img;
                this.UrlWindows = url_windows;
                this.UrlMacOsX = url_macosx;
                this.UrlIos = url_ios;
                this.UrlAndroid = url_android;
                this.Atos = atos;
                this.Lexile = lexile;
                this.Isbn = isbn;
                this.Illustrator = illustrator;
                this.Narrator = narrator;
                this.UrlBackgroundCover = background_cover;
            }

            public void SetCountryCode(string language)
            {
                string code = string.Empty;

                if (language == Languages.English.ToDescription())
                {
                    code = "EN";
                }
                else if (language == Languages.Norwegian.ToDescription())
                {
                    code = "NO";
                }
                else if (language == Languages.British.ToDescription())
                {
                    code = "EN-GB";
                }
                else if (language == Languages.Chinese.ToDescription())
                {
                    code = "CN";
                }
                else if (language == Languages.Danish.ToDescription())
                {
                    code = "DK";
                }
                else if (language == Languages.NyNorsk.ToDescription())
                {
                    code = "NN-NO";
                }
                else
                {
                    Debug.LogError($"BookModel => SetCountryCode: Don't expect language - {language}. Set default country code to EN ");
                    code = "EN";
                }

                this.CountryCode = code;
            }
        }

        public struct QuizQuestionCounter
        {
            public int visible;
            public int invisible;
        }

        public class CheckBookExistModel
        {
            public BookModel book;
            public Translation translation;
            public Action<bool> existAction;
        }
    }
}