using Xunit;

namespace Core.UnitTests.Pagination;

public class PaginationInputTests
{
    [Fact]
    public void Constructor_WhenNoValuesAreProvided_UsesFirstPageAndTenItems()
    {
        var input = new PaginationInput();

        Assert.Equal(1, input.PageNumber);
        Assert.Equal(10, input.PageSize);
    }
}
