using System.Text.Json;
using System.Text.Json.Serialization;
using RuleEngine.Core.Rule.DesignTime.Parameters;

namespace RuleEngine.Core.Rule.DesignTime.Serialization;

/// <summary>
/// Polymorphic converter for ParameterDefinition types using the "Type" discriminator.
/// </summary>
public sealed class ParameterDefinitionJsonConverter : JsonConverter<ParameterDefinition>
{
    private static readonly Dictionary<string, Type> TypeMap = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
    {
        { nameof(StringParameter), typeof(StringParameter) },
        { nameof(NumericParameter), typeof(NumericParameter) },
        { nameof(ListParameter), typeof(ListParameter) },
        { nameof(BooleanListParameter), typeof(BooleanListParameter) },
        { nameof(EqualityListParameter), typeof(EqualityListParameter) },
        { nameof(ArrayParameter), typeof(ArrayParameter) },
        { nameof(DateTimeParameter), typeof(DateTimeParameter) },
        { nameof(DateTimeGroupParameter), typeof(DateTimeGroupParameter) },
        { nameof(ReadonlyParameter), typeof(ReadonlyParameter) },
        { nameof(DefinitionParameter), typeof(DefinitionParameter) },
        { nameof(DestinationParameter), typeof(DestinationParameter) }
    };

    public override ParameterDefinition? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        if (!document.RootElement.TryGetProperty("Type", out var typeProperty))
            throw new JsonException("ParameterDefinition.Type is required for polymorphic deserialization.");

        var typeName = typeProperty.GetString();
        if (string.IsNullOrWhiteSpace(typeName) || !TypeMap.TryGetValue(typeName, out var targetType))
            throw new JsonException($"Unknown ParameterDefinition type: '{typeName}'.");

        var json = document.RootElement.GetRawText();
        return (ParameterDefinition?)JsonSerializer.Deserialize(json, targetType, options);
    }

    public override void Write(Utf8JsonWriter writer, ParameterDefinition value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (object)value, value.GetType(), options);
    }
}