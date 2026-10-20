namespace Alumni.IntegrationTests.Members;

[Collection("PostgreSQL")]
public sealed class MemberHandlerTests(PostgreSqlFixture fixture) : IAsyncLifetime
{
    private static readonly int[] SeedIds = [30, 10, 50, 20, 40];

    public ValueTask InitializeAsync() => new(fixture.ResetAsync());

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task Execute_WhenAddingMember_PersistsNormalizedFieldsAddressesAndAuditTimestamps()
    {
        var request = MemberTestData.ValidRequest() with
        {
            Email = "  MEMBER@Example.COM  ",
            FirstName = "  Élodie  ",
            LastName = "  D'Souza  ",
            MobileNo = "  009876543210  ",
            Extension = " +91 ",
            Gender = " female ",
            Branch = " IT ",
            CurrentAddress = new Address
            {
                PinCode = " 400 001 ",
                Country = " India ",
                State = " Maharashtra ",
                City = " Navi  Mumbai ",
                UserAddress = " 12 Current Street "
            },
            CorrespondenceAddress = new Address
            {
                PinCode = " SW1A 1AA ",
                Country = " United Kingdom ",
                State = " Greater London ",
                City = " London ",
                UserAddress = " 34 King's Road "
            }
        };
        await using var context = fixture.CreateContext();
        var before = DateTime.UtcNow;

        var result = await new AddMemberHandler(context).Execute(request, TestContext.Current.CancellationToken);

        var after = DateTime.UtcNow;
        Assert.True(result.IsT0);
        var response = result.AsT0;
        Assert.NotEqual(Guid.Empty, response.MemberId);
        Assert.NotEqual(request.MemberId, response.MemberId);
        Assert.Equal("member@example.com", response.Email);
        Assert.Equal("Élodie", response.FirstName);
        Assert.Equal("D'Souza", response.LastName);
        Assert.Equal("009876543210", response.MobileNo);
        Assert.Equal("+91", response.Extension);
        Assert.Equal(Gender.Female, response.Gender);
        Assert.Equal(Branch.IT, response.Branch);
        Assert.Equal(request.DateOfBirth, response.DateOfBirth);
        Assert.Equal(request.AdmissionYear, response.AdmissionYear);
        Assert.Equal(request.PassingYear, response.PassingYear);
        Assert.Equal(new Address
        {
            PinCode = "400 001",
            Country = "India",
            State = "Maharashtra",
            City = "Navi  Mumbai",
            UserAddress = "12 Current Street"
        }, response.CurrentAddress);
        Assert.Equal(new Address
        {
            PinCode = "SW1A 1AA",
            Country = "United Kingdom",
            State = "Greater London",
            City = "London",
            UserAddress = "34 King's Road"
        }, response.CorrespondenceAddress);

        await using var verification = fixture.CreateContext();
        var persisted = await verification.Members.SingleAsync(TestContext.Current.CancellationToken);
        Assert.True(persisted.Id > 0);
        Assert.Equal(response, persisted.ToMemberResponse());
        // PostgreSQL truncates DateTime ticks to microseconds.
        Assert.InRange(persisted.CreatedAt, before.AddTicks(-10), after);
        Assert.NotNull(persisted.UpdatedAt);
        Assert.InRange(persisted.UpdatedAt.Value, persisted.CreatedAt, after);
        Assert.Equal(DateTimeKind.Utc, persisted.CreatedAt.Kind);
        Assert.Equal(DateTimeKind.Utc, persisted.UpdatedAt.Value.Kind);
        Assert.Null(persisted.AuthUserId);
        Assert.False(persisted.IsDeleted);
    }

