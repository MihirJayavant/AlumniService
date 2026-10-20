using Grpc.Core;

namespace Alumni.Api.Grpc;

public sealed class CompanyGrpc(
    GetCompanyHandler getCompanyHandler,
    AddCompanyHandler addCompanyHandler) : Contracts.CompanyService.CompanyServiceBase
{
    public override async Task<Contracts.CompanyListReply> ListByMember(Contracts.MemberIdRequest request, ServerCallContext context)
    {
        var query = new GetCompany { MemberId = GrpcInput.Guid(request.MemberId, "member_id") };
        var result = await getCompanyHandler.Execute(query, context.CancellationToken);
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
            MemberId = GrpcInput.Guid(request.MemberId, "member_id"),
            CompanyId = GrpcInput.OptionalGuid(request.CompanyId, "company_id"),
            CompanyName = request.CompanyName,
            Designation = request.Designation,
            YearOfJoining = request.YearOfJoining,
            AnnualSalary = request.AnnualSalary,
        };
        var result = await addCompanyHandler.Execute(command, context.CancellationToken);
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
