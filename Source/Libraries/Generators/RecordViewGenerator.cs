using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Generators
{

    [Generator(LanguageNames.CSharp)]
    public sealed class RecordViewGenerator : IIncrementalGenerator
    {
        private const string RecordViewAttributeName = "Core.RecordViewAttribute";

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var facets = context.SyntaxProvider.ForAttributeWithMetadataName(
                    RecordViewAttributeName,
                    predicate: static (node, _) => node is TypeDeclarationSyntax,
                    transform: static (ctx, token) => GetTargetModel(ctx, token))
                .Where(static m => m is not null);

            context.RegisterSourceOutput(facets, static (spc, model) =>
            {
                spc.CancellationToken.ThrowIfCancellationRequested();
                var code = Generate(model);
                spc.AddSource($"{model!.Name}.g.cs", SourceText.From(code, Encoding.UTF8));
            });
        }

        private static RecordTargetModel? GetTargetModel(GeneratorAttributeSyntaxContext context,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            if (context.TargetSymbol is not INamedTypeSymbol targetSymbol)
            {
                return null;
            }

            if (context.Attributes.Length == 0)
            {
                return null;
            }

            var attribute = context.Attributes[0];
            token.ThrowIfCancellationRequested();

            if (attribute.ConstructorArguments[0].Value is not INamedTypeSymbol sourceType)
            {
                return null;
            }

            var excluded = new HashSet<string>(
                attribute.ConstructorArguments.ElementAtOrDefault(1).Values
                    .Select(v => v.Value?.ToString())
                    .Where(n => n != null)!);

            var members = new List<RecordMember>();
            foreach (var m in sourceType.GetMembers())
            {
                token.ThrowIfCancellationRequested();
                if (excluded.Contains(m.Name))
                {
                    continue;
                }

                if (m is IPropertySymbol { DeclaredAccessibility: Accessibility.Public } p)
                {
                    members.Add(
                        new RecordMember
                        {
                            Name = p.Name,
                            TypeName = p.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat
                                .WithMiscellaneousOptions(SymbolDisplayFormat.FullyQualifiedFormat.MiscellaneousOptions
                                    | SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier))
                        });
                }
            }

            var ns = targetSymbol.ContainingNamespace.IsGlobalNamespace
                ? null
                : targetSymbol.ContainingNamespace.ToDisplayString();

            return new RecordTargetModel
            {
                Name = targetSymbol.Name,
                Namespace = ns ?? "",
                Accessibility = targetSymbol.DeclaredAccessibility,
                SourceType = sourceType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                Members = [.. members]
            };
        }

        private static T GetNamedArg<T>(
            ImmutableArray<KeyValuePair<string, TypedConstant>> args,
            string name,
            T defaultValue)
            => args.FirstOrDefault(kv => kv.Key == name)
                .Value.Value is T t
                ? t
                : defaultValue;

        private static string Generate(RecordTargetModel? model)
        {
            if (model is null)
            {
                return "";
            }

            var sb = new StringBuilder();
            sb.AppendLine("#nullable enable");
            sb.AppendLine("using System;");
            sb.AppendLine();

            if (string.IsNullOrWhiteSpace(model.Namespace) is false)
            {
                sb.AppendLine($"namespace {model.Namespace};");
            }

            const string keyword = "record";
            var accessibility = model.Accessibility switch
            {
                Accessibility.Public => "public",
                Accessibility.Internal => "internal",
                Accessibility.Private => "private",
                Accessibility.Protected => "protected",
                Accessibility.ProtectedAndInternal => "private protected",
                Accessibility.ProtectedOrInternal => "protected internal",
                _ => ""
            };

            sb.AppendLine($"{accessibility} partial {keyword} {model.Name}");
            sb.AppendLine("{");

            foreach (var m in model.Members)
            {
                sb.AppendLine($"    public required {m.TypeName} {m.Name} {{ get; init; }}");
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }

    internal sealed record RecordMember
    {
        public required string Name { get; init; }
        public required string TypeName { get; init; }
    }

    internal sealed record RecordTargetModel
    {
        public required string Name { get; init; }
        public required string Namespace { get; init; }
        public required Accessibility Accessibility { get; init; }
        public required string SourceType { get; init; }
        public required IReadOnlyList<RecordMember> Members { get; init; }
    }

}
