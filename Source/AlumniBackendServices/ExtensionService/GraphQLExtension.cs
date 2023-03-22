using System;
using AlumniBackendServices.GraphQL;
using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Voyager;
using HotChocolate.Execution.Configuration;
using HotChocolate.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AlumniBackendServices.ExtensionService;

public static class GraphQLExtension
{
    public static void AddApplicationGraphQL(this IServiceCollection services)
    {
        services.AddGraphQLServer()
                .AddDefaultTransactionScopeHandler()
                .AddQueryType<QueryType>()
                .AddMutationType<MutationType>()
                .BindClrType<Guid, IdType>()
                .BindClrType<string, StringType>();
    }

    public static void UseApplicationGraphQL(this IApplicationBuilder app)
        => app.UseEndpoints(endpoints =>
        {
            endpoints.MapGraphQL("/graphql");
        })
        .UsePlayground("/graphql", "/ui/playground")
        .UseVoyager("/graphql", "/ui/voyager");
}
