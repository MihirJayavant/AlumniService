using System.Reflection;

namespace AlumniBackendServices.Models;

public record PageQuery
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public static ValueTask<PageQuery> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        const string pageNumber = "pageNumber";
        const string pageSize = "pageSize";

        _ = int.TryParse(context.Request.Query[pageNumber], out var page);
        _ = int.TryParse(context.Request.Query[pageSize], out var size);

        page = page == 0 ? 1 : page;

        var result = new PageQuery
        {
            PageNumber = page,
            PageSize = size
        };

        return ValueTask.FromResult(result);
    }
}
