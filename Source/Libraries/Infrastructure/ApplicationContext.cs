using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Infrastructure;

public class ApplicationContext(DbContextOptions<ApplicationContext> options)
    : IdentityDbContext<ApplicationUser>(options), IApplicationContext
{
    public DbSet<StudentEntity> Students { get; set; } = null!;
    public DbSet<CompanyEntity> Companies { get; set; } = null!;
    public DbSet<ExamEntity> Exams { get; set; } = null!;
    public DbSet<FurtherStudyEntity> FurtherStudies { get; set; } = null!;
    public DbSet<Faculty> Faculties { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new StudentConfiguration());
        builder.ApplyConfiguration(new ExamConfiguration());
        builder.ApplyConfiguration(new CompanyConfiguration());
        builder.ApplyConfiguration(new FurtherStudyConfiguration());
        builder.ApplyConfiguration(new FacultyConfiguration());
        base.OnModelCreating(builder);
    }
}

public interface IApplicationContext : IStudentDbContext, IFacultyDbContext;
