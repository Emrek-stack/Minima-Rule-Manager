using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RuleEngine.Core.Abstractions;

namespace RuleEngine.Core.Extensions
{
    /// <summary>
    /// Extension methods for registering RuleEngine services
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds RuleEngine core services to the service collection
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <returns>Service collection for chaining</returns>
        public static IServiceCollection AddRuleEngine(this IServiceCollection services)
        {
            // Gordios RuleEngine uses static RuleManager, no DI registration needed
            return services;
        }
    }
}
