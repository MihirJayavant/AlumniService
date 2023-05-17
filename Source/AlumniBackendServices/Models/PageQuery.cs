namespace AlumniBackendServices.Models;

public class PageQuery
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }

    public static ValueTask<PageQuery?> BindAsync(HttpContext context)
    {
        const string pageNumber = "pageNumber";
        const string pageSize = "pageSize";

        _ = int.TryParse(context.Request.Query[pageNumber], out var page);
        _ = int.TryParse(context.Request.Query[pageSize], out var size);

        page = page == 0 ? 1 : page;
        size = size == 0 ? 100 : size;

        var result = new PageQuery
        {
            PageNumber = page,
            PageSize = size
        };

        return ValueTask.FromResult<PageQuery?>(result);
    }

    // public static bool TryParse(string query, out PageQuery page)
    // {
    //     System.Console.WriteLine(query);
    //     page = new()
    //     {
    //         PageNumber = 1,
    //         PageSize = 10
    //     };

    //     return true;
    // }
}
