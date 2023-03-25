using System.Collections.Immutable;
using Application.Common.Models;

namespace Application.Students;

public record class AllStudentResponse : PaginatedList<StudentResponse>
{
    public AllStudentResponse(ImmutableList<StudentResponse> items, int count, int pageIndex, int pageSize) : base(items, count, pageIndex, pageSize)
    {
    }

    public AllStudentResponse(PaginatedList<StudentResponse> data)
        : base(data.Items, data.TotalCount, data.PageIndex, data.PageSize)
    {

    }
}

