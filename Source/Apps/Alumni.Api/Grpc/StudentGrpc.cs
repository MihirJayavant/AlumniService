using System.Globalization;
using Grpc.Core;

namespace Alumni.Api.Grpc;

public sealed class StudentGrpc(IStudentDbContext dbContext) : Contracts.StudentService.StudentServiceBase
{
    public override async Task<Contracts.StudentListReply> List(Contracts.PaginationRequest request, ServerCallContext context)
    {
        var query = new GetAllStudent
        {
            PageNumber = request.HasPageNumber ? request.PageNumber : 1,
            PageSize = request.HasPageSize ? request.PageSize : 10
        };
        var result = await new GetAllStudentHandler(dbContext).Execute(query, context.CancellationToken);
        return GrpcResult.Map(result, page =>
        {
            var reply = new Contracts.StudentListReply { Pagination = GrpcMapping.Pagination(page) };
            reply.Items.AddRange(page.Items.Select(ToReply));
            return reply;
        });
    }

    public override async Task<Contracts.StudentReply> Get(Contracts.StudentIdRequest request, ServerCallContext context)
    {
        var query = new GetStudent { Id = GrpcInput.Guid(request.StudentId, "student_id") };
        var result = await new GetStudentHandler(dbContext).Execute(query, context.CancellationToken);
        return GrpcResult.Map(result, ToReply);
    }

    public override async Task<Contracts.StudentReply> Add(Contracts.AddStudentRequest request, ServerCallContext context)
    {
        var command = new AddStudent
        {
            StudentId = GrpcInput.OptionalGuid(request.StudentId, "student_id"),
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
        var result = await new AddStudentHandler(dbContext).Execute(command, context.CancellationToken);
        return GrpcResult.Map(result, ToReply);
    }

    private static Contracts.StudentReply ToReply(StudentResponse student)
        => new()
        {
            StudentId = student.StudentId.ToString("D"),
            FirstName = student.FirstName,
            LastName = student.LastName,
            MobileNo = student.MobileNo,
            Extension = student.Extension,
            Gender = student.Gender,
            DateOfBirth = student.DateOfBirth.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            Email = student.Email,
            Branch = student.Branch,
            CurrentAddress = GrpcMapping.Address(student.CurrentAddress),
            CorrespondenceAddress = GrpcMapping.Address(student.CorrespondenceAddress),
            AdmissionYear = student.AdmissionYear,
            PassingYear = student.PassingYear
        };
}
