using Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Generators.UnitTests;

public class RecordViewGeneratorTests
{
    [Theory]
    [InlineData("public", Accessibility.Public)]
    [InlineData("internal", Accessibility.Internal)]
    [InlineData("", Accessibility.Internal)]
    public void Generate_WhenTargetHasAccessibility_PreservesAccessibility(string modifier, Accessibility expected)
    {
        var (_, compilation) = Generate($$"""
            public record Member(int Id);
            [Core.RecordView(typeof(Member))]
            {{modifier}} partial record MemberView;
            """);

        var view = GetView(compilation, "MemberView");
        Assert.Equal(expected, view.DeclaredAccessibility);
        Assert.Equal("Id", Assert.Single(GetProperties(view)).Name);
    }

    [Fact]
    public void Generate_WhenAccessibilityIsOnAnotherPartialDeclaration_PreservesAccessibility()
    {
        var (_, compilation) = Generate("""
            public record Member(int Id);
            [Core.RecordView(typeof(Member))]
            partial record MemberView;
            public partial record MemberView;
            """);

        Assert.Equal(Accessibility.Public, GetView(compilation, "MemberView").DeclaredAccessibility);
    }

    [Fact]
    public void Generate_WhenSourceHasPublicProperties_EmitsRequiredInitProperties()
    {
        var (result, compilation) = Generate("""
            namespace Models;
            public record Member(int Id, string Name);
            [Core.RecordView(typeof(Member))]
            public partial record MemberView;
            """);

        Assert.Single(result.GeneratedTrees);
        var view = GetView(compilation, "Models.MemberView");
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
    [InlineData("nameof(Member.Id)", new[] { "Name" })]
    [InlineData("nameof(Member.Id), nameof(Member.Name)", new string[0])]
    [InlineData("\"Missing\"", new[] { "Id", "Name" })]
    public void Generate_WhenPropertiesAreExcluded_OmitsMatchingProperties(string exclusions, string[] expected)
    {
        var (_, compilation) = Generate($$"""
            public record Member(int Id, string Name);
            [Core.RecordView(typeof(Member), {{exclusions}})]
            public partial record MemberView;
            """);

        Assert.Equal(expected, GetProperties(GetView(compilation, "MemberView")).Select(p => p.Name));
    }

    [Fact]
    public void Generate_WhenSourceHasOtherMembers_OnlyCopiesPublicProperties()
    {
        var (_, compilation) = Generate("""
            public record Member
            {
                public int Id { get; init; }
                private int PrivateValue { get; init; }
                internal int InternalValue { get; init; }
                protected int ProtectedValue { get; init; }
                public int Field;
                public int GetValue() => 1;
            }
            [Core.RecordView(typeof(Member))]
            public partial record MemberView;
            """);

        Assert.Equal("Id", Assert.Single(GetProperties(GetView(compilation, "MemberView"))).Name);
    }

    [Fact]
    public void Generate_WhenTypesAreNullableOrGeneric_PreservesPropertyTypes()
    {
        var (_, compilation) = Generate("""
            #nullable enable
            namespace Models;
            public record Address(string City);
            public record Member(string? Name, int? Age, System.Collections.Generic.List<Address?> Addresses);
            [Core.RecordView(typeof(Member))]
            public partial record MemberView;
            """);

        var sourceProperties = GetProperties(GetView(compilation, "Models.Member"));
        var viewProperties = GetProperties(GetView(compilation, "Models.MemberView"));
        Assert.Equal(sourceProperties.Length, viewProperties.Length);
        foreach (var source in sourceProperties)
        {
            var generated = Assert.Single(viewProperties, p => p.Name == source.Name);
            Assert.True(SymbolEqualityComparer.IncludeNullability.Equals(source.Type, generated.Type),
                $"Type of {source.Name} was not preserved: {source.Type} versus {generated.Type}.");
        }
    }

    [Theory]
    [InlineData("", "MemberView")]
    [InlineData("namespace Models;", "Models.MemberView")]
    [InlineData("namespace School.Models;", "School.Models.MemberView")]
    public void Generate_WhenTargetHasNamespace_UsesTargetNamespace(string declaration, string metadataName)
    {
        var (_, compilation) = Generate($$"""
            {{declaration}}
            public record Member(int Id);
            [Core.RecordView(typeof(Member))]
            public partial record MemberView;
            """);

        Assert.Equal("Id", Assert.Single(GetProperties(GetView(compilation, metadataName))).Name);
    }

    [Fact]
    public void Generate_WhenAttributeIsAbsent_ProducesNoOutput()
    {
        var (result, _) = Generate("public record Member(int Id);");

        Assert.Empty(result.GeneratedTrees);
    }

    [Fact]
    public void Generate_WhenMultipleTargetsExist_GeneratesEachViewIndependently()
    {
        var (result, compilation) = Generate("""
            public record Member(int Id, string Name);
            [Core.RecordView(typeof(Member))]
            public partial record MemberView;
            [Core.RecordView(typeof(Member), nameof(Member.Id))]
            public partial record MemberCommand;
            """);

        Assert.Equal(2, result.GeneratedTrees.Length);
        Assert.Equal(["Id", "Name"], GetProperties(GetView(compilation, "MemberView")).Select(p => p.Name));
        Assert.Equal("Name", Assert.Single(GetProperties(GetView(compilation, "MemberCommand"))).Name);
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
