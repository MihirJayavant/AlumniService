namespace Alumni.Student.FurtherStudy;

[RecordView(typeof(FurtherStudy), nameof(FurtherStudy.Id))]
public sealed partial record AddFurtherStudy
{
    public required Guid StudentId { get; init; }
}

public sealed class AddFurtherStudyValidator : AbstractValidator<AddFurtherStudy>
{

}

public class AddFurtherStudyHandler(IStudentDbContext context)
    : IHandler<AddFurtherStudy, FurtherStudyResponse>
{
    public AbstractValidator<AddFurtherStudy> Validator { get; } = new AddFurtherStudyValidator();

    public async Task<OneOf<FurtherStudyResponse, ErrorType>> Handle(AddFurtherStudy request, CancellationToken cancellationToken)
    {
        var student = await context.Students
                        .FirstOrDefaultAsync(s => s.StudentId == request.StudentId, cancellationToken);

        if (student is null)
        {
            return new ErrorType() { Status = ResponseStatus.NotFound, Message = "Student not found" };
        }

        var furtherStudy = new FurtherStudyEntity()
        {
            Id = 0,
            FurtherStudyId = Guid.CreateVersion7(),
            InstituteName = request.InstituteName,
            Degree = request.Degree,
            AdmissionYear = request.AdmissionYear,
            PassingYear = request.PassingYear,
            Country = request.Country,
            City = request.City,
        };
        student.FurtherStudies.Add(furtherStudy);
        await context.SaveChangesAsync(cancellationToken);

        return furtherStudy.ToFurtherStudyResponse();
    }
}
