using Alumni.Auth;
using Microsoft.AspNetCore.Identity;
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
        builder.Entity<AuthUser>().HasOne<StudentEntity>().WithOne()
            .HasForeignKey<AuthUser>(user => user.StudentProfileId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<AuthUser>().HasOne<Faculty>().WithOne()
            .HasForeignKey<AuthUser>(user => user.FacultyProfileId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Invitation>().HasOne<StudentEntity>().WithMany()
            .HasForeignKey(invitation => invitation.StudentProfileId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Invitation>().HasOne<Faculty>().WithMany()
            .HasForeignKey(invitation => invitation.FacultyProfileId).OnDelete(DeleteBehavior.Restrict);
    }
}

public interface IApplicationContext : IStudentDbContext, IFacultyDbContext;
