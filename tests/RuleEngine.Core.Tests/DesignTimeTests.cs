using System.Text.RegularExpressions;
using FluentAssertions;
using RuleEngine.Core.Rule.DesignTime;
using RuleEngine.Core.Rule.DesignTime.Metadatas;
using RuleEngine.Core.Rule.DesignTime.Parameters;
using RuleEngine.Core.Rule.DesignTime.Statements;

namespace RuleEngine.Core.Tests;

public class DesignTimeTests
{
    private readonly IDictionary<string, NamedRuleMetadata> _namedRules = new Dictionary<string, NamedRuleMetadata>
    {
        ["Rule1"] = new NamedRuleMetadata("Title1")
        {
            ExpressionFormat = "(Rule1Statement {0} {1})",
            ParameterDefinations = new List<ParameterDefinition>
            {
                new StringParameter(""),
                new NumericParameter("")
            }
        },
        ["Rule2"] = new NamedRuleMetadata("Title2")
        {
            ExpressionFormat = "(Rule2Statement\r\n{0} {1} {2})",
            ParameterDefinations = new List<ParameterDefinition>
            {
                new StringParameter(""),
                new NumericParameter(""),
                new BooleanListParameter("")
            }
        },
        ["Rule3"] = new NamedRuleMetadata("Title3")
        {
            ExpressionFormat = "Rule3Function(new HashSet<object>(new object[]{{{0}}}), {1})",
            ParameterDefinations = new List<ParameterDefinition>
            {
                new ArrayParameter("", typeof(int)),
                new ListParameter("")
                {
                    Items = new Dictionary<string, ListParameterItem>
                    {
                        { "ListItem1", new ListParameterItem("List item 1", "\"{0}\"") },
                        { "ListItem2", new ListParameterItem("List item 2", "\"{0}\"") }
                    }
                }
            }
        }
    };

    private readonly List<Statement> _statements = new()
    {
        new NamedRuleStatement
        {
            Name = "Rule1",
            Label = "Rule1 Label",
            ParameterValues = new List<string> { "Param1", "2" }
        },
        new AndOperatorStatement(),
        new NamedRuleStatement
        {
            Name = "Rule2",
            Label = "Rule2 Label",
            ParameterValues = new List<string> { "Param1", "2", "true" }
        },
        new OrOperatorStatement(),
        new NamedRuleStatement
        {
            Name = "Rule3",
            Label = "Rule3 Label",
            ParameterValues = new List<string> { "\"1\",\"2\",\"3\"", "ListItem1" },
            ParameterLabels = new Dictionary<string, List<string>>
            {
                { "0", new List<string> { "Label for 1", "Label for 2", "Label for 3" } },
                { "1", new List<string> { "Label for ListItem1" } }
            }
        }
    };

    private string GenerateRuleString()
    {
        var tree = new RuleTreeStatement();
        tree.Statements.AddRange(_statements);
        return RuleGenerator.Generate(tree, 0, "", _namedRules);
    }

    [Fact]
    public void Generate_ShouldContainExpectedSegments()
    {
        var ruleString = GenerateRuleString();
        var ruleStringLines = Regex.Split(ruleString, "\r\n");

        ruleStringLines[0].Should().Contain("Rule1Statement");
        ruleStringLines[1].Should().Contain("&&");
        ruleStringLines[2].Should().Contain("Rule2Statement");
        ruleStringLines[4].Should().Contain("||");
        ruleStringLines[5].Should().Contain("Rule3Function");
    }

    [Fact]
    public void Parse_ShouldReconstructStatements()
    {
        var ruleTreeStatement = RuleParser.Parse(GenerateRuleString());
        ruleTreeStatement.Statements.Should().HaveCount(_statements.Count);

        for (var i = 0; i < ruleTreeStatement.Statements.Count; i++)
        {
            var statement = ruleTreeStatement.Statements[i];
            statement.Type.Should().Be(_statements[i].Type);

            if (statement is NamedRuleStatement namedRuleStatement)
            {
                var expected = (NamedRuleStatement)_statements[i];
                namedRuleStatement.Name.Should().Be(expected.Name);
                namedRuleStatement.Label.Should().Be(expected.Label);
                if (expected.Name == "Rule3")
                {
                    namedRuleStatement.ParameterValues.Should().HaveCount(2);
                    namedRuleStatement.ParameterValues[0].Should().StartWith("new HashSet<object>");
                    namedRuleStatement.ParameterValues[1].Should().Be("ListItem1");
                }
                else
                {
                    namedRuleStatement.ParameterValues.Should().BeEquivalentTo(
                        expected.ParameterValues,
                        options => options.WithStrictOrdering());
                }
            }
        }
    }
}
