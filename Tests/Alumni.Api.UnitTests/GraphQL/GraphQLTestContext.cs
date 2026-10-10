using System.Collections.Concurrent;
using System.Text.Json;
using Alumni.Api.ExtensionService;
using Alumni.Faculty;
using Alumni.Student;
using Alumni.Student.Company;
using Alumni.Student.Exam;
using Alumni.Student.FurtherStudy;
using HotChocolate;
using HotChocolate.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Alumni.Api.UnitTests.GraphQL;

internal sealed class GraphQLTestContext : IAsyncDisposable
{
    public const string EmptyId = "00000000-0000-0000-0000-000000000000";
    public const string ValidId = "11111111-1111-1111-1111-111111111111";

    private readonly ServiceProvider services;

    public GraphQLTestContext(bool includeScalarFixture = false)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(Accesses);
        services.AddScoped<RejectingContext>();
        services.AddScoped<IStudentDbContext>(provider => provider.GetRequiredService<RejectingContext>());
        services.AddScoped<IFacultyDbContext>(provider => provider.GetRequiredService<RejectingContext>());
        HandlerTestRegistration.AddDomainHandlers(services);
        services.AddApplicationGraphQL();
        if (includeScalarFixture)
        {
            services.AddGraphQLServer().AddTypeExtension<GraphQLScalarFixtureQuery>();
        }
        this.services = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });
    }

    public ConcurrentQueue<Guid> Accesses { get; } = new();

    public async Task<IRequestExecutor> GetExecutorAsync() => await services.GetRequestExecutorAsync();

    public async Task<JsonElement> ExecuteAsync(string document)
    {
        var executor = await GetExecutorAsync();
        var request = OperationRequestBuilder.New().SetDocument(document).Build();
        await using var result = await executor.ExecuteAsync(request);
        using var json = JsonDocument.Parse(result.ToJson());
        return json.RootElement.Clone();
    }

    public ValueTask DisposeAsync() => services.DisposeAsync();

    private sealed class RejectingContext(ConcurrentQueue<Guid> accesses) : IStudentDbContext, IFacultyDbContext
    {
        private readonly Guid instanceId = Guid.NewGuid();

        public DbSet<StudentEntity> Students => Reject<StudentEntity>();
        public DbSet<CompanyEntity> Companies => Reject<CompanyEntity>();
        public DbSet<ExamEntity> Exams => Reject<ExamEntity>();
        public DbSet<FurtherStudyEntity> FurtherStudies => Reject<FurtherStudyEntity>();
        public DbSet<Alumni.Faculty.Faculty> Faculties => Reject<Alumni.Faculty.Faculty>();

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            accesses.Enqueue(instanceId);
            throw new InvalidOperationException("Unexpected unit-test database save.");
        }

        private DbSet<T> Reject<T>() where T : class
        {
            accesses.Enqueue(instanceId);
            throw new InvalidOperationException("Private provider failure.");
        }
    }
}
