using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace PolicyGenerator.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class PolicyClassAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor NameFieldMustExist = new(
        id: DiagnosticIds.PG0001NameFieldMustExist,
        title: "Policy type must have a Name field",
        messageFormat: "Policy type '{0}' must have a 'Name' field",
        category: "PolicyGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Classes annotated with a Policy attribute should have a 'Name' field."
    );

    private static readonly DiagnosticDescriptor ClaimsFieldMustExist = new(
        id: DiagnosticIds.PG0002ClaimsFieldMustExist,
        title: "Policy type must have a Claims field",
        messageFormat: "Policy type '{0}' must have a 'Claims' field",
        category: "PolicyGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Classes annotated with a Policy attribute should have a 'Claims' field."
    );

    private static readonly DiagnosticDescriptor NameFieldMustBeStaticOrConst = new(
        id: DiagnosticIds.PG0003NameFieldMustBeStaticOrConst,
        title: "Policy type Name field must be static or const",
        messageFormat: "Policy type '{0}' Name field must be static or const",
        category: "PolicyGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The Name field of a Policy type must be static or const."
    );

    private static readonly DiagnosticDescriptor ClaimsFieldMustBeStatic = new(
        id: DiagnosticIds.PG0004ClaimsFieldMustBeStatic,
        title: "Policy type Claims field must be static",
        messageFormat: "Policy type '{0}' Claims field must be static",
        category: "PolicyGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The Claims field of a Policy type must be static."
    );

    private static readonly DiagnosticDescriptor NameFieldMustBeString = new(
        id: DiagnosticIds.PG0005NameFieldMustBeString,
        title: "Policy type Name field must be string",
        messageFormat: "Policy type '{0}' Name field must be string",
        category: "PolicyGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The Name field of a Policy type must be string."
    );

    private static readonly DiagnosticDescriptor ClaimsFieldMustBeStringCollection = new(
        id: DiagnosticIds.PG0006ClaimsFieldMustBeStringCollection,
        title: "Policy type Claims field must be string collection",
        messageFormat: "Policy type '{0}' Claims field must be string collection",
        category: "PolicyGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The Claims field of a Policy type must be string collection."
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create([
        NameFieldMustExist,
        ClaimsFieldMustExist,
        NameFieldMustBeStaticOrConst,
        ClaimsFieldMustBeStatic,
        NameFieldMustBeString,
        ClaimsFieldMustBeStringCollection
    ]);

    public override void Initialize(AnalysisContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
    }

    private static void AnalyzeSymbol(SymbolAnalysisContext context)
    {
        var token = context.CancellationToken;
        token.ThrowIfCancellationRequested();

        if (context.Symbol is not INamedTypeSymbol symbol)
        {
            return;
        }

        if (!symbol.GetAttributes().Any(x => x.AttributeClass?.ToString() == "PolicyGenerator.PolicyAttribute"))
        {
            return;
        }

        token.ThrowIfCancellationRequested();

        var nameField = symbol.GetMembers().OfType<IFieldSymbol>().FirstOrDefault(f => f.Name == "Name");
        if (nameField == null)
        {
            context.ReportDiagnostic(Diagnostic.Create(NameFieldMustExist, symbol.Locations[0], symbol.Name));
        }
        else
        {
            if (nameField is { IsStatic: false, IsConst: false })
            {
                context.ReportDiagnostic(Diagnostic.Create(NameFieldMustBeStaticOrConst, nameField.Locations[0],
                    symbol.Name));
            }

            if (nameField.Type.SpecialType != SpecialType.System_String)
            {
                context.ReportDiagnostic(Diagnostic.Create(NameFieldMustBeString, nameField.Locations[0], symbol.Name));
            }
        }

        var claimsField = symbol.GetMembers().OfType<IFieldSymbol>().FirstOrDefault(f => f.Name == "Claims");
        if (claimsField == null)
        {
            context.ReportDiagnostic(Diagnostic.Create(ClaimsFieldMustExist, symbol.Locations[0], symbol.Name));
        }
        else
        {
            if (claimsField is { IsStatic: false })
            {
                context.ReportDiagnostic(Diagnostic.Create(ClaimsFieldMustBeStatic, claimsField.Locations[0],
                    symbol.Name));
            }

            var isStringEnumerable = IsIEnumerableOfString(claimsField.Type);
            if (!isStringEnumerable)
            {
                context.ReportDiagnostic(Diagnostic.Create(ClaimsFieldMustBeStringCollection, claimsField.Locations[0],
                    symbol.Name));
            }
        }
    }

    private static bool IsIEnumerableOfString(ITypeSymbol type)
    {
        if (type is IArrayTypeSymbol { ElementType.SpecialType: SpecialType.System_String })
        {
            return true;
        }

        if (type is not INamedTypeSymbol namedType) return false;
        if (namedType.ConstructedFrom.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T &&
            namedType.TypeArguments is [{ SpecialType: SpecialType.System_String }])
        {
            return true;
        }

        foreach (var @interface in namedType.AllInterfaces)
        {
            if (@interface.ConstructedFrom.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T &&
                @interface.TypeArguments is [{ SpecialType: SpecialType.System_String }])
            {
                return true;
            }
        }

        return false;
    }
}