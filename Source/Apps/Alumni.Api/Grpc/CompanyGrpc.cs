using Grpc.Core;

namespace Alumni.Api.Grpc;

public sealed class CompanyGrpc(IStudentDbContext database) : Contracts.CompanyService.CompanyServiceBase
{
    public override async Task<Contracts.CompanyListReply> ListByStudent(Contracts.StudentIdRequest request, ServerCallContext context)
    {
        var query = new GetCompany { StudentId = GrpcInput.Guid(request.StudentId, "student_id") };
        var result = await new GetCompanyHandler(database).Execute(query, context.CancellationToken);
        return GrpcResult.Map(result, page =>
        {
            var reply = new Contracts.CompanyListReply { Pagination = GrpcMapping.Pagination(page) };
            reply.Items.AddRange(page.Items.Select(Map));
            return reply;
        });
    }

    public override async Task<Contracts.CompanyReply> Add(Contracts.AddCompanyRequest request, ServerCallContext context)
    {
        var command = new AddCompany
        {
            StudentId = GrpcInput.Guid(request.StudentId, "student_id"),
            CompanyId = GrpcInput.OptionalGuid(request.CompanyId, "company_id"),
            CompanyName = request.CompanyName,
            Designation = request.Designation,
            YearOfJoining = request.YearOfJoining,
            AnnualSalary = request.AnnualSalary,
        };
        var result = await new AddCompanyHandler(database).Execute(command, context.CancellationToken);
        return GrpcResult.Map(result, Map);
    }

    private static Contracts.CompanyReply Map(CompanyResponse company) => new()
    {
        CompanyId = company.CompanyId.ToString(),
        CompanyName = company.CompanyName,
        Designation = company.Designation,
        YearOfJoining = company.YearOfJoining,
        AnnualSalary = company.AnnualSalary,
    };
}
