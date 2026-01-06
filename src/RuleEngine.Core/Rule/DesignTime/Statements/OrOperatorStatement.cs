namespace RuleEngine.Core.Rule.DesignTime.Statements;

/// <summary>
/// || operator statement.
/// </summary>
public class OrOperatorStatement : Statement
{
    /// <summary>
    /// Creates an OR operator statement.
    /// </summary>
    public OrOperatorStatement()
    {
        ExpressionString = "||";
    }
}
