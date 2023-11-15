using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.BooksLibraryModels;

namespace PFS.Assets.Scripts.Commands.BooksLoading
{
    public class InitializeCategoriesCommand : BaseCommand
    {
        [Inject] public BooksLibrary BooksLibrary { get; private set; }
        [Inject] public ChildModel ChildModel { get; private set; }

        private ChildModel child;
        public override void Execute()
        {
            Retain();

            child = ChildModel.GetChild(PlayerPrefsModel.CurrentChildId);

            if (child.CategoriesForStrategy != null)
            {
                // init categories positions
                for (int i = 0; i < child.CategoriesForStrategy.Length; i++)
                {
                    child.CategoriesForStrategy[i].Position = i;
                }
                BooksLibrary.InitBooksCategories(child.CategoriesForStrategy.Length);
            }

            Release();
        }
    }
}