namespace RuleEngine.Core.Extensions;

/// <summary>
/// Type extension methods
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Gets a friendly name for a type, including generic type parameters
    /// </summary>
    /// <param name="type">The type</param>
    /// <returns>Friendly type name</returns>
    public static string GetFriendlyName(this Type type)
    {
        if (type.IsGenericType)
        {
            var genericArgs = type.GetGenericArguments();
            var typeName = type.Name.Split('`')[0];
            return $"{typeName}<{string.Join(", ", genericArgs.Select(GetFriendlyName))}>";
        }
        return type.Name;
    }
}