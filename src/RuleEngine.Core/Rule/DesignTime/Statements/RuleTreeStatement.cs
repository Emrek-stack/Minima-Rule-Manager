namespace RuleEngine.Core.Rule.DesignTime.Statements;

/// <summary>
/// Birden fazla kuraldan oluşan kurallar kümesi
/// </summary>
public class RuleTreeStatement : Statement
{
    /// <summary>
    /// Kümenin ismi. Girilmesi zorunlu değildir.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Alt kurallar
    /// </summary>
    public List<Statement> Statements { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public RuleTreeStatement()
    {
        Statements = new List<Statement>();
    }
}