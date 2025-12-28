using System;
using System.Threading.Tasks;
using CampaignEngine.Core.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace CampaignEngine.Core.Cache
{
    public class MemoryCacheProvider : ICacheProvider
    {
        private readonly IMemoryCache _cache;

        public MemoryCacheProvider(IMemoryCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public T? Get<T>(string key) => _cache.TryGetValue(key, out T? value) ? value : default;

        public void Set<T>(string key, T value, TimeSpan expiration) => _cache.Set(key, value, expiration);

        public T? GetOrCreate<T>(string key, Func<T> factory, TimeSpan expiration) => _cache.GetOrCreate(key, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = expiration;
            return factory();
        });

        public async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan expiration) => await _cache.GetOrCreateAsync(key, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = expiration;
            return await factory();
        });

        public string GenerateKey(params object[] parts) => string.Join(":", parts);

        public void RemoveByPrefix(string prefix)
        {
            // Memory cache doesn't support prefix removal natively
            // This is a simplified implementation
        }
    }
}
