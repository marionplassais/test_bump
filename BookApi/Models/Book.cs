namespace BookApi.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool isBorrowed { get; set; }

        public Book(String title, String author, String description, decimal price) {
            Title = title;
            Author = author;
            Description = description;
            Price = price;
            isBorrowed = false;
        }

    }

}