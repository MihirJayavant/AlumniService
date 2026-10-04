using Alumni.Api.Grpc;

namespace Alumni.Api.ExtensionService;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1708:Identifiers should differ by more than case",
    Justification = "False positive for multiple C# extension blocks: https://github.com/dotnet/sdk/issues/51716")]
public static class GrpcExtension
{
    extension(IServiceCollection services)
    {
        public void AddApplicationGrpc()
            => services.AddGrpc();
    }

    extension(IEndpointRouteBuilder app)
    {
        public void MapApplicationGrpc()
        {
            app.MapGrpcService<StudentGrpc>();
            app.MapGrpcService<FacultyGrpc>();
            app.MapGrpcService<CompanyGrpc>();
            app.MapGrpcService<ExamGrpc>();
            app.MapGrpcService<FurtherStudyGrpc>();
        }
    }
}
