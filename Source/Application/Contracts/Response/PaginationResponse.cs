namespace Application.Contracts.Response;

public class PaginationResponse<T>
{
    public T Data { get; set; } = default!;
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
}
