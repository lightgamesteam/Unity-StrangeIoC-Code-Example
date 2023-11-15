using Conditions;
using System.Collections.Generic;

namespace PFS.Assets.Scripts.Models.BooksLibraryModels
{
    public class BooksLibraryFilter
    {
        public List<SimplifiedLevels> stages;
        public List<Languages> languages;
        public List<BooksCategory> interests;

        public string searchTitle;

        public BooksLibraryFilter(List<SimplifiedLevels> stages, List<Languages> languages, List<BooksCategory> interests, string searchTitle)
        {
            this.stages = stages;
            this.languages = languages;
            this.interests = interests;

            this.searchTitle = searchTitle;
        }

        public bool IsEmpty()
        {
            return stages.Count == 0 && languages.Count == 0 && interests.Count == 0 && string.IsNullOrEmpty(searchTitle);
        }
    }
}