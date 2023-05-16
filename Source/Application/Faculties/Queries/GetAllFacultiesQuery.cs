namespace Application.Faculties;

public record class GetAllFacultiesQuery : IRequest<OneOf<PaginatedList<FacultyResponse>, ErrorType>>
{
    public PaginationQuery Pagination { get; }
    public GetAllFacultiesQuery(int pageNumber, int pageSize)
            => Pagination = new(pageNumber, pageSize);
}

public sealed class GetAllFacultyQueryValidator : AbstractValidator<GetAllFacultiesQuery>
{
    public GetAllFacultyQueryValidator()
    {
        RuleFor(x => x.Pagination.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");

        RuleFor(x => x.Pagination.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize at least greater than or equal to 1.");
    }
}

public class GetAllFacultiesHandler : IRequestHandler<GetAllFacultiesQuery, OneOf<PaginatedList<FacultyResponse>, ErrorType>>
{
    private readonly IApplicationDbContext context;
    private readonly IMapper mapper;
    public GetAllFacultiesHandler(IApplicationDbContext context, IMapper mapper)
                    => (this.context, this.mapper) = (context, mapper);

    public async Task<OneOf<PaginatedList<FacultyResponse>, ErrorType>> Handle(GetAllFacultiesQuery request, CancellationToken cancellationToken)
                 => await context.Faculties
                            .ProjectTo<FacultyResponse>(mapper.ConfigurationProvider)
                            .PaginatedListAsync(request.Pagination.PageNumber, request.Pagination.PageSize, cancellationToken);
}
