namespace BookApi.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public List<Book> borrowedBooks { get; set; } = new List<Book>();
    }
}