using Microsoft.EntityFrameworkCore;

namespace Domain;

public static class PaginationQuery
{
    public static async Task<PaginatedList<T>> Paginate<T>(this IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var count = await source.CountAsync(cancellationToken);
        var items = await source.Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<T>
        {
            Items = items, TotalCount = count, PageNumber = pageNumber, PageSize = pageSize,
        };
    }

    public static PaginatedList<TOutput> WithItems<TInput, TOutput>(this PaginatedList<TInput> source, Func<TInput, TOutput> selector) =>
        new()
        {
            Items = source.Items.Select(selector).ToList(),
            TotalCount = source.TotalCount,
            PageNumber = source.PageNumber,
            PageSize = source.PageSize,
        };
}
