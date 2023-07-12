namespace CatalogApp.Models;

public class BookFile : Book
{
    public IFormFile? BookFormFile { get; set; }
}