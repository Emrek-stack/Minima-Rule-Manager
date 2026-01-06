namespace RuleEngine.Core.Rule.DesignTime.Statements;

/// <summary>
/// && operator statement.
/// </summary>
public class AndOperatorStatement : Statement
{
    /// <summary>
    /// Creates an AND operator statement.
    /// </summary>
    public AndOperatorStatement()
    {
        ExpressionString = "&&";
    }
}
