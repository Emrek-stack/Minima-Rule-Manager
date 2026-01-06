namespace RuleEngine.Core.Rule.DesignTime.Statements;

/// <summary>
/// Complex rule statement.
/// </summary>
public class ComplexRuleStatement : Statement
{
    /// <summary>
    /// Complex rules can have optional names.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Creates a complex rule statement.
    /// </summary>
    /// <param name="ruleStr"></param>
    /// <param name="name"></param>
    public ComplexRuleStatement(string ruleStr, string name = "")
    {
        ExpressionString = ruleStr;
        Name = name;
    }
}
