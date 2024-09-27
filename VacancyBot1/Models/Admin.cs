using System.ComponentModel.DataAnnotations;


namespace VacancyBot1.Models;
public class Admin
{
    public int Id { get; set; }

    [Required]
    public long TelegramId { get; set; }

    public string TelegramUsername { get; set; } = default!;

    public bool IsSuperAdmin { get; set; } = false;
}
