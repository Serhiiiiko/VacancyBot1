using System.ComponentModel.DataAnnotations;

namespace VacancyBot1.Models;
public class Candidate
{
    public int Id { get; set; }

    [Required]
    public long TelegramId { get; set; }

    public string TelegramUsername { get; set; }

    [Required]
    public string FullName { get; set; }

    [Required]
    public string PhoneNumber { get; set; }

    public string WorkExperience { get; set; }

    public int VacancyId { get; set; }

    public Vacancy Vacancy { get; set; }

    public DateTime DateApplied { get; set; } = DateTime.UtcNow;
}