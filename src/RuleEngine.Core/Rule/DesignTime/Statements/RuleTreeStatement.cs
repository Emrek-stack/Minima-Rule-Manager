namespace RuleEngine.Core.Rule.DesignTime.Statements;

/// <summary>
/// Rule tree containing multiple statements.
/// </summary>
public class RuleTreeStatement : Statement
{
    /// <summary>
    /// Tree name (optional).
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Child statements.
    /// </summary>
    public List<Statement> Statements { get; private set; }

    /// <summary>
    /// Creates a rule tree statement.
    /// </summary>
    public RuleTreeStatement()
    {
        Statements = new List<Statement>();
    }
}
