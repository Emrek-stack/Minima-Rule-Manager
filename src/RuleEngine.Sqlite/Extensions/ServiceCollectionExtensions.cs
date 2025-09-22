using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RuleEngine.Core.Abstractions;
using RuleEngine.Core.Extensions;
using RuleEngine.Sqlite.Data;
using RuleEngine.Sqlite.Repositories;

namespace RuleEngine.Sqlite.Extensions
{
    /// <summary>
    /// Extension methods for registering RuleEngine SQLite services
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds RuleEngine with SQLite persistence
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="connectionString">SQLite connection string</param>
        /// <returns>Service collection for chaining</returns>
        public static IServiceCollection AddRuleEngineWithSqlite(this IServiceCollection services, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string cannot be null or empty", nameof(connectionString));

            // Add RuleEngine core services
            services.AddRuleEngine();

            // Add DbContext
            services.AddDbContext<RuleDbContext>(options =>
                options.UseSqlite(connectionString));

            // Add repositories
            services.TryAddScoped<IRuleRepository, SqliteRuleRepository>();
            services.TryAddScoped<IAuditRepository, SqliteAuditRepository>();

            return services;
        }

        /// <summary>
        /// Adds RuleEngine with SQLite persistence and custom evaluator
        /// </summary>
        /// <typeparam name="TEvaluator">Type of the custom evaluator</typeparam>
        /// <param name="services">Service collection</param>
        /// <param name="connectionString">SQLite connection string</param>
        /// <returns>Service collection for chaining</returns>
        public static IServiceCollection AddRuleEngineWithSqlite<TEvaluator>(this IServiceCollection services, string connectionString)
            where TEvaluator : class, IRuleEvaluator
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string cannot be null or empty", nameof(connectionString));

            // Add RuleEngine core services
            services.AddRuleEngine();

            // Add DbContext
            services.AddDbContext<RuleDbContext>(options =>
                options.UseSqlite(connectionString));

            // Add repositories
            services.TryAddScoped<IRuleRepository, SqliteRuleRepository>();
            services.TryAddScoped<IAuditRepository, SqliteAuditRepository>();

            return services;
        }
    }
}
