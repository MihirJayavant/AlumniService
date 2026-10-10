using Alumni.Api.GraphQL;
using Alumni.Faculty;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Alumni.Api.UnitTests.GraphQL;

public class GraphQLCancellationTests
{
    [Fact]
    public async Task GetFacultyAsync_WhenRequestAlreadyCancelled_PropagatesCancellationBeforeDatabaseAccess()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var context = new RejectingFacultyContext();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            new FacultyQuery().GetFacultyAsync(Guid.NewGuid(), new GetFacultyHandler(context), cancellation.Token));

        Assert.Equal(0, context.AccessCount);
    }

    [Fact]
    public async Task DeleteFacultyAsync_WhenRequestAlreadyCancelled_PropagatesCancellationBeforeDatabaseAccess()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var context = new RejectingFacultyContext();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            new FacultyMutation().DeleteFacultyAsync(Guid.NewGuid(), new DeleteFacultyHandler(context), cancellation.Token));

        Assert.Equal(0, context.AccessCount);
    }

    private sealed class RejectingFacultyContext : IFacultyDbContext
    {
        public int AccessCount { get; private set; }

        public DbSet<Alumni.Faculty.Faculty> Faculties
        {
            get
            {
                AccessCount++;
                throw new InvalidOperationException("Cancelled requests must not access the database.");
            }
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            AccessCount++;
            throw new InvalidOperationException("Cancelled requests must not save changes.");
        }
    }
}
