namespace Alumni.IntegrationTests.Members;

[Collection("PostgreSQL")]
public sealed class MemberRelationshipTests(PostgreSqlFixture fixture) : IAsyncLifetime
{
    public ValueTask InitializeAsync() => new(fixture.ResetAsync());

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task Execute_WhenAddingCompany_PersistsNormalizedFieldsAndGetsOnlyRequestedMembersCompanies()
    {
        var member = await MemberTestData.AddAsync(fixture);
        var other = await MemberTestData.AddAsync(fixture, "other@example.com");
        await using var context = fixture.CreateContext();
        var result = await new AddCompanyHandler(context).Execute(CompanyRequest(member.MemberId), TestContext.Current.CancellationToken);
        var unrelated = await new AddCompanyHandler(context).Execute(
            CompanyRequest(other.MemberId) with { CompanyName = "Other Company" }, TestContext.Current.CancellationToken);

        Assert.True(result.IsT0);
        Assert.True(unrelated.IsT0);
        var response = result.AsT0;
        Assert.NotEqual(Guid.Empty, response.CompanyId);
        Assert.Equal("Example Company", response.CompanyName);
        Assert.Equal("Engineer", response.Designation);
        Assert.Equal(2020, response.YearOfJoining);
        Assert.Equal(1200000, response.AnnualSalary);

        await using var verification = fixture.CreateContext();
        var persisted = await verification.Companies.Include(item => item.Member)
            .SingleAsync(item => item.CompanyId == response.CompanyId, TestContext.Current.CancellationToken);
        Assert.True(persisted.Id > 0);
        Assert.Equal(member.MemberId, persisted.Member.MemberId);
        Assert.Equal(persisted.Member.Id, persisted.MemberId);
        Assert.Equal(response, persisted.ToCompanyResponse());
        var retrieved = await new GetCompanyHandler(verification).Execute(
            new GetCompany { MemberId = member.MemberId }, TestContext.Current.CancellationToken);
        Assert.True(retrieved.IsT0);
        Assert.Equal(response, Assert.Single(retrieved.AsT0.Items));
        Assert.Equal(1, retrieved.AsT0.TotalCount);
    }

    [Fact]
    public async Task Execute_WhenAddingExam_PersistsNormalizedFieldsAndGetsOnlyRequestedMembersExams()
    {
        var member = await MemberTestData.AddAsync(fixture);
        var other = await MemberTestData.AddAsync(fixture, "other@example.com");
        await using var context = fixture.CreateContext();
        var result = await new AddExamHandler(context).Execute(ExamRequest(member.MemberId), TestContext.Current.CancellationToken);
        var unrelated = await new AddExamHandler(context).Execute(
            ExamRequest(other.MemberId) with { ExamName = "Other Exam" }, TestContext.Current.CancellationToken);

        Assert.True(result.IsT0);
        Assert.True(unrelated.IsT0);
        var response = result.AsT0;
        Assert.NotEqual(Guid.Empty, response.ExamId);
        Assert.Equal("GATE", response.ExamName);
        Assert.Equal(750, response.Score);
        Assert.Equal(2020, response.Year);

        await using var verification = fixture.CreateContext();
        var persisted = await verification.Exams.Include(item => item.Member)
            .SingleAsync(item => item.ExamId == response.ExamId, TestContext.Current.CancellationToken);
        Assert.True(persisted.Id > 0);
        Assert.Equal(member.MemberId, persisted.Member.MemberId);
        Assert.Equal(persisted.Member.Id, persisted.MemberId);
        Assert.Equal(response, persisted.ToExamResponse());
        var retrieved = await new GetExamHandler(verification).Execute(
            new GetExam { MemberId = member.MemberId }, TestContext.Current.CancellationToken);
        Assert.True(retrieved.IsT0);
        Assert.Equal(response, Assert.Single(retrieved.AsT0.Items));
        Assert.Equal(1, retrieved.AsT0.TotalCount);
    }

    [Fact]
    public async Task Execute_WhenAddingFurtherStudy_PersistsNormalizedFieldsAndGetsOnlyRequestedMembersFurtherStudies()
    {
        var member = await MemberTestData.AddAsync(fixture);
        var other = await MemberTestData.AddAsync(fixture, "other@example.com");
        await using var context = fixture.CreateContext();
        var result = await new AddFurtherStudyHandler(context).Execute(StudyRequest(member.MemberId), TestContext.Current.CancellationToken);
        var unrelated = await new AddFurtherStudyHandler(context).Execute(
            StudyRequest(other.MemberId) with { InstituteName = "Other Institute" }, TestContext.Current.CancellationToken);

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
        var persisted = await verification.FurtherStudies.Include(item => item.Member)
            .SingleAsync(item => item.FurtherStudyId == response.FurtherStudyId, TestContext.Current.CancellationToken);
        Assert.True(persisted.Id > 0);
        Assert.Equal(member.MemberId, persisted.Member.MemberId);
        Assert.Equal(persisted.Member.Id, persisted.MemberId);
        Assert.Equal(response, persisted.ToFurtherStudyResponse());
        var retrieved = await new GetFurtherStudyHandler(verification).Execute(
            new GetFurtherStudy { MemberId = member.MemberId }, TestContext.Current.CancellationToken);
        Assert.True(retrieved.IsT0);
        Assert.Equal(response, Assert.Single(retrieved.AsT0.Items));
        Assert.Equal(1, retrieved.AsT0.TotalCount);
    }

