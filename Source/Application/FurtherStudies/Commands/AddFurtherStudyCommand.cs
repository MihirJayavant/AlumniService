namespace Application.FurtherStudies;

public class AddFurtherStudyCommand : IRequest<OneOf<FurtherStudyResponse, ErrorType>>
{
    public required string InstituteName { get; init; }
    public required string Degree { get; init; }
    public required int AdmissionYear { get; init; }
    public required int PassingYear { get; init; }
    public required string Country { get; init; }
    public required string City { get; init; }
    public required int StudentId { get; init; }
}

public sealed class AddFurtherStudyValidator : AbstractValidator<AddFurtherStudyCommand>
{
    public AddFurtherStudyValidator()
        => RuleFor(x => x.StudentId)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Id should be at least greater than or equal to 1.");
}

public class AddFurtherStudyHandler : IRequestHandler<AddFurtherStudyCommand, OneOf<FurtherStudyResponse, ErrorType>>
{
    private readonly IApplicationDbContext context;
    private readonly IMapper mapper;

    public AddFurtherStudyHandler(IApplicationDbContext context, IMapper mapper)
                => (this.context, this.mapper) = (context, mapper);

    public Task<OneOf<FurtherStudyResponse, ErrorType>> Handle(AddFurtherStudyCommand request, CancellationToken cancellationToken)
    {
        return ValidationHelper.ValidateAndRun(request, new AddFurtherStudyValidator(), GetData);

        async Task<OneOf<FurtherStudyResponse, ErrorType>> GetData()
        {
            var student = await context.Students
                            .FirstOrDefaultAsync(s => s.StudentId == request.StudentId, cancellationToken);

            if (student is null)
            {
                return new ErrorType(ResponseStatus.NotFound, "Student not found");
            }

            var furtherStudy = mapper.Map<FurtherStudy>(request);
            await context.FurtherStudies.AddAsync(furtherStudy, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            var response = mapper.Map<FurtherStudyResponse>(furtherStudy);

            return response;
        }
    }

}
