using Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Generators.UnitTests;

public class RecordViewGeneratorTests
{
    [Fact]
    public void Generate_WhenSourceHasPublicProperties_EmitsRequiredInitProperties()
    {
        var (result, compilation) = Generate("""
            namespace Models;
            public record Student(int Id, string Name);
            [Core.RecordView(typeof(Student))]
            public partial record StudentView;
            """);

        Assert.Single(result.GeneratedTrees);
        var view = GetView(compilation, "Models.StudentView");
        Assert.Equal(["Id", "Name"], GetProperties(view).Select(p => p.Name));
        Assert.All(GetProperties(view), p =>
        {
            Assert.True(p.IsRequired);
            Assert.NotNull(p.SetMethod);
            Assert.True(p.SetMethod.IsInitOnly);
            Assert.Equal(Accessibility.Public, p.DeclaredAccessibility);
        });
    }

    [Theory]
    [InlineData("nameof(Student.Id)", new[] { "Name" })]
    [InlineData("nameof(Student.Id), nameof(Student.Name)", new string[0])]
    [InlineData("\"Missing\"", new[] { "Id", "Name" })]
    public void Generate_WhenPropertiesAreExcluded_OmitsMatchingProperties(string exclusions, string[] expected)
    {
        var (_, compilation) = Generate($$"""
            public record Student(int Id, string Name);
            [Core.RecordView(typeof(Student), {{exclusions}})]
            public partial record StudentView;
            """);

        Assert.Equal(expected, GetProperties(GetView(compilation, "StudentView")).Select(p => p.Name));
    }

    [Fact]
    public void Generate_WhenSourceHasOtherMembers_OnlyCopiesPublicProperties()
    {
        var (_, compilation) = Generate("""
            public record Student
            {
                public int Id { get; init; }
                private int PrivateValue { get; init; }
                internal int InternalValue { get; init; }
                protected int ProtectedValue { get; init; }
                public int Field;
                public int GetValue() => 1;
            }
            [Core.RecordView(typeof(Student))]
            public partial record StudentView;
            """);

        Assert.Equal("Id", Assert.Single(GetProperties(GetView(compilation, "StudentView"))).Name);
    }

    [Fact]
    public void Generate_WhenTypesAreNullableOrGeneric_PreservesPropertyTypes()
    {
        var (_, compilation) = Generate("""
            #nullable enable
            namespace Models;
            public record Address(string City);
            public record Student(string? Name, int? Age, System.Collections.Generic.List<Address?> Addresses);
            [Core.RecordView(typeof(Student))]
            public partial record StudentView;
            """);

        var sourceProperties = GetProperties(GetView(compilation, "Models.Student"));
        var viewProperties = GetProperties(GetView(compilation, "Models.StudentView"));
        Assert.Equal(sourceProperties.Length, viewProperties.Length);
        foreach (var source in sourceProperties)
        {
            var generated = Assert.Single(viewProperties, p => p.Name == source.Name);
            Assert.True(SymbolEqualityComparer.IncludeNullability.Equals(source.Type, generated.Type),
                $"Type of {source.Name} was not preserved: {source.Type} versus {generated.Type}.");
        }
    }

    [Theory]
    [InlineData("", "StudentView")]
    [InlineData("namespace Models;", "Models.StudentView")]
    [InlineData("namespace School.Models;", "School.Models.StudentView")]
    public void Generate_WhenTargetHasNamespace_UsesTargetNamespace(string declaration, string metadataName)
    {
        var (_, compilation) = Generate($$"""
            {{declaration}}
            public record Student(int Id);
            [Core.RecordView(typeof(Student))]
            public partial record StudentView;
            """);

        Assert.Equal("Id", Assert.Single(GetProperties(GetView(compilation, metadataName))).Name);
    }

    [Fact]
    public void Generate_WhenAttributeIsAbsent_ProducesNoOutput()
    {
        var (result, _) = Generate("public record Student(int Id);");

        Assert.Empty(result.GeneratedTrees);
    }

    [Fact]
    public void Generate_WhenMultipleTargetsExist_GeneratesEachViewIndependently()
    {
        var (result, compilation) = Generate("""
            public record Student(int Id, string Name);
            [Core.RecordView(typeof(Student))]
            public partial record StudentView;
            [Core.RecordView(typeof(Student), nameof(Student.Id))]
            public partial record StudentCommand;
            """);

        Assert.Equal(2, result.GeneratedTrees.Length);
        Assert.Equal(["Id", "Name"], GetProperties(GetView(compilation, "StudentView")).Select(p => p.Name));
        Assert.Equal("Name", Assert.Single(GetProperties(GetView(compilation, "StudentCommand"))).Name);
    }

    private static (GeneratorDriverRunResult Result, Compilation Compilation) Generate(string source)
    {
        // Use the test process's framework assemblies and the real attribute contract.
        var assemblyPaths = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(Path.PathSeparator)
            .Append(typeof(RecordViewAttribute).Assembly.Location)
            .Distinct(StringComparer.Ordinal);
        var references = assemblyPaths.Select(path => MetadataReference.CreateFromFile(path));
        var input = CSharpCompilation.Create("GeneratorTests",
            [CSharpSyntaxTree.ParseText(source)], references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                nullableContextOptions: NullableContextOptions.Enable));
        GeneratorDriver driver = CSharpGeneratorDriver.Create(new RecordViewGenerator());
        driver = driver.RunGeneratorsAndUpdateCompilation(input, out var output, out var diagnostics);
        var result = driver.GetRunResult();

        Assert.Empty(diagnostics);
        Assert.Null(Assert.Single(result.Results).Exception);
        Assert.Empty(output.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error));
        return (result, output);
    }

    private static INamedTypeSymbol GetView(Compilation compilation, string metadataName)
        => Assert.IsAssignableFrom<INamedTypeSymbol>(compilation.GetTypeByMetadataName(metadataName));

    private static IPropertySymbol[] GetProperties(INamedTypeSymbol type)
        => type.GetMembers().OfType<IPropertySymbol>()
            .Where(p => p.DeclaredAccessibility == Accessibility.Public).ToArray();
}
