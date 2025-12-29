using RuleEngine.Core.Rule.DesignTime.Metadatas;
using RuleEngine.Core.Rule.DesignTime.Statements;

namespace RuleEngine.Core.Rule.DesignTime;

/// <summary>
/// Parse edilmiş kuraldan string oluşturmak için kullanılır.
/// </summary>
public static class RuleGenerator
{
    public const string StatementSeperator = " /**Statement_End**/\r\n";

    /// <summary>
    /// Parse edilmiş bir kuralı, string kural haline dönüştürür.
    /// </summary>
    /// <param name="statement"></param>
    /// <param name="depth"></param>
    /// <param name="indent"></param>
    /// <param name="namedRules">Statement içindeki isimli kurallar buradan verilmelidir.</param>
    /// <returns></returns>
    public static string Generate(Statement statement,
        int depth = 0,
        string? indent = null,
        IDictionary<string, NamedRuleMetadata>? namedRules = null)
    {
        if (statement == null)
            return "";

        indent ??= string.Empty;

        if (statement is AndOperatorStatement andStatement)
            return MetadataManager.PredefinedMetadatas.AndOperator.GenerateExpressionString(andStatement, depth, indent);

        if (statement is OrOperatorStatement orStatement)
            return MetadataManager.PredefinedMetadatas.OrOperator.GenerateExpressionString(orStatement, depth, indent);

        if (statement is ComplexRuleStatement complexStatement)
            return MetadataManager.PredefinedMetadatas.ComplexRule.GenerateExpressionString(complexStatement, depth, indent);

        if (statement is IncorrectRuleStatement incorrectStatement)
            return MetadataManager.PredefinedMetadatas.IncorrectRule.GenerateExpressionString(incorrectStatement, depth, indent);

        if (statement is RuleTreeStatement ruleTreeStatement)
            return MetadataManager.PredefinedMetadatas.RuleTree.GenerateExpressionString(ruleTreeStatement, depth, indent, namedRules);

        if (statement is NamedRuleStatement namedRuleStatement)
        {
            if (namedRules == null)
            {
                do
                {
                    namedRules = MetadataManager.NamedRuleMetadatas;
                } while (!MetadataManager.Initialized);
            }

            if (namedRules == null || !namedRules.ContainsKey(namedRuleStatement.Name))
                return $"//ERROR: The Rule({namedRuleStatement.Name}) is not found";

            return namedRules[namedRuleStatement.Name].GenerateExpressionString(namedRuleStatement, depth, indent);
        }

        return $"//ERROR: The Rule type is not defined: {statement.Type}";
    }
}