namespace Alumni.IntegrationTests.Staff;

[Collection("PostgreSQL")]
public sealed class StaffMemberHandlerTests(PostgreSqlFixture fixture) : IAsyncLifetime
{
    public ValueTask InitializeAsync() => new(fixture.ResetAsync());

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task Execute_WhenAddingFaculty_PersistsNormalizedFieldsAndAuditTimestamps()
    {
        await using var context = fixture.CreateContext();
        var request = ValidRequest() with
        {
            Email = "  ALICE@Example.COM  ",
            FirstName = "  Alice  ",
            LastName = "  D'Souza  ",
            Extension = "  +91  "
        };
        var before = DateTime.UtcNow;

        var result = await new AddStaffMemberHandler(context).Execute(request, TestContext.Current.CancellationToken);

        var after = DateTime.UtcNow;
        Assert.True(result.IsT0);
        var response = result.AsT0;
        Assert.NotEqual(Guid.Empty, response.FacultyId);
        Assert.Equal("alice@example.com", response.Email);
        Assert.Equal("Alice", response.FirstName);
        Assert.Equal("D'Souza", response.LastName);
        Assert.Equal("+91", response.Extension);
        Assert.Equal(request.MobileNo, response.MobileNo);
        Assert.InRange(response.CreatedAt, before, after);
        Assert.Equal(response.CreatedAt, response.UpdatedAt);

        await using var verification = fixture.CreateContext();
        var persisted = await verification.StaffMembers.SingleAsync(TestContext.Current.CancellationToken);
        Assert.True(persisted.Id > 0);
        Assert.Equal(response.FacultyId, persisted.FacultyId);
        Assert.Equal(response.Email, persisted.Email);
        Assert.Equal(response.FirstName, persisted.FirstName);
        Assert.Equal(response.LastName, persisted.LastName);
        Assert.Equal(response.Extension, persisted.Extension);
        Assert.Equal(response.MobileNo, persisted.MobileNo);
        // PostgreSQL stores timestamps at microsecond precision.
        Assert.InRange((persisted.CreatedAt - response.CreatedAt).Duration(), TimeSpan.Zero, TimeSpan.FromMilliseconds(1));
        Assert.Equal(DateTimeKind.Utc, persisted.CreatedAt.Kind);
        Assert.Equal(persisted.CreatedAt, persisted.UpdatedAt);
        Assert.False(persisted.IsDeleted);
        Assert.Null(persisted.AuthUserId);
    }

    [Fact]
    public async Task Execute_WhenNormalizedEmailAlreadyExists_ReturnsConflictWithoutInsertingFaculty()
    {
        var existing = Assert.Single(await SeedAsync(10));
        await using var context = fixture.CreateContext();

        var result = await new AddStaffMemberHandler(context).Execute(
            ValidRequest() with { Email = $"  {existing.Email.ToUpperInvariant()}  " },
            TestContext.Current.CancellationToken);

        Assert.True(result.IsT1);
        Assert.Equal(ResponseStatus.Conflict, result.AsT1.Status);
        Assert.Equal("Faculty with this email already exists", result.AsT1.Message);
        await using var verification = fixture.CreateContext();
        var persisted = await verification.StaffMembers.SingleAsync(TestContext.Current.CancellationToken);
        Assert.Equal(existing, persisted.ToStaffMemberResponse());
    }

    [Fact]
    public async Task Execute_WhenGettingExistingFaculty_ReturnsPersistedResponse()
    {
        var existing = Assert.Single(await SeedAsync(10));
        await using var context = fixture.CreateContext();

        var result = await new GetStaffMemberHandler(context).Execute(
            new GetStaffMember { FacultyId = existing.FacultyId }, TestContext.Current.CancellationToken);

        Assert.True(result.IsT0);
        Assert.Equal(existing, result.AsT0);
    }

    [Fact]
    public async Task Execute_WhenGettingMissingFaculty_ReturnsNotFound()
    {
        await SeedAsync(10);
        await using var context = fixture.CreateContext();

        var result = await new GetStaffMemberHandler(context).Execute(
            new GetStaffMember { FacultyId = Guid.NewGuid() }, TestContext.Current.CancellationToken);

        Assert.True(result.IsT1);
        Assert.Equal(ResponseStatus.NotFound, result.AsT1.Status);
        Assert.Equal("Faculty not found", result.AsT1.Message);
    }

    [Fact]
    public async Task Execute_WhenDeletingExistingFaculty_RemovesOnlyRequestedRowAndReturnsItsResponse()
    {
        var seeded = await SeedAsync(10, 20);
        await using var context = fixture.CreateContext();

        var result = await new DeleteStaffMemberHandler(context).Execute(
            new DeleteStaffMember { FacultyId = seeded[0].FacultyId }, TestContext.Current.CancellationToken);

        Assert.True(result.IsT0);
        Assert.Equal(seeded[0], result.AsT0);
        await using var verification = fixture.CreateContext();
        var remaining = await verification.StaffMembers.SingleAsync(TestContext.Current.CancellationToken);
        Assert.Equal(seeded[1], remaining.ToStaffMemberResponse());
    }