    [Fact]
    public async Task Execute_WhenNormalizedEmailExists_ReturnsConflictWithoutChangingMember()
    {
        var existing = await MemberTestData.AddAsync(fixture);
        await using var context = fixture.CreateContext();

        var result = await new AddMemberHandler(context).Execute(
            MemberTestData.ValidRequest("  MEMBER@EXAMPLE.COM  "), TestContext.Current.CancellationToken);

        Assert.True(result.IsT1);
        Assert.Equal(ResponseStatus.Conflict, result.AsT1.Status);
        await using var verification = fixture.CreateContext();
        var persisted = await verification.Members.SingleAsync(TestContext.Current.CancellationToken);
        Assert.Equal(existing, persisted.ToMemberResponse());
    }

    [Fact]
    public async Task Execute_WhenGettingExistingMember_ReturnsPersistedFieldsAndBothAddresses()
    {
        var existing = await MemberTestData.AddAsync(fixture);
        await using var context = fixture.CreateContext();

        var result = await new GetMemberHandler(context).Execute(
            new GetMember { Id = existing.MemberId }, TestContext.Current.CancellationToken);

        Assert.True(result.IsT0);
        Assert.Equal(existing, result.AsT0);
    }

    [Fact]
    public async Task Execute_WhenGettingMissingMember_ReturnsNotFound()
    {
        await MemberTestData.AddAsync(fixture);
        await using var context = fixture.CreateContext();

        var result = await new GetMemberHandler(context).Execute(
            new GetMember { Id = Guid.NewGuid() }, TestContext.Current.CancellationToken);

        Assert.True(result.IsT1);
        Assert.Equal(ResponseStatus.NotFound, result.AsT1.Status);
    }

    [Theory]
    [InlineData(1, false, true, new[] { 10, 20 })]
    [InlineData(2, true, true, new[] { 30, 40 })]
    [InlineData(3, true, false, new[] { 50 })]
    [InlineData(4, true, false, new int[0])]
    public async Task Execute_WhenListingMembers_ReturnsOrderedPageAndAccurateTotals(
        int pageNumber, bool hasPreviousPage, bool hasNextPage, int[] expectedIds)
    {
        await using (var seed = fixture.CreateContext())
        {
            seed.Members.AddRange(SeedIds.Select(id =>
                MemberTestData.ValidRequest($"member{id}@example.com").ToMember() with { Id = id }));
            await seed.SaveChangesAsync(TestContext.Current.CancellationToken);
        }
        await using var context = fixture.CreateContext();

        var result = await new GetMembersHandler(context).Execute(
            new GetMembers { PageNumber = pageNumber, PageSize = 2 }, TestContext.Current.CancellationToken);

        Assert.True(result.IsT0);
        var page = result.AsT0;
        Assert.Equal(expectedIds.Select(id => $"member{id}@example.com"), page.Items.Select(item => item.Email));
        Assert.Equal(5, page.TotalCount);
        Assert.Equal(3, page.TotalPages);
        Assert.Equal(pageNumber, page.PageNumber);
        Assert.Equal(2, page.PageSize);
        Assert.Equal(hasPreviousPage, page.HasPreviousPage);
        Assert.Equal(hasNextPage, page.HasNextPage);
    }

    [Fact]
    public async Task Execute_WhenListingEmptyMemberTable_ReturnsEmptyPage()
    {
        await using var context = fixture.CreateContext();

        var result = await new GetMembersHandler(context).Execute(
            new GetMembers { PageNumber = 1, PageSize = 2 }, TestContext.Current.CancellationToken);

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
        var existing = await MemberTestData.AddAsync(fixture);
        await using var context = fixture.CreateContext();
        context.Members.Add(MemberTestData.ValidRequest(existing.Email).ToMember());

        var exception = await Assert.ThrowsAsync<DbUpdateException>(
            () => context.SaveChangesAsync(TestContext.Current.CancellationToken));

        var postgresException = Assert.IsType<PostgresException>(exception.InnerException);
        Assert.Equal(PostgresErrorCodes.UniqueViolation, postgresException.SqlState);
        await using var verification = fixture.CreateContext();
        var persisted = await verification.Members.SingleAsync(TestContext.Current.CancellationToken);
        Assert.Equal(existing, persisted.ToMemberResponse());
    }
}
