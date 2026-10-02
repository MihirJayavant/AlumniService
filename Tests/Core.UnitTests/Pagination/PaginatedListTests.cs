using Xunit;

namespace Core.UnitTests.Pagination;

public class PaginatedListTests
{
    [Theory]
    [InlineData(0, 10, 0)]
    [InlineData(10, 10, 1)]
    [InlineData(20, 10, 2)]
    [InlineData(1, 10, 1)]
    [InlineData(21, 10, 3)]
    public void TotalPages_WhenCountIsProvided_RoundsUpPartialPages(int totalCount, int pageSize, int expectedPages)
    {
        var page = new PaginatedList<int>
        {
            Items = [],
            TotalCount = totalCount,
            PageNumber = 1,
            PageSize = pageSize,
        };

        Assert.Equal(expectedPages, page.TotalPages);
    }

    [Theory]
    [InlineData(0, 1, false, false)]
    [InlineData(5, 1, false, false)]
    [InlineData(25, 1, false, true)]
    [InlineData(25, 2, true, true)]
    [InlineData(25, 3, true, false)]
    public void Navigation_WhenPagePositionChanges_IdentifiesAdjacentPages(
        int totalCount, int pageNumber, bool hasPreviousPage, bool hasNextPage)
    {
        var page = new PaginatedList<int>
        {
            Items = [],
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = 10,
        };

        Assert.Equal(hasPreviousPage, page.HasPreviousPage);
        Assert.Equal(hasNextPage, page.HasNextPage);
    }
}
