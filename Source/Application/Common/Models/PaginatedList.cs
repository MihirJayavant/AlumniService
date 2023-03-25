using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Models;

public record class PaginatedList<T>
{
    public ImmutableList<T> Items { get; }
    public int PageSize { get; }
    public int PageIndex { get; }
    public int TotalPages { get; }
    public int TotalCount { get; }

    public PaginatedList(ImmutableList<T> items, int count, int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
        Items = items;
    }

    public bool HasPreviousPage => PageIndex > 1;

    public bool HasNextPage => PageIndex < TotalPages;

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize, CancellationToken cancellationToken = default)
    {
        var count = await source.CountAsync(cancellationToken);
        var items = await source.Skip((pageIndex - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync(cancellationToken);

        return new PaginatedList<T>(items.ToImmutableList(), count, pageIndex, pageSize);
    }
}
