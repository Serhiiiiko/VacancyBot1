using System.ComponentModel.DataAnnotations;

namespace VacancyBot1.Models;
public class Vacancy
{
    public int Id { get; set; }

    [Required]
<<<<<<< HEAD
    public string Title { get; set; } = default!;

    public string Description { get; set; } = default!;

    public string Requirements { get; set; } = default!; 

    public byte[] Image { get; set; } = default!;
=======
    public string Title { get; set; }

    public string Description { get; set; }

    public string Requirements { get; set; }

    public byte[] Image { get; set; } // Store image as byte array
>>>>>>> 507a0380ee99a877d10d8417417469b5c62df161

    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
}
