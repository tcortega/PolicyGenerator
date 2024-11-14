using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace PolicyGenerator.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class PolicyClassAnalyzer : DiagnosticAnalyzer
{
	public static readonly DiagnosticDescriptor NameFieldMustExist = new(
		id: DiagnosticIds.PG0001NameFieldMustExist,
		title: "Policy type must have a Name field",
		messageFormat: "Policy type '{0}' must have a 'Name' field",
		category: "PolicyGenerator",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		description: "Classes annotated with a Policy attribute should have a 'Name' field."
	);

	public static readonly DiagnosticDescriptor RequiredFieldsMissing = new(
		id: DiagnosticIds.PG0002RequiredFieldsMissing,
		title: "Policy type must have at least one required field",
		messageFormat: "Policy type '{0}' must have at least one of these fields: 'Claims', 'Roles', or 'AuthenticationSchemes'",
		category: "PolicyGenerator",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		description: "Classes annotated with a Policy attribute must have at least one of: Claims, Roles, or AuthenticationSchemes fields."
	);

	public static readonly DiagnosticDescriptor NameFieldMustBeStaticOrConst = new(
		id: DiagnosticIds.PG0003NameFieldMustBeStaticOrConst,
		title: "Policy type Name field must be static or const",
		messageFormat: "Policy type '{0}' Name field must be static or const",
		category: "PolicyGenerator",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		description: "The Name field of a Policy type must be static or const."
	);

	public static readonly DiagnosticDescriptor RequiredFieldMustBeStatic = new(
		id: DiagnosticIds.PG0004RequiredFieldMustBeStatic,
		title: "Policy type required field must be static",
		messageFormat: "Policy type '{0}' field '{1}' must be static",
		category: "PolicyGenerator",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		description: "The Claims, Roles, and AuthenticationSchemes fields of a Policy type must be static."
	);

	public static readonly DiagnosticDescriptor NameFieldMustBeString = new(
		id: DiagnosticIds.PG0005NameFieldMustBeString,
		title: "Policy type Name field must be string",
		messageFormat: "Policy type '{0}' Name field must be string",
		category: "PolicyGenerator",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		description: "The Name field of a Policy type must be string."
	);

	public static readonly DiagnosticDescriptor RequiredFieldMustBeStringCollection = new(
		id: DiagnosticIds.PG0006RequiredFieldMustBeStringCollection,
		title: "Policy type required field must be string collection",
		messageFormat: "Policy type '{0}' field '{1}' must be string collection",
		category: "PolicyGenerator",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		description: "The Claims, Roles, and AuthenticationSchemes fields of a Policy type must be string collections."
	);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create([
		NameFieldMustExist,
		RequiredFieldsMissing,
		NameFieldMustBeStaticOrConst,
		RequiredFieldMustBeStatic,
		NameFieldMustBeString,
		RequiredFieldMustBeStringCollection
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

		AnalyzeNameField(context, symbol);
		AnalyzeRequiredFields(context, symbol);
	}

	private static void AnalyzeNameField(SymbolAnalysisContext context, INamedTypeSymbol symbol)
	{
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
	}

	private static void AnalyzeRequiredFields(SymbolAnalysisContext context, INamedTypeSymbol symbol)
	{
		var requiredFieldNames = new[] { "Claims", "Roles", "AuthenticationSchemes" };
		var fields = symbol.GetMembers()
			.OfType<IFieldSymbol>()
			.Where(f => requiredFieldNames.Contains(f.Name))
			.ToList();

		if (fields.Count == 0)
		{
			context.ReportDiagnostic(Diagnostic.Create(RequiredFieldsMissing, symbol.Locations[0], symbol.Name));
			return;
		}

		foreach (var field in fields)
		{
			if (!field.IsStatic)
			{
				context.ReportDiagnostic(Diagnostic.Create(RequiredFieldMustBeStatic, field.Locations[0],
					symbol.Name, field.Name));
			}

			var isStringEnumerable = IsIEnumerableOfString(field.Type);
			if (!isStringEnumerable)
			{
				context.ReportDiagnostic(Diagnostic.Create(RequiredFieldMustBeStringCollection, field.Locations[0],
					symbol.Name, field.Name));
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
