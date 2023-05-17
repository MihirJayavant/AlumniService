namespace Application.Students;

public sealed class GetAllStudentQuery : IRequest<OneOf<AllStudentResponse, ErrorType>>
{
    public PaginationQuery Pagination { get; }

    public GetAllStudentQuery(int pageNumber, int pageSize)
            => Pagination = new(pageNumber, pageSize);
}

public sealed class GetAllStudentQueryValidator : AbstractValidator<GetAllStudentQuery>
{
    public GetAllStudentQueryValidator()
    {
        RuleFor(x => x.Pagination.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");

        RuleFor(x => x.Pagination.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize at least greater than or equal to 1.");
    }
}

public sealed class GetAllStudentHandler
            : IRequestHandler<GetAllStudentQuery, OneOf<AllStudentResponse, ErrorType>>
{

    private readonly IApplicationDbContext context;
    private readonly IMapper mapper;

    public GetAllStudentHandler(IApplicationDbContext context, IMapper mapper)
                => (this.context, this.mapper) = (context, mapper);

    public async Task<OneOf<AllStudentResponse, ErrorType>> Handle(GetAllStudentQuery request, CancellationToken cancellationToken)
    {
        var data = await context.Students
                    .ProjectTo<StudentResponse>(mapper.ConfigurationProvider)
                    .PaginatedListAsync(request.Pagination.PageNumber, request.Pagination.PageSize, cancellationToken);
        return new AllStudentResponse(data);
    }
}