    [Theory]
    [InlineData("Company")]
    [InlineData("Exam")]
    [InlineData("FurtherStudy")]
    public async Task Execute_WhenGettingRelationshipsForMemberWithoutAny_ReturnsEmptyPage(string relationship)
    {
        var member = await MemberTestData.AddAsync(fixture);
        await using var context = fixture.CreateContext();

        switch (relationship)
        {
            case "Company":
                AssertEmptyPage(await new GetCompanyHandler(context).Execute(
                    new GetCompany { MemberId = member.MemberId }, TestContext.Current.CancellationToken));
                break;
            case "Exam":
                AssertEmptyPage(await new GetExamHandler(context).Execute(
                    new GetExam { MemberId = member.MemberId }, TestContext.Current.CancellationToken));
                break;
            case "FurtherStudy":
                AssertEmptyPage(await new GetFurtherStudyHandler(context).Execute(
                    new GetFurtherStudy { MemberId = member.MemberId }, TestContext.Current.CancellationToken));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(relationship));
        }
    }

    [Theory]
    [InlineData("Company")]
    [InlineData("Exam")]
    [InlineData("FurtherStudy")]
    public async Task Execute_WhenGettingRelationshipsForMissingMember_ReturnsNotFound(string relationship)
    {
        await MemberTestData.AddAsync(fixture);
        var missingId = Guid.NewGuid();
        await using var context = fixture.CreateContext();

        switch (relationship)
        {
            case "Company":
                AssertNotFound(await new GetCompanyHandler(context).Execute(
                    new GetCompany { MemberId = missingId }, TestContext.Current.CancellationToken));
                break;
            case "Exam":
                AssertNotFound(await new GetExamHandler(context).Execute(
                    new GetExam { MemberId = missingId }, TestContext.Current.CancellationToken));
                break;
            case "FurtherStudy":
                AssertNotFound(await new GetFurtherStudyHandler(context).Execute(
                    new GetFurtherStudy { MemberId = missingId }, TestContext.Current.CancellationToken));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(relationship));
        }
    }

    [Theory]
    [InlineData("Company")]
    [InlineData("Exam")]
    [InlineData("FurtherStudy")]
    public async Task Execute_WhenAddingRelationshipForMissingMember_ReturnsNotFoundWithoutWritingRows(string relationship)
    {
        var member = await MemberTestData.AddAsync(fixture);
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
        var remainingMember = await verification.Members.SingleAsync(TestContext.Current.CancellationToken);
        Assert.Equal(member.MemberId, remainingMember.MemberId);
    }

    [Theory]
    [InlineData("Company")]
    [InlineData("Exam")]
    [InlineData("FurtherStudy")]
    public async Task SaveChangesAsync_WhenRelationshipHasNoMember_PostgreSqlRejectsInsert(string relationship)
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
                    MemberId = int.MaxValue,
                    Member = null!
                });
                break;
            case "Exam":
                context.Exams.Add(ExamRequest(Guid.NewGuid()).ToExam() with { MemberId = int.MaxValue });
                break;
            case "FurtherStudy":
                context.FurtherStudies.Add(StudyRequest(Guid.NewGuid()).ToFurtherStudy() with { MemberId = int.MaxValue });
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
        Assert.Equal("Member not found", result.AsT1.Message);
    }

    private static AddCompany CompanyRequest(Guid memberId) => new()
    {
        MemberId = memberId,
        CompanyId = Guid.NewGuid(),
        CompanyName = "  Example Company  ",
        Designation = "  Engineer  ",
        YearOfJoining = 2020,
        AnnualSalary = 1200000
    };

    private static AddExam ExamRequest(Guid memberId) => new()
    {
        MemberId = memberId,
        ExamId = Guid.NewGuid(),
        ExamName = "  GATE  ",
        Score = 750,
        Year = 2020
    };

    private static AddFurtherStudy StudyRequest(Guid memberId) => new()
    {
        MemberId = memberId,
        FurtherStudyId = Guid.NewGuid(),
        InstituteName = "  Example Institute  ",
        Degree = "  MSc  ",
        Country = "  India  ",
        City = "  Pune  ",
        AdmissionYear = 2020,
        PassingYear = 2022
    };
}
