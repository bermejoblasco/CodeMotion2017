using System.Collections.Generic;

namespace CodemtionRedisCache.Models
{
    public class Author
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public static List<Author> ListOfAuthors()
        {
            var list = new List<Author>();

            var author = new Author()
            {
                Name = "Robert",
                Surname = "C.Martin"
            };          

            list.Add(author);

            author = new Author()
            {
                Name = "Martin",
                Surname = "Fowler"
            };

            list.Add(author);

            author = new Author()
            {
                Name = "Scott",
                Surname = "Hanselman"
            };

            list.Add(author);

            author = new Author()
            {
                Name = "Eric",
                Surname = "Evans"
            };

            list.Add(author);

            author = new Author()
            {
                Name = "Rockford",
                Surname = "Lhotka"
            };

            list.Add(author);

            return list;
        }
    }
}
