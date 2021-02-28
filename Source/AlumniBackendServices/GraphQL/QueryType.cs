using HotChocolate.Types;

namespace AlumniBackendServices.GraphQL
{
    public class QueryType : ObjectType
    {
        protected override void Configure(IObjectTypeDescriptor descriptor)
                => descriptor.Include<StudentQuery>()
                            .Include<CompanyQuery>();
    }
}
