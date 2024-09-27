using System.ComponentModel.DataAnnotations;

namespace VacancyBot1.Models;
public class Vacancy
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    public string Description { get; set; }

    public string Requirements { get; set; }

    public byte[] Image { get; set; } // Store image as byte array

    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
}
