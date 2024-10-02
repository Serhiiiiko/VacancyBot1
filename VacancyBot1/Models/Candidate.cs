using System.ComponentModel.DataAnnotations;

namespace VacancyBot1.Models;
public class Candidate
{
    public int Id { get; set; }

    [Required]
    public long TelegramId { get; set; }

    public string TelegramUsername { get; set; } = default!;

    [Required]
    public string FullName { get; set; } = default!;

    [Required]
    public string PhoneNumber { get; set; } = default!;

    public string WorkExperience { get; set; } = default!;

    public string? CVFilePath { get; set; }

    public int VacancyId { get; set; }
    public string? Email { get; set; }

    public Vacancy Vacancy { get; set; } = default!;

    public DateTime DateApplied { get; set; } = DateTime.UtcNow;
}