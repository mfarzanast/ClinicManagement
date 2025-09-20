using EmployeeAPi.Models;
using EmployeeAPI.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Patient>  Patients { get; set; }
    public DbSet<Payments> Payments { get; set; }
    public DbSet<HealthInformation> HealthInformations { get; set; }
}