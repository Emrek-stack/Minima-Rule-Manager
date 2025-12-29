using Microsoft.Extensions.Logging;
using RuleEngine.Core.Abstractions;

namespace RuleEngine.Core.Rule.DesignTime;

/// <summary>
/// Initializes design-time metadata manager through DI.
/// </summary>
public sealed class MetadataManagerInitializer
{
    public MetadataManagerInitializer(IRuleRepository ruleRepository, ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("RuleEngine.Core.Rule.DesignTime.MetadataManager");
        MetadataManager.Configure(ruleRepository, logger);
    }
}