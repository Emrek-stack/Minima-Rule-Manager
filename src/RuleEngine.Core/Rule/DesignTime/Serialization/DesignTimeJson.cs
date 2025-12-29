using System.Text.Json;
using System.Text.Json.Serialization;

namespace RuleEngine.Core.Rule.DesignTime.Serialization;

/// <summary>
/// Shared JSON options for design-time metadata parsing.
/// </summary>
public static class DesignTimeJson
{
    public static readonly JsonSerializerOptions Options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    static DesignTimeJson()
    {
        Options.Converters.Add(new TypeJsonConverter());
        Options.Converters.Add(new ParameterDefinitionJsonConverter());
    }
}