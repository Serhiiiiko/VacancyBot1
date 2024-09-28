using System.ComponentModel.DataAnnotations;

namespace VacancyBot1.Models;
public class Candidate
{
    public int Id { get; set; }

    [Required]
    public long TelegramId { get; set; }

<<<<<<< HEAD
    public string TelegramUsername { get; set; } = default!;

    [Required]
    public string FullName { get; set; } = default!;

    [Required]
    public string PhoneNumber { get; set; } = default!;

    public string WorkExperience { get; set; } = default!;

    public int VacancyId { get; set; }

    public Vacancy Vacancy { get; set; } = default!;
=======
    public string TelegramUsername { get; set; }

    [Required]
    public string FullName { get; set; }

    [Required]
    public string PhoneNumber { get; set; }

    public string WorkExperience { get; set; }

    public int VacancyId { get; set; }

    public Vacancy Vacancy { get; set; }
>>>>>>> 507a0380ee99a877d10d8417417469b5c62df161

    public DateTime DateApplied { get; set; } = DateTime.UtcNow;
}