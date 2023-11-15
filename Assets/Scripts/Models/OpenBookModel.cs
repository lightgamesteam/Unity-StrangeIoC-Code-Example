using Conditions;

namespace PFS.Assets.Scripts.Models.BooksLibraryModels
{
    public class OpenBookModel
    {
        public BookModel Book { get; private set; }
        public BookReadingType ReadingType { get; private set; }

        public OpenBookModel(BookModel book, BookReadingType readingType)
        {
            this.Book = book;
            this.ReadingType = readingType;
        }
    }
}