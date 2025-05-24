using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Generators
{

    [Generator(LanguageNames.CSharp)]
    public sealed class RecordViewGenerator : IIncrementalGenerator
    {
        private const string RecordViewAttributeName = "Domain.RecordView";

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
            if (context.TargetSymbol is not INamedTypeSymbol targetSymbol) return null;
            if (context.Attributes.Length == 0) return null;

            var attribute = context.Attributes[0];
            token.ThrowIfCancellationRequested();

            if (attribute.ConstructorArguments[0].Value is not INamedTypeSymbol sourceType) return null;

            var excluded = new HashSet<string>(
                attribute.ConstructorArguments.ElementAtOrDefault(1).Values
                    .Select(v => v.Value?.ToString())
                    .Where(n => n != null)!);

            var members = new List<RecordMember>();
            foreach (var m in sourceType.GetMembers())
            {
                token.ThrowIfCancellationRequested();
                if (excluded.Contains(m.Name)) continue;

                if (m is IPropertySymbol { DeclaredAccessibility: Accessibility.Public } p)
                {
                    members.Add(
                        new RecordMember(
                            p.Name,
                            p.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat
                            )));
                }
            }

            var ns = targetSymbol.ContainingNamespace.IsGlobalNamespace
                ? null
                : targetSymbol.ContainingNamespace.ToDisplayString();

            return new RecordTargetModel(
                targetSymbol.Name,
                ns,
                sourceType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                [..members]);
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
#if DEBUG
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
#endif
            if (model is null) return "";
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine();

            if (string.IsNullOrWhiteSpace(model.Namespace) is false)
            {
                sb.AppendLine($"namespace {model.Namespace};");
            }

            const string keyword = "record";

            sb.AppendLine($"public partial {keyword} {model.Name}");
            sb.AppendLine("{");

            foreach (var m in model.Members)
            {
                sb.AppendLine($"    public required {m.TypeName} {m.Name} {{ get; init; }}");
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }

    internal class RecordMember
    {
        public string Name { get; }
        public string TypeName { get; }
        public RecordMember(string name, string typeName)
        {
            Name = name;
            TypeName = typeName;
        }
    }

    internal class RecordTargetModel
    {
        public string Name { get; }
        public string Namespace { get; }
        public string SourceType { get; }
        public IReadOnlyList<RecordMember> Members { get; }

        public RecordTargetModel(string name, string ns, string sourceType, IReadOnlyList<RecordMember> members)
        {
            Name = name;
            Namespace = ns;
            SourceType = sourceType;
            Members = members;
        }
    }

}
