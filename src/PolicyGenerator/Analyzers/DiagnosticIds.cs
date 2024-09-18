using System.Diagnostics.CodeAnalysis;

namespace PolicyGenerator.Analyzers;

[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Diagnostic IDs start with PG")]
internal static class DiagnosticIds
{
    public const string PG0001NameFieldMustExist = "PG0001";
    public const string PG0002ClaimsFieldMustExist = "PG0002";

    public const string PG0003NameFieldMustBeStaticOrConst = "PG0003";
    public const string PG0004ClaimsFieldMustBeStatic = "PG0004";

    public const string PG0005NameFieldMustBeString = "PG0005";
    public const string PG0006ClaimsFieldMustBeStringCollection = "PG0006";
}