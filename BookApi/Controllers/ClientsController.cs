using Microsoft.AspNetCore.Mvc;
using BookApi.Models;
using BookApi.Servic;

namespace BookApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {

        private static readonly List<Client> Clients = new();
        private readonly LibraryService _libraryService;
        private readonly ILogger<BooksController> _logger;

        public ClientsController(ILogger<BooksController> logger, LibraryService libraryService)
        {
            _logger = logger;
            _libraryService = libraryService;
        }


        // GET: Get all the clients
        [HttpGet]
        public IEnumerable<Client> Get()
        {
            return _libraryService.Clients;
        }

        // GET: Find a client by its id
        [HttpGet("{id}")]
        public ActionResult<Client> Get(int id)
        {
            var client = _libraryService.Clients.Find(b => b.Id == id);
            if (client == null)
            {
                return NotFound($"The client with ID {id} was not found.");
            }
            return client;
        }

        // POST: Insert clients
        [HttpPost]
        public ActionResult<IEnumerable<Client>> Post([FromBody] List<Client> newClients)
        {
            if (newClients == null || !newClients.Any())
            {
                return BadRequest("No cliens provided");
            }

            foreach (var client in newClients)
            {
                client.Id = _libraryService.Clients.Count + 1; // Auto-increment for ID
                _libraryService.Clients.Add(client);
            }

            // Returns the list of added clients
            return CreatedAtAction(nameof(Get), new { }, newClients);
        }

        // PUT: Update a client by its id
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Client updatedClient)
        {
            if (updatedClient == null || updatedClient.Id != id)
            {
                return BadRequest();
            }

            var existingClient = _libraryService.Clients.Find(b => b.Id == id);
            if (existingClient == null)
            {
                return NotFound($"The client with ID {id} was not found.");
            }

            existingClient.Name = updatedClient.Name;
            existingClient.Age = updatedClient.Age;

            return Ok($"The client {existingClient.Name} has been successfully updated.");
        }

        // DELETE: Delete a client
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var client = _libraryService.Clients.Find(b => b.Id == id);
            if (client == null)
            {
                return NotFound($"The client with ID {id} was not found.");
            }

            _libraryService.Clients.Remove(client);
            return Ok($"The client {client.Name} has been successfully deleted");
        }
    }
}