using Microsoft.AspNetCore.Mvc;
using BookApi.Models;
using BookApi.Servic;

namespace BookApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private static readonly List<Book> Books = new();
        private static readonly List<Client> Clients = new();

        private readonly LibraryService _libraryService;

        private readonly ILogger<BooksController> _logger;

        public BooksController(ILogger<BooksController> logger, LibraryService libraryService)
        {
            _logger = logger;
            _libraryService = libraryService;
        }

        // GET: Get all the books
        [HttpGet]
        public IEnumerable<Book> Get()
        {
            return _libraryService.Books;
        }

        // GET: Find a book by its id
        [HttpGet("{id}")]
        public ActionResult<Book> Get(int id)
        {
            var book = _libraryService.Books.Find(b => b.Id == id);
            if (book == null)
            {
                return NotFound($"The book with ID {id} was not found in the library.");
            }
            return book;
        }

        // POST: Insert books
        [HttpPost]
        public ActionResult<IEnumerable<Book>> Post([FromBody] List<Book> newBooks)
        {
            if (newBooks == null || !newBooks.Any())
            {
                return BadRequest("No books provided");
            }

            foreach (var book in newBooks)
            {
                book.Id = _libraryService.Books.Count + 1; // Auto-increment for ID
                _libraryService.Books.Add(book);
            }

            // Return the list of added books
            return CreatedAtAction(nameof(Get), new { }, newBooks);
        }

        // PUT: Update a book by its id
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Book updatedBook)
        {
            if (updatedBook == null || updatedBook.Id != id)
            {
                return BadRequest();
            }

            var existingBook = _libraryService.Books.Find(b => b.Id == id);
            if (existingBook == null)
            {
                return NotFound($"The book with ID {id} was not found in the library.");
            }

            existingBook.Title = updatedBook.Title;
            existingBook.Author = updatedBook.Author;
            existingBook.Price = updatedBook.Price;

            return Ok($"The book '{existingBook.Title}' has been successfully updated");
        }

        // DELETE: Delete a book
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var book = _libraryService.Books.Find(b => b.Id == id);
            if (book == null)
            {
                return NotFound($"The book with ID {id} was not found in the library.");
            }

            _libraryService.Books.Remove(book);
            return Ok($"The book '{book.Title}' has been successfully deleted");
        }

        // PUT: Borrow a book 
        // @param idClient
        // @param idBook
        [HttpPut("borrowBook/{idBook}")]
        public IActionResult BorrowBook(int idBook, [FromQuery] int client)
        {
            _logger.LogInformation("Test if we have the book.");

            // Test if we have the book
            var book = _libraryService.Books.Find(b => b.Id == idBook);
            if (book == null)
            {
                return NotFound($"The book with ID {idBook} was not found in the library.");
            }

            // Test if the book is already borrowed by another customer
            var borrowerCustomer = _libraryService.Clients.Find(c => c.borrowedBooks.Count > 0 && c.borrowedBooks.Contains(book));
            // _logger.LogInformation("borrower customer: "+borrowerCustomer == null ? "null" : "not null");
            // _logger.LogInformation("book borrowed " +book.isBorrowed);
            if (book.isBorrowed && borrowerCustomer != null && borrowerCustomer.Id != client)
            {
                return BadRequest($"The book with ID {idBook} is already borrowed by {borrowerCustomer.Name}.");
            }

            // Test if the client exists
            var existingClient = _libraryService.Clients.Find(b => b.Id == client);
            if (existingClient == null)
            {
                return NotFound($"The client with ID {client} was not found.");
            }

            existingClient.borrowedBooks.Add(book);
            book.isBorrowed = true;

            return Ok($"The book {idBook} is now borrowed by the client {existingClient.Name}.");
        }

        // PUT: Return a book 
        // @param idClient
        // @param idBook
        [HttpPut("returnBook/{idBook}")]
        public IActionResult ReturnBook(int idBook, [FromQuery] int client)
        {
            _logger.LogInformation("Test if we have the book.");

            // Test if we have the book
            var book = _libraryService.Books.Find(b => b.Id == idBook);
            if (book == null)
            {
                return NotFound($"The book with ID {idBook} was not found in the library.");
            }

            // Test if the client exists
            var existingClient = _libraryService.Clients.Find(b => b.Id == client);
            if (existingClient == null)
            {
                return NotFound($"The client with ID {client} was not found.");
            }

            // Test if the book is borrowed by the client
            var borrowerCustomer = existingClient.borrowedBooks.Count > 0 && existingClient.borrowedBooks.Contains(book);
            _logger.LogInformation("borrower customer: "+borrowerCustomer);
            _logger.LogInformation("book borrowed " +book.isBorrowed);

            if (!book.isBorrowed || !borrowerCustomer) {
                return BadRequest($"The book '{book.Title}' is not borrowed by {existingClient.Name}.");
            }


            existingClient.borrowedBooks.Remove(book);
            book.isBorrowed = false;

            return Ok($"The book '{book.Title}' is not anymore borrowed by the client {existingClient.Name}.");
        }
    }
}
