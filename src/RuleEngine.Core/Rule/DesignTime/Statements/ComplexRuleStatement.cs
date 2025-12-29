namespace RuleEngine.Core.Rule.DesignTime.Statements;

/// <summary>
/// Kompleks kural
/// </summary>
public class ComplexRuleStatement : Statement
{
    /// <summary>
    /// Kompleks kurallara isim verilebilir. İsimli kurallardaki gibi zorunlu değildir.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ruleStr"></param>
    /// <param name="name"></param>
    public ComplexRuleStatement(string ruleStr, string name = "")
    {
        ExpressionString = ruleStr;
        Name = name;
    }
}