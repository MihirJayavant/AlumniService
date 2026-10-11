namespace Alumni.IntegrationTests.Student;

[Collection("PostgreSQL")]
public sealed class StudentRelationshipTests(PostgreSqlFixture fixture) : IAsyncLifetime
{
    public ValueTask InitializeAsync() => new(fixture.ResetAsync());

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task Execute_WhenAddingCompany_PersistsNormalizedFieldsAndGetsOnlyRequestedStudentsCompanies()
    {
        var student = await StudentTestData.AddAsync(fixture);
        var other = await StudentTestData.AddAsync(fixture, "other@example.com");
        await using var context = fixture.CreateContext();
        var result = await new AddCompanyHandler(context).Execute(CompanyRequest(student.StudentId), TestContext.Current.CancellationToken);
        var unrelated = await new AddCompanyHandler(context).Execute(
            CompanyRequest(other.StudentId) with { CompanyName = "Other Company" }, TestContext.Current.CancellationToken);

        Assert.True(result.IsT0);
        Assert.True(unrelated.IsT0);
        var response = result.AsT0;
        Assert.NotEqual(Guid.Empty, response.CompanyId);
        Assert.Equal("Example Company", response.CompanyName);
        Assert.Equal("Engineer", response.Designation);
        Assert.Equal(2020, response.YearOfJoining);
        Assert.Equal(1200000, response.AnnualSalary);

        await using var verification = fixture.CreateContext();
        var persisted = await verification.Companies.Include(item => item.Student)
            .SingleAsync(item => item.CompanyId == response.CompanyId, TestContext.Current.CancellationToken);
        Assert.True(persisted.Id > 0);
        Assert.Equal(student.StudentId, persisted.Student.StudentId);
        Assert.Equal(persisted.Student.Id, persisted.StudentId);
        Assert.Equal(response, persisted.ToCompanyResponse());
        var retrieved = await new GetCompanyHandler(verification).Execute(
            new GetCompany { StudentId = student.StudentId }, TestContext.Current.CancellationToken);
        Assert.True(retrieved.IsT0);
        Assert.Equal(response, Assert.Single(retrieved.AsT0.Items));
        Assert.Equal(1, retrieved.AsT0.TotalCount);
    }

    [Fact]
    public async Task Execute_WhenAddingExam_PersistsNormalizedFieldsAndGetsOnlyRequestedStudentsExams()
    {
        var student = await StudentTestData.AddAsync(fixture);
        var other = await StudentTestData.AddAsync(fixture, "other@example.com");
        await using var context = fixture.CreateContext();
        var result = await new AddExamHandler(context).Execute(ExamRequest(student.StudentId), TestContext.Current.CancellationToken);
        var unrelated = await new AddExamHandler(context).Execute(
            ExamRequest(other.StudentId) with { ExamName = "Other Exam" }, TestContext.Current.CancellationToken);

        Assert.True(result.IsT0);
        Assert.True(unrelated.IsT0);
        var response = result.AsT0;
        Assert.NotEqual(Guid.Empty, response.ExamId);
        Assert.Equal("GATE", response.ExamName);
        Assert.Equal(750, response.Score);
        Assert.Equal(2020, response.Year);

        await using var verification = fixture.CreateContext();
        var persisted = await verification.Exams.Include(item => item.Student)
            .SingleAsync(item => item.ExamId == response.ExamId, TestContext.Current.CancellationToken);
        Assert.True(persisted.Id > 0);
        Assert.Equal(student.StudentId, persisted.Student.StudentId);
        Assert.Equal(persisted.Student.Id, persisted.StudentId);
        Assert.Equal(response, persisted.ToExamResponse());
        var retrieved = await new GetExamHandler(verification).Execute(
            new GetExam { StudentId = student.StudentId }, TestContext.Current.CancellationToken);
        Assert.True(retrieved.IsT0);
        Assert.Equal(response, Assert.Single(retrieved.AsT0.Items));
        Assert.Equal(1, retrieved.AsT0.TotalCount);
    }