    [Fact]
    public async Task Execute_WhenDeletingMissingFaculty_ReturnsNotFoundWithoutChangingRows()
    {
        var existing = Assert.Single(await SeedAsync(10));
        await using var context = fixture.CreateContext();

        var result = await new DeleteStaffMemberHandler(context).Execute(
            new DeleteStaffMember { FacultyId = Guid.NewGuid() }, TestContext.Current.CancellationToken);

        Assert.True(result.IsT1);
        Assert.Equal(ResponseStatus.NotFound, result.AsT1.Status);
        Assert.Equal("Faculty not found", result.AsT1.Message);
        await using var verification = fixture.CreateContext();
        var persisted = await verification.StaffMembers.SingleAsync(TestContext.Current.CancellationToken);
        Assert.Equal(existing, persisted.ToStaffMemberResponse());
    }

    [Theory]
    [InlineData(1, false, true, new[] { 10, 20 })]
    [InlineData(2, true, true, new[] { 30, 40 })]
    [InlineData(3, true, false, new[] { 50 })]
    [InlineData(4, true, false, new int[0])]
    public async Task Execute_WhenListingFaculty_ReturnsPageOrderedByIdWithAccurateMetadata(
        int pageNumber, bool hasPreviousPage, bool hasNextPage, int[] expectedIds)
    {
        await SeedAsync(30, 10, 50, 20, 40);
        await using var context = fixture.CreateContext();

        var result = await new GetStaffMembersHandler(context).Execute(
            new GetStaffMembers { PageNumber = pageNumber, PageSize = 2 }, TestContext.Current.CancellationToken);

        Assert.True(result.IsT0);
        var page = result.AsT0;
        Assert.Equal(expectedIds.Select(id => $"faculty{id}@example.com"), page.Items.Select(item => item.Email));
        Assert.Equal(5, page.TotalCount);
        Assert.Equal(3, page.TotalPages);
        Assert.Equal(pageNumber, page.PageNumber);
        Assert.Equal(2, page.PageSize);
        Assert.Equal(hasPreviousPage, page.HasPreviousPage);
        Assert.Equal(hasNextPage, page.HasNextPage);
    }

    [Fact]
    public async Task Execute_WhenListingEmptyFacultyTable_ReturnsEmptyPageWithZeroTotals()
    {
        await using var context = fixture.CreateContext();

        var result = await new GetStaffMembersHandler(context).Execute(
            new GetStaffMembers { PageNumber = 1, PageSize = 2 }, TestContext.Current.CancellationToken);

        Assert.True(result.IsT0);
        var page = result.AsT0;
        Assert.Empty(page.Items);
        Assert.Equal(0, page.TotalCount);
        Assert.Equal(0, page.TotalPages);
        Assert.Equal(1, page.PageNumber);
        Assert.Equal(2, page.PageSize);
        Assert.False(page.HasPreviousPage);
        Assert.False(page.HasNextPage);
    }

    [Fact]
    public async Task SaveChangesAsync_WhenDuplicateEmailBypassesHandler_PostgreSqlRejectsInsert()
    {
        var existing = Assert.Single(await SeedAsync(10));
        await using var context = fixture.CreateContext();
        context.StaffMembers.Add(Entity(20) with { Email = existing.Email });

        var exception = await Assert.ThrowsAsync<DbUpdateException>(
            () => context.SaveChangesAsync(TestContext.Current.CancellationToken));

        var postgresException = Assert.IsType<PostgresException>(exception.InnerException);
        Assert.Equal(PostgresErrorCodes.UniqueViolation, postgresException.SqlState);
        await using var verification = fixture.CreateContext();
        var persisted = await verification.StaffMembers.SingleAsync(TestContext.Current.CancellationToken);
        Assert.Equal(existing, persisted.ToStaffMemberResponse());
    }

    private async Task<StaffMemberResponse[]> SeedAsync(params int[] ids)
    {
        await using var context = fixture.CreateContext();
        var entities = ids.Select(Entity).ToArray();
        context.StaffMembers.AddRange(entities);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        return entities.Select(entity => entity.ToStaffMemberResponse()).ToArray();
    }

    private static StaffMemberEntity Entity(int id) => new()
    {
        Id = id,
        FacultyId = Guid.NewGuid(),
        Email = $"faculty{id}@example.com",
        FirstName = $"Faculty{id}",
        LastName = "Example",
        Extension = "+91",
        MobileNo = 9876543210,
        CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
    };

    private static AddStaffMember ValidRequest() => new()
    {
        Email = "alice@example.com",
        FirstName = "Alice",
        LastName = "D'Souza",
        Extension = "+91",
        MobileNo = 9876543210
    };
}
