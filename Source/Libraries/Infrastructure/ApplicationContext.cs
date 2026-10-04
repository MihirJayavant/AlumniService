using Alumni.Auth;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Infrastructure;

public class ApplicationContext(DbContextOptions<ApplicationContext> options)
    : IdentityDbContext<AuthUser, AuthRole, string>(options), IApplicationContext
{
    public DbSet<Invitation> Invitations { get; set; } = null!;
    public DbSet<AuthSession> AuthSessions { get; set; } = null!;
    public DbSet<StudentEntity> Students { get; set; } = null!;
    public DbSet<CompanyEntity> Companies { get; set; } = null!;
    public DbSet<ExamEntity> Exams { get; set; } = null!;
    public DbSet<FurtherStudyEntity> FurtherStudies { get; set; } = null!;
    public DbSet<Faculty> Faculties { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ConfigureAuthModel();
        builder.ApplyConfiguration(new StudentConfiguration());
        builder.ApplyConfiguration(new ExamConfiguration());
        builder.ApplyConfiguration(new CompanyConfiguration());
        builder.ApplyConfiguration(new FurtherStudyConfiguration());
        builder.ApplyConfiguration(new FacultyConfiguration());

    }
}

public interface IApplicationContext : IStudentDbContext, IFacultyDbContext;
