namespace Alumni.Student.FurtherStudy;

public record GetFurtherStudy
{
    public Guid StudentId { get; init; }
}

file sealed class GetFurtherStudyValidator : AbstractValidator<GetFurtherStudy>
{

}

public class GetFurtherStudyHandler(IStudentDbContext context)
    : IHandler<GetFurtherStudy, PaginatedList<FurtherStudyResponse>>
{
    public AbstractValidator<GetFurtherStudy> Validator { get; } = new GetFurtherStudyValidator();

    public async Task<OneOf<PaginatedList<FurtherStudyResponse>, ErrorType>> Handle(GetFurtherStudy request, CancellationToken cancellationToken)
    {
        var student = await context.Students.FirstOrDefaultAsync(s => s.StudentId == request.StudentId, cancellationToken);
        if (student is null)
        {
            return new ErrorType { Message = "Student not found", Status = ResponseStatus.NotFound };
        }

        var result = await context.FurtherStudies
            .Where(s => s.StudentId == student.Id)
            .Paginate(1, 50, cancellationToken);

        return result.WithItems(f => f.ToFurtherStudyResponse());
    }
}
