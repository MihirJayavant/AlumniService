using System;
using AlumniBackendServices.GraphQL;
using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Voyager;
using HotChocolate.Execution.Configuration;
using HotChocolate.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AlumniBackendServices.ExtensionService
{
    public static class GraphQLExtension
    {
        public static void AddApplicationGraphQL(this IServiceCollection services)
        {

            var schema = SchemaBuilder.New()
                                    .AddQueryType<QueryType>()
                                    .AddMutationType<MutationType>()
                                    .BindClrType<Guid, IdType>()
                                    .BindClrType<string, StringType>()
                                    .Create();

            services.AddGraphQL(schema, new QueryExecutionOptions { ForceSerialExecution = true });

        }

        public static void UseApplictionGraphQL(this IApplicationBuilder app)
                    => app.UseGraphQL("/graphql")
                            .UsePlayground("/graphql", "/ui/playground")
                            .UseVoyager("/graphql", "/ui/voyager");
    }
}
