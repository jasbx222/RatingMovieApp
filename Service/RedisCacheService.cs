using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using MovieRatingAPI.Interface;

namespace MovieRatingAPI.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;

        public RedisCacheService(
            IDistributedCache cache)
        {
            _cache = cache;
        }

        // =========================
        // GET
        // =========================
        public async Task<T?> GetData<T>(
            string key)
        {
            var cachedData =
                await _cache.GetStringAsync(key);

            if (string.IsNullOrEmpty(cachedData))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(
                cachedData);
        }

        // =========================
        // SET
        // =========================
        public async Task SetData<T>(
            string key,
            T data,
            TimeSpan expiration)
        {
            var options =
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow =
                        expiration
                };

            var jsonData =
                JsonSerializer.Serialize(data);

            await _cache.SetStringAsync(
                key,
                jsonData,
                options);
        }

        // =========================
        // REMOVE
        // =========================
        public async Task RemoveData(
            string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}