namespace Alumni.Api.UnitTests;

internal static class HandlerTestRegistration
{
    internal static Type[] AllHandlers { get; } =
    [
        typeof(AddStudentHandler), typeof(GetStudentHandler), typeof(GetAllStudentHandler),
        typeof(AddStaffMemberHandler), typeof(GetStaffMemberHandler), typeof(GetStaffMembersHandler), typeof(DeleteStaffMemberHandler),
        typeof(AddCompanyHandler), typeof(GetCompanyHandler), typeof(AddExamHandler), typeof(GetExamHandler),
        typeof(AddFurtherStudyHandler), typeof(GetFurtherStudyHandler), typeof(BootstrapAdminHandler)
    ];

    internal static void AddDomainHandlers(IServiceCollection services)
    {
        foreach (var handler in AllHandlers.Where(handler => handler != typeof(BootstrapAdminHandler)))
        {
            services.AddScoped(handler);
        }
    }

    internal static void AddUnreachableHandlers(IServiceCollection services)
    {
        foreach (var handler in AllHandlers)
        {
            services.AddScoped(handler, _ =>
                throw new InvalidOperationException("Handler must not be resolved during route inspection."));
        }
    }
}
