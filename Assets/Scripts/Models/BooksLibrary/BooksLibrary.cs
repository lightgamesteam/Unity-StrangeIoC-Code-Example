using System.Collections.Generic;

namespace PFS.Assets.Scripts.Models.BooksLibraryModels
{
    public class BooksLibrary
    {
        public static BooksLibrary Instance { get; set; }

        public Books[] categoriesBooks;

        public Books searchResultBooks = new Books();

        public Books downloadedBooks = new Books();
        public Books favoritesBooks = new Books();

        public Books featuredBooks = new Books();
        public Books homeworkBooks = new Books();

        //Need for count book with not allowed language from strategy for counting summ of all books and stop download Coroutine
        public Books nonAllowedLanguageBooks = new Books();

        public Books statsMostReadBooks = new Books();

        public BooksLibrary()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void InitBooksCategories(int categoryLenght)
        {
            categoriesBooks = new Books[categoryLenght];
            for (int i = 0; i < categoriesBooks.Length; i++)
            {
                categoriesBooks[i] = new Books();
            }
        }

        public void ClearBooks()
        {
            foreach (var item in categoriesBooks)
            {
                item.Clear();
            }

            searchResultBooks.Clear();

            downloadedBooks.Clear();
            favoritesBooks.Clear();

            featuredBooks.Clear();

            statsMostReadBooks.Clear();
            homeworkBooks.Clear();

            nonAllowedLanguageBooks.Clear();
        }

        public BookModel GetBook(string bookId)
        {
            List<Books> categories = new List<Books>();
            categories.AddRange(categoriesBooks);
            categories.AddRange(new Books[6] { searchResultBooks,
                                           downloadedBooks,
                                           favoritesBooks,
                                           homeworkBooks,
                                           statsMostReadBooks,
                                           featuredBooks });

            foreach (Books category in categories)
            {
                foreach (BookModel book in category.books)
                {
                    if (book.Id == bookId)
                    {
                        return book;
                    }
                }
            }

            return null;
        }

        public BookModel GetBookByGuizId(string quizId)
        {
            List<Books> categories = new List<Books>();
            categories.AddRange(categoriesBooks);
            categories.AddRange(new Books[6] { searchResultBooks,
                                           downloadedBooks,
                                           favoritesBooks,
                                           homeworkBooks,
                                           statsMostReadBooks,
                                           featuredBooks });

            foreach (Books category in categories)
            {
                foreach (BookModel book in category.books)
                {
                    if (book.QuizId == quizId)
                    {
                        return book;
                    }
                }
            }

            return null;
        }
        public BookModel GetBook(string bookId, Books categotyBooks)
        {
            foreach (BookModel book in categotyBooks.books)
            {
                if (book.Id == bookId)
                {
                    return book;
                }
            }

            return null;
        }

        public void AddHomeworkBooks(List<BookModel> books)
        {
            homeworkBooks.Clear();
            homeworkBooks.books.AddRange(books);
        }
    }

    public class Books
    {
        public List<BookModel> books = new List<BookModel>();
        public int totalBooks = -1;
        public int currentPage = 1;

        public void Clear()
        {
            books.Clear();
            totalBooks = -1;
            currentPage = 1;
        }

        public bool ContainsBook(BookModel askBook)
        {
            bool ret = false;

            for (int i = 0; i < books.Count; i++)
            {
                if (askBook.Id == books[i].Id)
                {
                    ret = true;
                }
            }

            return ret;
        }
    }
}