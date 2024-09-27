using Microsoft.EntityFrameworkCore;
using VacancyBot1.Models;

namespace VacancyBot1.Data;
public class ApplicationDbContext : DbContext
{
    public DbSet<Vacancy> Vacancies { get; set; }
    public DbSet<Candidate> Candidates { get; set; }
    public DbSet<Admin> Admins { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }
}
