using Alumni.Api.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Alumni.Api.UnitTests.Migrations;

public sealed class NetworkMigrationTests
{
    [Fact]
    public void Up_WhenRenamingNetwork_PreservesTablesAndExistingRoleAssignments()
    {
        var operations = new RenameStudentToNetworkMembers().UpOperations;

        Assert.DoesNotContain(operations, operation => operation is DropTableOperation or CreateTableOperation);
        var tables = operations.OfType<RenameTableOperation>().ToArray();
        Assert.Equal(4, tables.Length);
        Assert.All(tables, table => Assert.Equal("Network", table.NewSchema));
        Assert.Contains(tables, table => table.Name == "students" && table.NewName == "members");
        var identifiers = operations.OfType<RenameColumnOperation>().ToArray();
        Assert.Equal(4, identifiers.Length);
        Assert.All(identifiers, column =>
        {
            Assert.Equal("StudentId", column.Name);
            Assert.Equal("MemberId", column.NewName);
        });
        Assert.DoesNotContain(operations, operation => operation is DeleteDataOperation or InsertDataOperation);
        var role = Assert.Single(operations.OfType<UpdateDataOperation>());
        Assert.Equal("Student", role.KeyValues[0, 0]);
        Assert.Equal("Member", role.Values[0, Array.IndexOf(role.Columns, "Name")]);
    }

    [Fact]
    public void Down_WhenRevertingNetwork_PreservesTablesAndRestoresIdentifiers()
    {
        var operations = new RenameStudentToNetworkMembers().DownOperations;

        Assert.DoesNotContain(operations, operation => operation is DropTableOperation or CreateTableOperation);
        var tables = operations.OfType<RenameTableOperation>().ToArray();
        Assert.Equal(4, tables.Length);
        Assert.All(tables, table => Assert.Equal("Student", table.NewSchema));
        Assert.Contains(tables, table => table.Name == "members" && table.NewName == "students");
        Assert.All(operations.OfType<RenameColumnOperation>(), column =>
        {
            Assert.Equal("MemberId", column.Name);
            Assert.Equal("StudentId", column.NewName);
        });
    }
}
