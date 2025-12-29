using System.Text.Json;
using System.Text.Json.Serialization;

namespace RuleEngine.Core.Rule.DesignTime.Serialization;

/// <summary>
/// Serializes System.Type as assembly-qualified name.
/// </summary>
public sealed class TypeJsonConverter : JsonConverter<Type>
{
    public override Type? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        var typeName = reader.GetString();
        if (string.IsNullOrWhiteSpace(typeName))
            return null;

        return Type.GetType(typeName, throwOnError: false) ?? typeof(object);
    }

    public override void Write(Utf8JsonWriter writer, Type value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.AssemblyQualifiedName);
    }
}