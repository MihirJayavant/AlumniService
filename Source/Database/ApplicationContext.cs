using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Database;

public class ApplicationContext : IdentityDbContext<IdentityUser>
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {

    }

    public DbSet<StudentAccount> StudentAccount { get; set; } = default!;
    public DbSet<Student> Students { get; set; } = default!;
    public DbSet<Faculty> Faculties { get; set; } = default!;
    public DbSet<Company> Companies { get; set; } = default!;
    public DbSet<Exam> Exams { get; set; } = default!;
    public DbSet<FurtherStudy> FurtherStudies { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
