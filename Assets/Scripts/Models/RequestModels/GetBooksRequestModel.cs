using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Conditions;
using PFS.Assets.Scripts.Models.BooksLibraryModels;

namespace PFS.Assets.Scripts.Models.Requests
{
    public class GetBooksRequestModel : BasicRequestModel
    {
        public int page;
        public int perpage;
        public string[] categoryIds;
        [JsonProperty(PropertyName = "simplified_level")]
        public string[] simplifiedLevels;
        public string[] languages = new string[2] { "English", "Norwegian" };
        public string search = string.Empty;

        [NonSerialized] public List<BooksCategory> categories = new List<BooksCategory>();
        [NonSerialized] public List<SimplifiedLevels> simplifiedLevelsEnums = new List<SimplifiedLevels>();
        [NonSerialized] public List<Languages> languagesEnums = new List<Languages>();
        [NonSerialized] public BooksRequestType booksRequestType = BooksRequestType.BooksByCategory;

        public GetBooksRequestModel() : base()
        { }

        public GetBooksRequestModel(Action successAction, Action failAction) : base(successAction, failAction)
        { }

        public GetBooksRequestModel(int page, int perpage, string[] categories, string[] simplifiedLevels, string[] languages, string search, Action successAction, Action failAction) : this(successAction, failAction)
        {
            this.page = page;
            this.perpage = perpage;
            this.categoryIds = categories;
            this.simplifiedLevels = simplifiedLevels;
            this.languages = languages;
            this.search = search;
        }

        public GetBooksRequestModel(int page, int perpage, List<BooksCategory> categories, List<SimplifiedLevels> simplifiedLevelsEnums, List<Languages> languagesEnums, string search, BooksRequestType booksRequestType, Action successAction, Action failAction) : this(successAction, failAction)
        {
            this.page = page;
            this.perpage = perpage;
            this.categories = categories;
            this.simplifiedLevelsEnums = simplifiedLevelsEnums;
            this.languagesEnums = languagesEnums;
            this.search = search;
            this.booksRequestType = booksRequestType;
        }

        /// <summary>
        /// Form book library by one category
        /// </summary>
        /// <param name="page"></param>
        /// <param name="perpage"></param>
        /// <param name="category"></param>
        /// <param name="successAction"></param>
        /// <param name="failAction"></param>
        public GetBooksRequestModel(int page, int perpage, BooksCategory category, Action successAction, Action failAction) : this(successAction, failAction)
        {
            this.page = page;
            this.perpage = perpage;
            this.categories = new List<BooksCategory>() { category };
            //this is fix for Norwegian category
            //if category - Norwegian, we need to show books whitch has Norwegian for sure
            if (category.TechnicalName == "Norwegian")
            {
                this.languagesEnums = new List<Languages>() { Languages.Norwegian };
            }
            else //default 
            {
                this.languagesEnums = new List<Languages>();
            }
            this.booksRequestType = BooksRequestType.BooksByCategory;
        }
    }
}