    [Fact]
    public async Task Execute_WhenAddingFurtherStudy_PersistsNormalizedFieldsAndGetsOnlyRequestedStudentsFurtherStudies()
    {
        var student = await StudentTestData.AddAsync(fixture);
        var other = await StudentTestData.AddAsync(fixture, "other@example.com");
        await using var context = fixture.CreateContext();
        var result = await new AddFurtherStudyHandler(context).Execute(StudyRequest(student.StudentId), TestContext.Current.CancellationToken);
        var unrelated = await new AddFurtherStudyHandler(context).Execute(
            StudyRequest(other.StudentId) with { InstituteName = "Other Institute" }, TestContext.Current.CancellationToken);

        Assert.True(result.IsT0);
        Assert.True(unrelated.IsT0);
        var response = result.AsT0;
        Assert.NotEqual(Guid.Empty, response.FurtherStudyId);
        Assert.Equal("Example Institute", response.InstituteName);
        Assert.Equal("MSc", response.Degree);
        Assert.Equal("India", response.Country);
        Assert.Equal("Pune", response.City);
        Assert.Equal(2020, response.AdmissionYear);
        Assert.Equal(2022, response.PassingYear);

        await using var verification = fixture.CreateContext();
        var persisted = await verification.FurtherStudies.Include(item => item.Student)
            .SingleAsync(item => item.FurtherStudyId == response.FurtherStudyId, TestContext.Current.CancellationToken);
        Assert.True(persisted.Id > 0);
        Assert.Equal(student.StudentId, persisted.Student.StudentId);
        Assert.Equal(persisted.Student.Id, persisted.StudentId);
        Assert.Equal(response, persisted.ToFurtherStudyResponse());
        var retrieved = await new GetFurtherStudyHandler(verification).Execute(
            new GetFurtherStudy { StudentId = student.StudentId }, TestContext.Current.CancellationToken);
        Assert.True(retrieved.IsT0);
        Assert.Equal(response, Assert.Single(retrieved.AsT0.Items));
        Assert.Equal(1, retrieved.AsT0.TotalCount);
    }

