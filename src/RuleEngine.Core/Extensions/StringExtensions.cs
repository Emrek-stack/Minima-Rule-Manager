using System.Text;

namespace RuleEngine.Core.Extensions;

/// <summary>
/// String helper extensions used by design-time rule rendering.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Repeats the string <paramref name="count"/> times.
    /// </summary>
    /// <param name="str"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static string Repeat(this string str, int count)
    {
        return string.Concat(Enumerable.Repeat(str, count));
    }

    /// <summary>
    /// Truncates the string to <paramref name="count"/> characters and appends <paramref name="suffix"/>.
    /// </summary>
    /// <param name="str"></param>
    /// <param name="count"></param>
    /// <param name="suffix"></param>
    /// <returns></returns>
    public static string Elapsis(this string str, int count, string suffix = "...")
    {
        if (str == null)
            return "";
        var builder = new StringBuilder(new string(str.Take(count).ToArray()));
        if (str.Length > count)
            builder.Append(suffix);
        return builder.ToString();
    }
}