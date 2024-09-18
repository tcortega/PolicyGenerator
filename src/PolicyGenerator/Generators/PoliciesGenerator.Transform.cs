using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PolicyGenerator.Generators;

public sealed partial class PoliciesGenerator
{
    private static PolicyDescriptor? TransformPolicy(GeneratorAttributeSyntaxContext context, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var symbol = (INamedTypeSymbol)context.TargetSymbol;

        token.ThrowIfCancellationRequested();

        if (GetValidPolicyName(symbol) is not { } policyName)
            return null;

        if (GetValidClaims(context, symbol) is not { Length: > 0 } claims)
            return null;

        token.ThrowIfCancellationRequested();

        return new PolicyDescriptor
        {
            Name = policyName,
            Claims = string.Join(",", claims),
            ClassPath = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
        };
    }

    private static string? GetValidPolicyName(INamespaceOrTypeSymbol symbol)
    {
        return symbol.GetMembers()
            .OfType<IFieldSymbol>()
            .FirstOrDefault(p => p.Name == "Name")?.ConstantValue as string;
    }

    private static string[]? GetValidClaims(GeneratorAttributeSyntaxContext context, INamespaceOrTypeSymbol symbol)
    {
        var fieldSymbol = symbol.GetMembers()
            .OfType<IFieldSymbol>()
            .FirstOrDefault(p => p.Name == "Claims");

        if (fieldSymbol is not { IsStatic: true, DeclaringSyntaxReferences.Length: > 0 }) return null;

        var syntaxRef = fieldSymbol.DeclaringSyntaxReferences.First();
        var node = syntaxRef.GetSyntax() as VariableDeclaratorSyntax;
        if (node?.Initializer?.Value is not CollectionExpressionSyntax collectionExpression) return null;

        var semanticModel = context.SemanticModel;
        return collectionExpression.Elements
            .Select(s => GetClaimValue(s, semanticModel))
            .Where(value => value != null)
            .ToArray()!;
    }

    private static string? GetClaimValue(CollectionElementSyntax syntax, SemanticModel semanticModel)
    {
        if (syntax is not ExpressionElementSyntax expression) return null;

        if (expression.Expression is LiteralExpressionSyntax)
        {
            var constantValue = semanticModel.GetConstantValue(expression.Expression);
            if (constantValue is { HasValue: true, Value: string value })
            {
                return $"\"{value}\"";
            }
        }

        var symbolInfo = semanticModel.GetSymbolInfo(expression.Expression);
        return symbolInfo.Symbol?.ToString();
    }
}