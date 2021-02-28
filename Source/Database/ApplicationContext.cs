using Microsoft.EntityFrameworkCore;
using Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Database
{
    public class ApplicationContext: IdentityDbContext<IdentityUser>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options): base(options)
        {

        }

        public DbSet<StudentAccount> StudentAccount { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Company> Companies {get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<FurtherStudy> FurtherStudies {get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
