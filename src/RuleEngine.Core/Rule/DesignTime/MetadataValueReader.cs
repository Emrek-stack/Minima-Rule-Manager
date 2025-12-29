using System.Text.Json;
using RuleEngine.Core.Rule.DesignTime.Serialization;

namespace RuleEngine.Core.Rule.DesignTime;

internal static class MetadataValueReader
{
    public static bool TryGet<T>(Dictionary<string, object> metadata, string key, out T? value)
    {
        value = default;
        if (!metadata.TryGetValue(key, out var raw))
            return false;

        if (raw is T typed)
        {
            value = typed;
            return true;
        }

        if (raw is JsonElement element)
        {
            value = element.Deserialize<T>(DesignTimeJson.Options);
            return value != null;
        }

        try
        {
            var json = JsonSerializer.Serialize(raw, DesignTimeJson.Options);
            value = JsonSerializer.Deserialize<T>(json, DesignTimeJson.Options);
            return value != null;
        }
        catch
        {
            return false;
        }
    }

    public static string? GetString(Dictionary<string, object> metadata, string key)
    {
        return TryGet<string>(metadata, key, out var value) ? value : null;
    }

    public static bool? GetBoolean(Dictionary<string, object> metadata, string key)
    {
        return TryGet<bool>(metadata, key, out var value) ? value : null;
    }
}