    [Theory]
    [InlineData("Company")]
    [InlineData("Exam")]
    [InlineData("FurtherStudy")]
    public async Task Execute_WhenGettingRelationshipsForStudentWithoutAny_ReturnsEmptyPage(string relationship)
    {
        var student = await StudentTestData.AddAsync(fixture);
        await using var context = fixture.CreateContext();

        switch (relationship)
        {
            case "Company":
                AssertEmptyPage(await new GetCompanyHandler(context).Execute(
                    new GetCompany { StudentId = student.StudentId }, TestContext.Current.CancellationToken));
                break;
            case "Exam":
                AssertEmptyPage(await new GetExamHandler(context).Execute(
                    new GetExam { StudentId = student.StudentId }, TestContext.Current.CancellationToken));
                break;
            case "FurtherStudy":
                AssertEmptyPage(await new GetFurtherStudyHandler(context).Execute(
                    new GetFurtherStudy { StudentId = student.StudentId }, TestContext.Current.CancellationToken));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(relationship));
        }
    }

    [Theory]
    [InlineData("Company")]
    [InlineData("Exam")]
    [InlineData("FurtherStudy")]
    public async Task Execute_WhenGettingRelationshipsForMissingStudent_ReturnsNotFound(string relationship)
    {
        await StudentTestData.AddAsync(fixture);
        var missingId = Guid.NewGuid();
        await using var context = fixture.CreateContext();

        switch (relationship)
        {
            case "Company":
                AssertNotFound(await new GetCompanyHandler(context).Execute(
                    new GetCompany { StudentId = missingId }, TestContext.Current.CancellationToken));
                break;
            case "Exam":
                AssertNotFound(await new GetExamHandler(context).Execute(
                    new GetExam { StudentId = missingId }, TestContext.Current.CancellationToken));
                break;
            case "FurtherStudy":
                AssertNotFound(await new GetFurtherStudyHandler(context).Execute(
                    new GetFurtherStudy { StudentId = missingId }, TestContext.Current.CancellationToken));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(relationship));
        }
    }

    [Theory]
    [InlineData("Company")]
    [InlineData("Exam")]
    [InlineData("FurtherStudy")]
    public async Task Execute_WhenAddingRelationshipForMissingStudent_ReturnsNotFoundWithoutWritingRows(string relationship)
    {
        var student = await StudentTestData.AddAsync(fixture);
        var missingId = Guid.NewGuid();
        await using var context = fixture.CreateContext();

        switch (relationship)
        {
            case "Company":
                AssertNotFound(await new AddCompanyHandler(context).Execute(CompanyRequest(missingId), TestContext.Current.CancellationToken));
                break;
            case "Exam":
                AssertNotFound(await new AddExamHandler(context).Execute(ExamRequest(missingId), TestContext.Current.CancellationToken));
                break;
            case "FurtherStudy":
                AssertNotFound(await new AddFurtherStudyHandler(context).Execute(StudyRequest(missingId), TestContext.Current.CancellationToken));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(relationship));
        }

        await using var verification = fixture.CreateContext();
        Assert.False(await verification.Companies.AnyAsync(TestContext.Current.CancellationToken));
        Assert.False(await verification.Exams.AnyAsync(TestContext.Current.CancellationToken));
        Assert.False(await verification.FurtherStudies.AnyAsync(TestContext.Current.CancellationToken));
        var remainingStudent = await verification.Students.SingleAsync(TestContext.Current.CancellationToken);
        Assert.Equal(student.StudentId, remainingStudent.StudentId);
    }

    [Theory]
    [InlineData("Company")]
    [InlineData("Exam")]
    [InlineData("FurtherStudy")]
    public async Task SaveChangesAsync_WhenRelationshipHasNoStudent_PostgreSqlRejectsInsert(string relationship)
    {
        await using var context = fixture.CreateContext();
        switch (relationship)
        {
            case "Company":
                context.Companies.Add(new CompanyEntity
                {
                    Id = 0,
                    CompanyId = Guid.NewGuid(),
                    CompanyName = "Example Company",
                    Designation = "Engineer",
                    YearOfJoining = 2020,
                    AnnualSalary = 1200000,
                    StudentId = int.MaxValue,
                    Student = null!
                });
                break;
            case "Exam":
                context.Exams.Add(ExamRequest(Guid.NewGuid()).ToExam() with { StudentId = int.MaxValue });
                break;
            case "FurtherStudy":
                context.FurtherStudies.Add(StudyRequest(Guid.NewGuid()).ToFurtherStudy() with { StudentId = int.MaxValue });
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(relationship));
        }

        var exception = await Assert.ThrowsAsync<DbUpdateException>(
            () => context.SaveChangesAsync(TestContext.Current.CancellationToken));
        var postgresException = Assert.IsType<PostgresException>(exception.InnerException);
        Assert.Equal(PostgresErrorCodes.ForeignKeyViolation, postgresException.SqlState);
        await using var verification = fixture.CreateContext();
        Assert.False(await verification.Companies.AnyAsync(TestContext.Current.CancellationToken));
        Assert.False(await verification.Exams.AnyAsync(TestContext.Current.CancellationToken));
        Assert.False(await verification.FurtherStudies.AnyAsync(TestContext.Current.CancellationToken));
    }

    private static void AssertEmptyPage<T>(OneOf<PaginatedList<T>, ErrorType> result)
    {
        Assert.True(result.IsT0);
        Assert.Empty(result.AsT0.Items);
        Assert.Equal(0, result.AsT0.TotalCount);
        Assert.Equal(0, result.AsT0.TotalPages);
        Assert.False(result.AsT0.HasNextPage);
        Assert.False(result.AsT0.HasPreviousPage);
    }

    private static void AssertNotFound<T>(OneOf<T, ErrorType> result)
    {
        Assert.True(result.IsT1);
        Assert.Equal(ResponseStatus.NotFound, result.AsT1.Status);
        Assert.Equal("Student not found", result.AsT1.Message);
    }

    private static AddCompany CompanyRequest(Guid studentId) => new()
    {
        StudentId = studentId,
        CompanyId = Guid.NewGuid(),
        CompanyName = "  Example Company  ",
        Designation = "  Engineer  ",
        YearOfJoining = 2020,
        AnnualSalary = 1200000
    };

    private static AddExam ExamRequest(Guid studentId) => new()
    {
        StudentId = studentId,
        ExamId = Guid.NewGuid(),
        ExamName = "  GATE  ",
        Score = 750,
        Year = 2020
    };

    private static AddFurtherStudy StudyRequest(Guid studentId) => new()
    {
        StudentId = studentId,
        FurtherStudyId = Guid.NewGuid(),
        InstituteName = "  Example Institute  ",
        Degree = "  MSc  ",
        Country = "  India  ",
        City = "  Pune  ",
        AdmissionYear = 2020,
        PassingYear = 2022
    };
}
