using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CatalogApp.Models;

public class Book
{
    public int Id {get; set;}
    [Required]
    public string? Title {get; set;}
    [Required]
    public string? Author {get; set;}
    [Required]
    public Genre Genre {get; set;}   
    [Required]
    public Restrict Restrict { get; set; } 
    public AgeGroup AgeGroup { get; set; } 
    public string? Series {get; set;}    
    public string? Publisher {get; set;}
    public string? Isbn {get; set;}
    public string? Location {get; set;}
    public string? FileExtension {get; set;}
    public byte[]? BookArray {get; set;}
}

public enum Genre { Biography, Children, Dystopian, Fantasy, Fiction, History, Horror, Mystery, NonFiction, Play, Poetry, Romance, RPG, ScienceFiction, Textbook, Thriller, Travel, Western, YoungAdult, Other }
public enum Restrict { None, Adult, Teen, Child }
public enum AgeGroup { Adult, YoungAdult, Child, Other }
