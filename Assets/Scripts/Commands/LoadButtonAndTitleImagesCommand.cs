using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.BooksLibraryModels;
using System.Collections.Generic;
using System.Linq;

namespace PFS.Assets.Scripts.Commands.UI
{
    public class LoadButtonAndTitleImagesCommand : BaseCommand
    {
        [Inject] public ChildModel ChildModel { get; private set; }
        public override void Execute()
        {
            Retain();

            List<BooksCategory> allCategoriesAvailibleForChild = ChildModel.GetChild(PlayerPrefsModel.CurrentChildId)?.CategoriesForStrategy.ToList();

            foreach (BooksCategory booksCategory in allCategoriesAvailibleForChild)
            {
                booksCategory.DownloadButtonImageForPool();
                booksCategory.DownloadTitleImageForPool();
            }
            Release();
        }
    }
}