using Alumni.Api.GraphQL;

namespace Alumni.Api.ExtensionService;

public static class GraphQLExtension
{
    public static IServiceCollection AddApplicationGraphQL(this IServiceCollection services)
    {
        services.AddGraphQLServer()
            .AddQueryType(descriptor => descriptor.Name("Query"))
            .AddMutationType(descriptor => descriptor.Name("Mutation"))
            .AddTypeExtension<StudentQuery>()
            .AddTypeExtension<FacultyQuery>()
            .AddTypeExtension<CompanyQuery>()
            .AddTypeExtension<ExamQuery>()
            .AddTypeExtension<FurtherStudyQuery>()
            .AddTypeExtension<StudentMutation>()
            .AddTypeExtension<FacultyMutation>()
            .AddTypeExtension<CompanyMutation>()
            .AddTypeExtension<ExamMutation>()
            .AddTypeExtension<FurtherStudyMutation>()
            .BindRuntimeType<Guid, UuidType>()
            .BindRuntimeType<DateOnly, LocalDateType>()
            .BindRuntimeType<long, LongType>()
            .ModifyOptions(options =>
            {
                // Queries may run in parallel; failed mutations must not share tracked changes.
                options.DefaultQueryDependencyInjectionScope = DependencyInjectionScope.Resolver;
                options.DefaultMutationDependencyInjectionScope = DependencyInjectionScope.Resolver;
            })
            .ModifyRequestOptions(options => options.IncludeExceptionDetails = false);

        return services;
    }

    public static IEndpointConventionBuilder MapApplicationGraphQL(this IEndpointRouteBuilder app)
        => app.MapGraphQL("/graphql");
}
