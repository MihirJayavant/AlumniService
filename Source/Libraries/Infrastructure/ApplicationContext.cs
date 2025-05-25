using Alumni.Faculty;
using Alumni.Student;
using Alumni.Student.Company;
using Alumni.Student.Exam;
using Alumni.Student.FurtherStudy;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Infrastructure;

public class ApplicationContext(DbContextOptions<ApplicationContext> options)
    : IdentityDbContext<ApplicationUser>(options), IApplicationContext
{
    public DbSet<StudentEntity> Students { get; set; }
    public DbSet<CompanyEntity> Companies { get; set;}
    public DbSet<ExamEntity> Exams { get; set;}
    public DbSet<FurtherStudyEntity> FurtherStudies { get; set;}
    public DbSet<Faculty> Faculties { get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new StudentConfiguration());
        modelBuilder.ApplyConfiguration(new ExamConfiguration());
        modelBuilder.ApplyConfiguration(new CompanyConfiguration());
        modelBuilder.ApplyConfiguration(new FurtherStudyConfiguration());
        modelBuilder.ApplyConfiguration(new FacultyConfiguration());
        base.OnModelCreating(modelBuilder);
    }

}

public interface IApplicationContext: IStudentDbContext, IFacultyDbContext;
