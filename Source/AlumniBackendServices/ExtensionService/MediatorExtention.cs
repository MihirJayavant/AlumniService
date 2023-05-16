using Application.Common.Models;
using Application.Exams;
using Application.Students;
using FluentValidation;
using Infrastructure.Behaviors;

namespace AlumniBackendServices.ExtensionService;

public static class MeditorExtension
{
    public static void AddMediatorServices(this IServiceCollection services)
    => services.AddValidatorsFromAssembly(typeof(StudentResponse).Assembly)
            .AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(StudentResponse).Assembly)
                    .AddValidation<AddExamCommand, ExamResponse>()
                );

    public static MediatRServiceConfiguration AddValidation<TRequest, TResponse>
    (this MediatRServiceConfiguration configuration) where TRequest : notnull
        => configuration.AddBehavior<IPipelineBehavior<TRequest, OneOf<TResponse, ErrorType>>, ValidationBehavior<TRequest, TResponse>>();
}
