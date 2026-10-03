namespace Alumni.Student.FurtherStudy;

[RecordView(typeof(FurtherStudy), nameof(FurtherStudy.Id))]
public sealed partial record AddFurtherStudy
{
    public required Guid StudentId { get; init; }
}

public sealed class AddFurtherStudyValidator : AbstractValidator<AddFurtherStudy>
{
    public AddFurtherStudyValidator()
    {
        RuleFor(x => x.StudentId).ValidGuid();
        RuleFor(x => x.InstituteName).RequiredText(50);
        RuleFor(x => x.Degree).RequiredText(50);
        RuleFor(x => x.Country).RequiredText(30);
        RuleFor(x => x.City).RequiredText(30);
        RuleFor(x => x.AdmissionYear).InclusiveBetween(1, 9999);
        RuleFor(x => x.PassingYear).Cascade(CascadeMode.Stop).InclusiveBetween(1, 9999)
            .GreaterThanOrEqualTo(x => x.AdmissionYear);
    }
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

        var furtherStudy = request.ToFurtherStudy();
        student.FurtherStudies.Add(furtherStudy);
        await context.SaveChangesAsync(cancellationToken);

        return furtherStudy.ToFurtherStudyResponse();
    }
}

public static class AddFurtherStudyMapper
{
    public static FurtherStudyEntity ToFurtherStudy(this AddFurtherStudy request)
        => new FurtherStudyEntity
        {
            Id = 0,
            FurtherStudyId = Guid.CreateVersion7(),
            InstituteName = request.InstituteName.Trim(),
            Degree = request.Degree.Trim(),
            AdmissionYear = request.AdmissionYear,
            PassingYear = request.PassingYear,
            Country = request.Country.Trim(),
            City = request.City.Trim(),
        };
}
