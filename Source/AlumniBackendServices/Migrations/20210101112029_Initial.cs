using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace AlumniBackendServices.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Faculties",
                columns: table => new
                {
                    FacultyId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "varchar(50)", nullable: false),
                    FirstName = table.Column<string>(type: "varchar(50)", nullable: false),
                    LastName = table.Column<string>(type: "varchar(50)", nullable: false),
                    Extension = table.Column<string>(type: "varchar(10)", nullable: false),
                    MobileNo = table.Column<long>(type: "bigint", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faculties", x => x.FacultyId);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    StudentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "varchar(30)", nullable: false),
                    LastName = table.Column<string>(type: "varchar(30)", nullable: false),
                    MobileNo = table.Column<long>(type: "bigint", nullable: false),
                    Extension = table.Column<string>(type: "varchar(10)", nullable: false),
                    Gender = table.Column<string>(type: "varchar(20)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Email = table.Column<string>(type: "varchar(50)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DateLastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Branch = table.Column<string>(type: "varchar(30)", nullable: false),
                    CurrentAddress_Pincode = table.Column<int>(type: "integer", nullable: true),
                    CurrentAddress_Country = table.Column<string>(type: "varchar(30)", nullable: true),
                    CurrentAddress_State = table.Column<string>(type: "varchar(30)", nullable: true),
                    CurrentAddress_City = table.Column<string>(type: "varchar(30)", nullable: true),
                    CurrentAddress_UserAddress = table.Column<string>(type: "varchar(100)", nullable: true),
                    CorrespondanceAddress_Pincode = table.Column<int>(type: "integer", nullable: true),
                    CorrespondanceAddress_Country = table.Column<string>(type: "varchar(30)", nullable: true),
                    CorrespondanceAddress_State = table.Column<string>(type: "varchar(30)", nullable: true),
                    CorrespondanceAddress_City = table.Column<string>(type: "varchar(30)", nullable: true),
                    CorrespondanceAddress_UserAddress = table.Column<string>(type: "varchar(100)", nullable: true),
                    AdmissionYear = table.Column<short>(type: "SMALLINT", nullable: false),
                    PassingYear = table.Column<short>(type: "SMALLINT", nullable: false),
                    StudentAccount = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.StudentId);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    CompanyId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyName = table.Column<string>(type: "varchar(50)", nullable: false),
                    Designation = table.Column<string>(type: "varchar(30)", nullable: false),
                    YearOfJoining = table.Column<short>(type: "SMALLINT", nullable: false),
                    AnnualSalary = table.Column<long>(type: "bigint", nullable: false),
                    StudentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.CompanyId);
                    table.ForeignKey(
                        name: "FK_Companies_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Exams",
                columns: table => new
                {
                    ExamId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExamName = table.Column<string>(type: "varchar(50)", nullable: false),
                    Score = table.Column<short>(type: "SMALLINT", nullable: false),
                    Year = table.Column<short>(type: "SMALLINT", nullable: false),
                    StudentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exams", x => x.ExamId);
                    table.ForeignKey(
                        name: "FK_Exams_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FurtherStudies",
                columns: table => new
                {
                    FurtherStudyId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InstituteName = table.Column<string>(type: "varchar(50)", nullable: false),
                    Degree = table.Column<string>(type: "varchar(50)", nullable: false),
                    AdmissionYear = table.Column<short>(type: "SMALLINT", nullable: false),
                    PassingYear = table.Column<short>(type: "SMALLINT", nullable: false),
                    Country = table.Column<string>(type: "varchar(30)", nullable: false),
                    City = table.Column<string>(type: "varchar(30)", nullable: false),
                    StudentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FurtherStudies", x => x.FurtherStudyId);
                    table.ForeignKey(
                        name: "FK_FurtherStudies_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentAccount",
                columns: table => new
                {
                    StudentAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "varchar(50)", nullable: false),
                    Password = table.Column<string>(type: "varchar(200)", nullable: false),
                    StudentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAccount", x => x.StudentAccountId);
                    table.ForeignKey(
                        name: "FK_StudentAccount_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Companies_CompanyName_Designation",
                table: "Companies",
                columns: new[] { "CompanyName", "Designation" });

            migrationBuilder.CreateIndex(
                name: "IX_Companies_StudentId",
                table: "Companies",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_ExamName",
                table: "Exams",
                column: "ExamName");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_StudentId",
                table: "Exams",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Faculties_Email",
                table: "Faculties",
                column: "Email",
                unique: true)
                .Annotation("Npgsql:IndexInclude", new[] { "FirstName", "LastName" });

            migrationBuilder.CreateIndex(
                name: "IX_FurtherStudies_InstituteName_Country",
                table: "FurtherStudies",
                columns: new[] { "InstituteName", "Country" });

            migrationBuilder.CreateIndex(
                name: "IX_FurtherStudies_StudentId",
                table: "FurtherStudies",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAccount_StudentId",
                table: "StudentAccount",
                column: "StudentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_StudentId_Email",
                table: "Students",
                columns: new[] { "StudentId", "Email" },
                unique: true)
                .Annotation("Npgsql:IndexInclude", new[] { "FirstName", "LastName", "Branch" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "Exams");

            migrationBuilder.DropTable(
                name: "Faculties");

            migrationBuilder.DropTable(
                name: "FurtherStudies");

            migrationBuilder.DropTable(
                name: "StudentAccount");

            migrationBuilder.DropTable(
                name: "Students");
        }
    }
}
