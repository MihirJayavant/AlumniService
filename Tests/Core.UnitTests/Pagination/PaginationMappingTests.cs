using Xunit;

namespace Core.UnitTests.Pagination;

public class PaginationMappingTests
{
    [Fact]
    public void WithItems_WhenItemsExist_MapsInOrderAndPreservesPaginationMetadata()
    {
        var source = new PaginatedList<int>
        {
            Items = [3, 1, 2],
            TotalCount = 8,
            PageNumber = 2,
            PageSize = 3,
        };

        var mapped = source.WithItems(item => $"Item {item}");

        Assert.Equal(["Item 3", "Item 1", "Item 2"], mapped.Items);
        Assert.Equal(8, mapped.TotalCount);
        Assert.Equal(2, mapped.PageNumber);
        Assert.Equal(3, mapped.PageSize);
        Assert.Equal(3, mapped.TotalPages);
        Assert.True(mapped.HasPreviousPage);
        Assert.True(mapped.HasNextPage);
    }

    [Fact]
    public void WithItems_WhenItemsAreEmpty_ReturnsEmptyPageWithoutInvokingSelector()
    {
        var source = new PaginatedList<int>
        {
            Items = [],
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 10,
        };
        var selectorCalls = 0;

        var mapped = source.WithItems(item =>
        {
            selectorCalls++;
            return item.ToString(System.Globalization.CultureInfo.InvariantCulture);
        });

        Assert.Empty(mapped.Items);
        Assert.Equal(0, selectorCalls);
        Assert.Equal(0, mapped.TotalCount);
        Assert.Equal(1, mapped.PageNumber);
        Assert.Equal(10, mapped.PageSize);
        Assert.Equal(0, mapped.TotalPages);
        Assert.False(mapped.HasPreviousPage);
        Assert.False(mapped.HasNextPage);
    }

    [Fact]
    public void WithItems_WhenMappingItems_LeavesSourceUnchanged()
    {
        int[] originalItems = [4, 7];
        var source = new PaginatedList<int>
        {
            Items = originalItems,
            TotalCount = 5,
            PageNumber = 2,
            PageSize = 2,
        };

        var mapped = source.WithItems(item => item * 2);

        Assert.Equal([8, 14], mapped.Items);
        Assert.NotSame(source, mapped);
        Assert.NotSame(originalItems, mapped.Items);
        Assert.Same(originalItems, source.Items);
        Assert.Equal([4, 7], source.Items);
        Assert.Equal(5, source.TotalCount);
        Assert.Equal(2, source.PageNumber);
        Assert.Equal(2, source.PageSize);
        Assert.Equal(3, source.TotalPages);
        Assert.True(source.HasPreviousPage);
        Assert.True(source.HasNextPage);
    }
}
