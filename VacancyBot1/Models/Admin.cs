using System.ComponentModel.DataAnnotations;


namespace VacancyBot1.Models;
public class Admin
{
    public int Id { get; set; }

    [Required]
    public long TelegramId { get; set; }

<<<<<<< HEAD
    public string TelegramUsername { get; set; } = default!;
=======
    public string TelegramUsername { get; set; }
>>>>>>> 507a0380ee99a877d10d8417417469b5c62df161

    public bool IsSuperAdmin { get; set; } = false;
}
