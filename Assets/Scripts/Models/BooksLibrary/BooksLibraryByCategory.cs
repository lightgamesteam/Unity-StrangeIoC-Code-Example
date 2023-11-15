
namespace PFS.Assets.Scripts.Models.BooksLibraryModels
{
    public class BooksLibraryByCategory
    {
        public Books BooksByCategory { get; private set; } = new Books();
        public BooksCategory BookCategory { get; private set; }

        public BooksLibraryByCategory()
        {

        }

        public BooksLibraryByCategory(BooksCategory bookCategory)
        {
            this.BookCategory = bookCategory;
        }

        public void SetBooks(Books booksByCategory)
        {
            this.BooksByCategory = booksByCategory;
        }

        public void SetCategory(BooksCategory bookCategory)
        {
            this.BookCategory = bookCategory;
        }
    }
}