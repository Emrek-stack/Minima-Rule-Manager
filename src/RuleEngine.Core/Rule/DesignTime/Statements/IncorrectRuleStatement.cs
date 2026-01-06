namespace RuleEngine.Core.Rule.DesignTime.Statements;

/// <summary>
/// Represents an invalid rule statement.
/// </summary>
public class IncorrectRuleStatement : Statement
{
    /// <summary>
    /// Creates an incorrect rule statement.
    /// </summary>
    /// <param name="ruleStr"></param>
    public IncorrectRuleStatement(string ruleStr)
    {
        ExpressionString = ruleStr;
    }
}
