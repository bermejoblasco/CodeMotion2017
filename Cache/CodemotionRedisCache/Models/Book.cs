namespace CodemtionRedisCache.Models
{
    using System.Collections.Generic;

    public class Book
    {
        public string ISBN { get; set; }

        public string Title { get; set; }

        public int Score { get; set; }

        public List<Author> Authors { get; set; }

        public static List<Book> ListOfBooks()
        {
            var list = new List<Book>();

            var book = new Book()
            {
                ISBN = "1",
                Title = "Clean Code: A Handbook of Agile Software Craftsmanship",
                Score = 9
            };
            list.Add(book);

            book = new Book()
            {
                ISBN = "2",
                Title = "Refactoring Improving the Design of Existing Code",
                Score = 10
            };
            list.Add(book);

            book = new Book()
            {
                ISBN = "3",
                Title = "Head First Design Patterns",
                Score = 8
            };
            list.Add(book);

            book = new Book()
            {
                ISBN = "4",
                Title = "Soft Skills: The Software Developer's Life Manual",
                Score = 6
            };
            list.Add(book);

            book = new Book()
            {
                ISBN = "5",
                Title = "Domain-Driven Design: Tackling Complexity in the Heart of Software (I",
                Score = 7
            };
            list.Add(book);

            return list;
        }
    }
}
