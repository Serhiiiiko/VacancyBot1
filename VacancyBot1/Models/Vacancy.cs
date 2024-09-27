﻿using System.ComponentModel.DataAnnotations;

namespace VacancyBot1.Models;
public class Vacancy
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = default!;

    public string Description { get; set; } = default!;

    public string Requirements { get; set; } = default!; 

    public byte[] Image { get; set; } = default!;

    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
}
