using System.Text.Json;
using System.Text.RegularExpressions;
using RuleEngine.Core.Rule.DesignTime.Serialization;
using RuleEngine.Core.Rule.DesignTime.Statements;

namespace RuleEngine.Core.Rule.DesignTime;

/// <summary>
/// Parses rule strings into statement trees.
/// </summary>
public static class RuleParser
{
    /// <summary>
    /// Regex patterns recognized as parameters in rule strings.
    /// </summary>
    public static readonly Regex ParameterMatch = new Regex(
        @"new HashSet<object>\(new object\[\]\{(?:\\.|[^\}\\])*\}\)" +
        @"|""(?:\\.|[^""\\])*""" +
        @"|(?<![A-Za-z{])\b\d+(?:[.]\d+)?[dM]?(?![A-Za-z}])" +
        @"|true|false" +
        @"|(?<=[@])[A-Za-z]+[A-Za-z0-9_]*" +
        @"|==|!=|<=|>=| < |(?<!=) > " +
        @"|'(?:\\.|[^'\\])*'",
        RegexOptions.Compiled);

    private static readonly Regex NamedRuleMatch = new Regex(
        @"^\/\*(?<rule_name>[A-Za-z](?:\\.|[^|\\])*)([|](?<rule_label>(?:\\.|[^|\\])*))?([|](?<parameter_labels>.*))?\*\/(?<code>.*)$",
        RegexOptions.Compiled | RegexOptions.Singleline);
    private static readonly Regex ObjectParameterMatch = new Regex(@"new HashSet<object>\(new object\[\]\{(?:\\.|[^\}\\])*\}\)", RegexOptions.Compiled);

    private static string ClearParameterMatch(string matchedValue)
    {
        if (ObjectParameterMatch.IsMatch(matchedValue))
            return matchedValue.Substring(33, matchedValue.Length - 35);
        return matchedValue.Trim('\'').Trim('"');
    }

    private static RuleTreeStatement Parse(string[] statementStrings, string? treeName = null)
    {
        var result = new RuleTreeStatement
        {
            Name = treeName,
            ExpressionString = string.Join(RuleGenerator.StatementSeperator, statementStrings)
        };
        for (var i = 0; i < statementStrings.Length; i++)
        {
            var statementStr = statementStrings[i].Trim();
            if (string.IsNullOrEmpty(statementStr))
                continue;

            if (statementStr.StartsWith("("))
            {
                string? name = null;
                if (statementStr.Length > 5 && statementStr.StartsWith("(/*") && statementStr.EndsWith("*/"))
                    name = statementStr.Substring(3, statementStr.Length - 5);
                var subTreeLines = new List<string>();
                i++;
                var paranDepth = 0;
                while (i < statementStrings.Length)
                {
                    statementStr = statementStrings[i].Trim();
                    if (paranDepth == 0 && statementStr == ")")
                        break;
                    if (statementStr.StartsWith("("))
                        paranDepth++;
                    if (statementStr == ")")
                        paranDepth--;
                    subTreeLines.Add(statementStr);
                    i++;
                }
                result.Statements.Add(Parse(subTreeLines.ToArray(), name));
                continue;
            }

            switch (statementStr)
            {
                case "&&":
                    result.Statements.Add(new AndOperatorStatement());
                    continue;
                case "||":
                    result.Statements.Add(new OrOperatorStatement());
                    continue;
                case "/**ComplexRule_Start**/":
                {
                    var subTreeLines = new List<string>();
                    i++;
                    statementStr = statementStrings[i];
                    var trimmedLine = statementStr.Trim();
                    var name = "";
                    while (i < statementStrings.Length && trimmedLine != "/**ComplexRule_End**/")
                    {
                        if (trimmedLine.StartsWith("/**") && trimmedLine.EndsWith("**/"))
                        {
                            name = trimmedLine.Substring(3, trimmedLine.Length - 6);
                        }
                        else
                        {
                            subTreeLines.Add(statementStr);
                        }

                        i++;
                        statementStr = statementStrings[i];
                        trimmedLine = statementStr.Trim();
                    }
                    result.Statements.Add(new ComplexRuleStatement(string.Join("\r\n", subTreeLines), name));
                    continue;
                }
            }

            if (statementStr.StartsWith("/*"))
            {
                var ruleParts = NamedRuleMatch.Match(statementStr);
                var ruleName = ruleParts.Groups["rule_name"]?.Value;
                var ruleLabel = ruleParts.Groups["rule_label"]?.Value;
                var ruleStr = ruleParts.Groups["code"]?.Value;
                var parameterLabels = ruleParts.Groups["parameter_labels"]?.Value;

                if (ruleName != null && ruleStr != null)
                {
                    var statement = new NamedRuleStatement
                    {
                        Name = ruleName,
                        Label = ruleLabel,
                        ExpressionString = ruleStr,
                        ParameterValues = ParameterMatch.Matches(ruleStr).Cast<Match>().Where(m => m.Success)
                            .Select(m => ClearParameterMatch(m.Value))
                            .ToList()
                    };

                    try
                    {
                        if (!string.IsNullOrWhiteSpace(parameterLabels))
                        {
                            statement.ParameterLabels = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(
                                parameterLabels,
                                DesignTimeJson.Options);
                        }
                    }
                    catch (Exception)
                    {
                    }
                    result.Statements.Add(statement);
                    continue;
                }
            }

            result.Statements.Add(new IncorrectRuleStatement(statementStr));
        }
        return result;
    }

    /// <summary>
    /// Parses a rule string and returns a statement tree.
    /// </summary>
    /// <param name="str">Rule string to parse.</param>
    /// <returns></returns>
    public static RuleTreeStatement Parse(string str)
    {
        var statementStrings = str.Split(new[] { RuleGenerator.StatementSeperator }, StringSplitOptions.None);
        return Parse(statementStrings);
    }
}
