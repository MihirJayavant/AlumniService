using System.Text.Json;
using Xunit;

namespace Alumni.Api.UnitTests.GraphQL;

public class GraphQLValidationTests
{
    public static TheoryData<string, string> InvalidOperations => new()
    {
        { "students", "{ students(pageNumber: 0, pageSize: 10) { totalCount } }" },
        { "students", "{ students(pageNumber: 1, pageSize: 0) { totalCount } }" },
        { "faculties", "{ faculties(pageNumber: 0, pageSize: 10) { totalCount } }" },
        { "faculties", "{ faculties(pageNumber: 1, pageSize: 0) { totalCount } }" },
        { "student", "{ student(id: \"" + GraphQLTestContext.EmptyId + "\") { studentId } }" },
        { "faculty", "{ faculty(facultyId: \"" + GraphQLTestContext.EmptyId + "\") { facultyId } }" },
        { "companies", "{ companies(studentId: \"" + GraphQLTestContext.EmptyId + "\") { totalCount } }" },
        { "exams", "{ exams(studentId: \"" + GraphQLTestContext.EmptyId + "\") { totalCount } }" },
        { "furtherStudies", "{ furtherStudies(studentId: \"" + GraphQLTestContext.EmptyId + "\") { totalCount } }" },
        { "deleteFaculty", "mutation { deleteFaculty(facultyId: \"" + GraphQLTestContext.EmptyId + "\") { facultyId } }" },
        { "addFaculty", """
            mutation { addFaculty(input: {
                email: "invalid", firstName: "Ada", lastName: "Lovelace", extension: "91", mobileNo: 9876543210
            }) { facultyId } }
            """ },
        { "addCompany", """
            mutation { addCompany(input: {
                studentId: "00000000-0000-0000-0000-000000000000",
                companyId: "11111111-1111-1111-1111-111111111111",
                companyName: "Company", designation: "Engineer", yearOfJoining: 2020, annualSalary: 5000000000
            }) { companyId } }
            """ },
        { "addExam", """
            mutation { addExam(input: {
                studentId: "00000000-0000-0000-0000-000000000000",
                examId: "11111111-1111-1111-1111-111111111111", examName: "Exam", score: 100, year: 2020
            }) { examId } }
            """ },
        { "addFurtherStudy", """
            mutation { addFurtherStudy(input: {
                studentId: "00000000-0000-0000-0000-000000000000",
                furtherStudyId: "11111111-1111-1111-1111-111111111111",
                instituteName: "University", degree: "MSc", country: "India", city: "Mumbai",
                admissionYear: 2018, passingYear: 2020
            }) { furtherStudyId } }
            """ },
        { "addStudent", """
            mutation { addStudent(input: {
                studentId: "11111111-1111-1111-1111-111111111111",
                firstName: "Ada", lastName: "Lovelace", mobileNo: "9876543210", extension: "91",
                gender: "Female", dateOfBirth: "1990-01-02", email: "invalid", branch: "CS",
                currentAddress: { pinCode: "400001", country: "India", state: "Maharashtra", city: "Mumbai", userAddress: "Main Road" },
                correspondenceAddress: { pinCode: "400001", country: "India", state: "Maharashtra", city: "Mumbai", userAddress: "Main Road" },
                admissionYear: 2010, passingYear: 2014
            }) { studentId } }
            """ }
    };

    [Theory]
    [MemberData(nameof(InvalidOperations))]
    public async Task Execute_WhenHandlerRejectsInput_ReturnsBadRequestWithoutDatabaseAccess(string field, string document)
    {
        await using var context = new GraphQLTestContext();

        var result = await context.ExecuteAsync(document);

        var error = Assert.Single(result.GetProperty("errors").EnumerateArray());
        Assert.Equal("BAD_REQUEST", error.GetProperty("extensions").GetProperty("code").GetString());
        Assert.Equal(field, Assert.Single(error.GetProperty("path").EnumerateArray()).GetString());
        Assert.Equal(JsonValueKind.Null, result.GetProperty("data").GetProperty(field).ValueKind);
        Assert.Empty(context.Accesses);
    }
}
