using BookApi.Models;

namespace BookApi.Servic
{
    public class LibraryService
    {
        public List<Book> Books { get; set; } = new();
        public List<Client> Clients { get; set; } = new();
    }
}

