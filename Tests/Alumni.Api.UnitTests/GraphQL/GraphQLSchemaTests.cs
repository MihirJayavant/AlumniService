using HotChocolate.Types;
using Xunit;

namespace Alumni.Api.UnitTests.GraphQL;

public class GraphQLSchemaTests
{
    private static readonly string[] QueryOperations =
        ["companies", "exams", "faculties", "faculty", "furtherStudies", "student", "students"];
    private static readonly string[] MutationOperations =
        ["addCompany", "addExam", "addFaculty", "addFurtherStudy", "addStudent", "deleteFaculty"];

    [Fact]
    public async Task Schema_WhenBuilt_ExposesDomainScalarAndInputContracts()
    {
        await using var context = new GraphQLTestContext();
        var schema = (await context.GetExecutorAsync()).Schema;

        var student = schema.Types.GetType<ObjectType>("StudentResponse");
        Assert.Equal("UUID", student.Fields["studentId"].Type.NamedType().Name);
        Assert.Equal("LocalDate", student.Fields["dateOfBirth"].Type.NamedType().Name);
        Assert.Equal("String", student.Fields["mobileNo"].Type.NamedType().Name);
        var company = schema.Types.GetType<ObjectType>("CompanyResponse");
        Assert.Equal("Long", company.Fields["annualSalary"].Type.NamedType().Name);
        var faculty = schema.Types.GetType<ObjectType>("FacultyResponse");
        Assert.Equal("Long", faculty.Fields["mobileNo"].Type.NamedType().Name);
        Assert.Equal("DateTime", faculty.Fields["createdAt"].Type.NamedType().Name);
        Assert.IsType<NonNullType>(faculty.Fields["createdAt"].Type);
        Assert.Equal("DateTime", faculty.Fields["updatedAt"].Type.NamedType().Name);
        Assert.IsNotType<NonNullType>(faculty.Fields["updatedAt"].Type);

        var input = schema.Types.GetType<InputObjectType>("AddFacultyInput");
        Assert.Equal("Long", input.Fields["mobileNo"].Type.NamedType().Name);
        Assert.Equal("AddFacultyInput", schema.MutationType!.Fields["addFaculty"].Arguments["input"].Type.NamedType().Name);
        var studentInput = schema.Types.GetType<InputObjectType>("AddStudentInput");
        Assert.Equal("LocalDate", studentInput.Fields["dateOfBirth"].Type.NamedType().Name);
        Assert.Equal("AddressInput", studentInput.Fields["currentAddress"].Type.NamedType().Name);
        Assert.Empty(context.Accesses);
    }

    [Fact]
    public async Task Schema_WhenBuilt_ExposesAllControllerOperationsWithNullableResults()
    {
        await using var context = new GraphQLTestContext();
        var executor = await context.GetExecutorAsync();

        var query = executor.Schema.QueryType;
        Assert.Equal(
            QueryOperations,
            query.Fields.Where(field => !field.Name.StartsWith("__", StringComparison.Ordinal))
                .Select(field => field.Name).OrderBy(name => name));
        var mutation = Assert.IsType<ObjectType>(executor.Schema.MutationType);
        Assert.Equal(
            MutationOperations,
            mutation.Fields.Where(field => !field.Name.StartsWith("__", StringComparison.Ordinal))
                .Select(field => field.Name).OrderBy(name => name));

        Assert.All(query.Fields.Where(field => !field.Name.StartsWith("__", StringComparison.Ordinal)),
            field => Assert.IsNotType<NonNullType>(field.Type));
        Assert.All(mutation.Fields.Where(field => !field.Name.StartsWith("__", StringComparison.Ordinal)),
            field => Assert.IsNotType<NonNullType>(field.Type));
        Assert.Empty(context.Accesses);
    }

    [Theory]
    [InlineData("students")]
    [InlineData("faculties")]
    public async Task Execute_WhenListPaginationIsOmitted_RejectsDocumentWithoutDatabaseAccess(string field)
    {
        await using var context = new GraphQLTestContext();

        var result = await context.ExecuteAsync($"{{ {field} {{ totalCount }} }}");

        Assert.NotEmpty(result.GetProperty("errors").EnumerateArray());
        Assert.Empty(context.Accesses);
    }

    [Theory]
    [InlineData("{ student(id: \"not-a-guid\") { studentId } }")]
    [InlineData("{ faculty(facultyId: \"not-a-guid\") { facultyId } }")]
    public async Task Execute_WhenGuidCannotBeCoerced_RejectsDocumentWithoutDatabaseAccess(string document)
    {
        await using var context = new GraphQLTestContext();

        var result = await context.ExecuteAsync(document);

        Assert.NotEmpty(result.GetProperty("errors").EnumerateArray());
        Assert.Empty(context.Accesses);
    }

    [Fact]
    public async Task Execute_WhenOneFieldFails_PreservesSiblingDataAndErrorPath()
    {
        await using var context = new GraphQLTestContext();

        var result = await context.ExecuteAsync(
            "{ __typename invalid: student(id: \"" + GraphQLTestContext.EmptyId + "\") { studentId } }");

        Assert.Equal("Query", result.GetProperty("data").GetProperty("__typename").GetString());
        Assert.Equal(System.Text.Json.JsonValueKind.Null, result.GetProperty("data").GetProperty("invalid").ValueKind);
        var error = Assert.Single(result.GetProperty("errors").EnumerateArray());
        Assert.Equal("BAD_REQUEST", error.GetProperty("extensions").GetProperty("code").GetString());
        Assert.Equal("invalid", Assert.Single(error.GetProperty("path").EnumerateArray()).GetString());
        Assert.Empty(context.Accesses);
    }

    [Fact]
    public async Task Execute_WhenMultipleQueryFieldsRun_UsesDifferentContextScopesAndHidesProviderDetails()
    {
        await using var context = new GraphQLTestContext();

        var result = await context.ExecuteAsync(
            "{ student(id: \"" + GraphQLTestContext.ValidId + "\") { studentId } " +
            "faculty(facultyId: \"" + GraphQLTestContext.ValidId + "\") { facultyId } }");

        Assert.Equal(2, context.Accesses.Count);
        Assert.Equal(2, context.Accesses.Distinct().Count());
        Assert.Equal(2, result.GetProperty("errors").GetArrayLength());
        Assert.All(result.GetProperty("errors").EnumerateArray(), error =>
        {
            Assert.Equal("INTERNAL_ERROR", error.GetProperty("extensions").GetProperty("code").GetString());
            Assert.DoesNotContain("Private provider", error.GetProperty("message").GetString());
        });
    }

    [Fact]
    public async Task Execute_WhenMutationFieldsRunSequentially_UsesDifferentContextScopesAfterFailure()
    {
        await using var context = new GraphQLTestContext();

        var result = await context.ExecuteAsync(
            "mutation { first: deleteFaculty(facultyId: \"" + GraphQLTestContext.ValidId + "\") { facultyId } " +
            "second: deleteFaculty(facultyId: \"" + GraphQLTestContext.ValidId + "\") { facultyId } }");

        Assert.Equal(2, context.Accesses.Count);
        Assert.Equal(2, context.Accesses.Distinct().Count());
        Assert.Equal(2, result.GetProperty("errors").GetArrayLength());
    }
}
