using System.Globalization;
using Grpc.Core;

namespace Alumni.Api.Grpc;

public sealed class MemberGrpc(
    GetMembersHandler getAllMemberHandler,
    GetMemberHandler getMemberHandler,
    AddMemberHandler addMemberHandler) : Contracts.MemberService.MemberServiceBase
{
    public override async Task<Contracts.MemberListReply> List(Contracts.PaginationRequest request, ServerCallContext context)
    {
        var query = new GetMembers
        {
            PageNumber = request.HasPageNumber ? request.PageNumber : 1,
            PageSize = request.HasPageSize ? request.PageSize : 10
        };
        var result = await getAllMemberHandler.Execute(query, context.CancellationToken);
        return GrpcResult.Map(result, page =>
        {
            var reply = new Contracts.MemberListReply { Pagination = GrpcMapping.Pagination(page) };
            reply.Items.AddRange(page.Items.Select(ToReply));
            return reply;
        });
    }

    public override async Task<Contracts.MemberReply> Get(Contracts.MemberIdRequest request, ServerCallContext context)
    {
        var query = new GetMember { Id = GrpcInput.Guid(request.MemberId, "member_id") };
        var result = await getMemberHandler.Execute(query, context.CancellationToken);
        return GrpcResult.Map(result, ToReply);
    }

    public override async Task<Contracts.MemberReply> Add(Contracts.AddMemberRequest request, ServerCallContext context)
    {
        var command = new AddMember
        {
            MemberId = GrpcInput.OptionalGuid(request.MemberId, "member_id"),
            FirstName = request.FirstName,
            LastName = request.LastName,
            MobileNo = request.MobileNo,
            Extension = request.Extension,
            Gender = request.Gender,
            DateOfBirth = GrpcInput.Date(request.DateOfBirth, "date_of_birth"),
            Email = request.Email,
            Branch = request.Branch,
            CurrentAddress = GrpcInput.Address(request.CurrentAddress, "current_address"),
            CorrespondenceAddress = GrpcInput.Address(request.CorrespondenceAddress, "correspondence_address"),
            AdmissionYear = request.AdmissionYear,
            PassingYear = request.PassingYear
        };
        var result = await addMemberHandler.Execute(command, context.CancellationToken);
        return GrpcResult.Map(result, ToReply);
    }

    private static Contracts.MemberReply ToReply(MemberResponse member)
        => new()
        {
            MemberId = member.MemberId.ToString("D"),
            FirstName = member.FirstName,
            LastName = member.LastName,
            MobileNo = member.MobileNo,
            Extension = member.Extension,
            Gender = member.Gender,
            DateOfBirth = member.DateOfBirth.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            Email = member.Email,
            Branch = member.Branch,
            CurrentAddress = GrpcMapping.Address(member.CurrentAddress),
            CorrespondenceAddress = GrpcMapping.Address(member.CorrespondenceAddress),
            AdmissionYear = member.AdmissionYear,
            PassingYear = member.PassingYear
        };
}
