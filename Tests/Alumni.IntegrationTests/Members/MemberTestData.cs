namespace Alumni.IntegrationTests.Members;

internal static class MemberTestData
{
    public static AddMember ValidRequest(string email = "member@example.com") => new()
    {
        MemberId = Guid.NewGuid(),
        FirstName = "Élodie",
        LastName = "D'Souza",
        Email = email,
        MobileNo = "9876543210",
        Extension = "+91",
        Gender = Gender.Female,
        DateOfBirth = new DateOnly(1998, 4, 12),
        Branch = Branch.IT,
        AdmissionYear = 2016,
        PassingYear = 2020,
        CurrentAddress = new Address
        {
            PinCode = "400001",
            Country = "India",
            State = "Maharashtra",
            City = "Mumbai",
            UserAddress = "12 Current Street"
        },
        CorrespondenceAddress = new Address
        {
            PinCode = "560001",
            Country = "India",
            State = "Karnataka",
            City = "Bengaluru",
            UserAddress = "34 Correspondence Road"
        }
    };

    public static async Task<MemberResponse> AddAsync(PostgreSqlFixture fixture, string email = "member@example.com")
    {
        await using var context = fixture.CreateContext();
        var result = await new AddMemberHandler(context).Execute(ValidRequest(email), TestContext.Current.CancellationToken);
        Assert.True(result.IsT0);
        return result.AsT0;
    }
}
