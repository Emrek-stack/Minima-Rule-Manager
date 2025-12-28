using CampaignEngine.Core.Cache;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace CampaignEngine.Core.Tests.Cache;

public class MemoryCacheProviderTests
{
    [Fact]
    public void Get_WithNonExistentKey_ShouldReturnDefault()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var provider = new MemoryCacheProvider(cache);
        var result = provider.Get<string>("nonexistent");
        result.Should().BeNull();
    }

    [Fact]
    public void Set_ShouldStoreValue()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var provider = new MemoryCacheProvider(cache);
        provider.Set("key1", "value1", TimeSpan.FromMinutes(5));
        var result = provider.Get<string>("key1");
        result.Should().Be("value1");
    }

    [Fact]
    public void GetOrCreate_ShouldCreateIfNotExists()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var provider = new MemoryCacheProvider(cache);
        var result = provider.GetOrCreate("key2", () => "created", TimeSpan.FromMinutes(5));
        result.Should().Be("created");
    }

    [Fact]
    public void GetOrCreate_ShouldReturnExistingValue()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var provider = new MemoryCacheProvider(cache);
        provider.Set("key3", "existing", TimeSpan.FromMinutes(5));
        var result = provider.GetOrCreate("key3", () => "new", TimeSpan.FromMinutes(5));
        result.Should().Be("existing");
    }

    [Fact]
    public async Task GetOrCreateAsync_ShouldCreateIfNotExists()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var provider = new MemoryCacheProvider(cache);
        var result = await provider.GetOrCreateAsync("key4", async () => await Task.FromResult("async"), TimeSpan.FromMinutes(5));
        result.Should().Be("async");
    }

    [Fact]
    public void GenerateKey_ShouldCombineParts()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var provider = new MemoryCacheProvider(cache);
        var key = provider.GenerateKey("part1", 123, "part2");
        key.Should().Be("part1:123:part2");
    }
}
