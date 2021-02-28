using System;
using Core.Entities;
using Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configuration
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.HasIndex(s => new {s.StudentId, s.Email})
                    .IsUnique()
                    .IncludeProperties( s => new
                    {
                        s.FirstName,
                        s.LastName,
                        s.Branch
                    });

            builder.Property(s => s.Email).HasColumnType("varchar(50)")
                                            .IsRequired();

            builder.Property(s => s.FirstName).HasColumnType("varchar(30)")
                                            .IsRequired();

            builder.Property(s => s.LastName).HasColumnType("varchar(30)")
                                            .IsRequired();


            builder.Property(s => s.Gender)
                            .IsRequired()
                            .HasColumnType("varchar(20)")
                            .HasConversion(
                                g => g.ToString(),
                                g => (Gender)Enum.Parse(typeof(Gender), g)
                                );

            builder.Property(s => s.Branch)
                            .IsRequired()
                            .HasColumnType("varchar(30)")
                            .HasConversion(
                                b => b.ToString(),
                                b => (Branch)Enum.Parse(typeof(Branch), b)
                                );

            builder.Property(s => s.Extension)
                            .HasColumnType("varchar(10)")
                            .IsRequired();

            builder.Property(s => s.MobileNo)
                            .IsRequired();

            builder.Property(s => s.DateOfBirth)
                            .IsRequired();

            builder.Property(s => s.DateCreated)
                            .IsRequired();

            builder.Property(s => s.DateLastModified)
                            .IsRequired();

            builder.Property(s => s.Email)
                            .HasColumnType("varchar(50)")
                            .IsRequired();

            builder.Property(s => s.AdmissionYear)
                            .HasColumnType("SMALLINT")
                            .IsRequired();
            builder.Property(s => s.PassingYear)
                            .HasColumnType("SMALLINT")
                            .IsRequired();

            builder.OwnsOne(
                s => s.CurrentAddress,
                addressBuilder =>
                {
                    addressBuilder.Property(a => a.Country)
                                    .HasColumnType("varchar(30)")
                                    .IsRequired();
                    addressBuilder.Property(a => a.State)
                                    .HasColumnType("varchar(30)")
                                    .IsRequired();
                    addressBuilder.Property(a => a.City)
                                    .HasColumnType("varchar(30)")
                                    .IsRequired();
                    addressBuilder.Property(a => a.UserAddress)
                                    .HasColumnType("varchar(100)")
                                    .IsRequired();
                    addressBuilder.Property(a => a.Pincode)
                                    .IsRequired();
                }
            );


            builder.OwnsOne(
                s=> s.CorrespondanceAddress,
                addressBuilder =>
                {
                    addressBuilder.Property(a => a.Country)
                                    .HasColumnType("varchar(30)")
                                    .IsRequired();
                    addressBuilder.Property(a => a.State)
                                    .HasColumnType("varchar(30)")
                                    .IsRequired();
                    addressBuilder.Property(a => a.City)
                                    .HasColumnType("varchar(30)")
                                    .IsRequired();
                    addressBuilder.Property(a => a.UserAddress)
                                    .HasColumnType("varchar(100)")
                                    .IsRequired();
                    addressBuilder.Property(a => a.Pincode)
                                    .IsRequired();
                }
            );

        }
    }
}
