using AlumniBackendServices.GraphQL;
using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Voyager;

namespace AlumniBackendServices.ExtensionService;

public static class GraphQLExtension
{
    public static void AddApplicationGraphQL(this IServiceCollection services) => services.AddGraphQLServer()
                .AddDefaultTransactionScopeHandler()
                .AddQueryType<QueryType>()
                .AddMutationType<MutationType>()
                .BindRuntimeType<Guid, IdType>()
                .BindRuntimeType<string, StringType>();

    public static void UseApplicationGraphQL(this IApplicationBuilder app)
        => app.UseEndpoints(endpoints => endpoints.MapGraphQL("/graphql"))
        //.UsePlayground("/graphql", "/ui/playground")
        .UseVoyager("/graphql", "/ui/voyager");
}
