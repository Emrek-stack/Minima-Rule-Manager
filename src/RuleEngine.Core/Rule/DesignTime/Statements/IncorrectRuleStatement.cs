namespace RuleEngine.Core.Rule.DesignTime.Statements;

/// <summary>
/// Hatalı kuralları ifade eden sınıf
/// </summary>
public class IncorrectRuleStatement : Statement
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ruleStr"></param>
    public IncorrectRuleStatement(string ruleStr)
    {
        ExpressionString = ruleStr;
    }
}