using StackExchange.Redis;
using System.Net.Http.Headers;
using Microsoft.Extensions.Caching.Distributed;

namespace NorlysElPrice.Service
{
    public class Client
    {
        private static readonly string cacheKey = "norlys-price-cache-key";

        private static readonly string endpointUrl = "https://norlys.dk/api/flexel/getall?days=1&sector=DK2";

        private static readonly TimeSpan cacheExpiryTimeAtMidnight = TimeSpan.FromHours((DateTime.Now.Date.AddDays(1) - DateTime.Now).TotalHours);

        public static async Task<string> GetData()
        {
            IDatabase? cache = null;
            
            string? cachedData = null;

            try
            {
                ConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect("localhost");

                // Create a cache object
                cache = connectionMultiplexer.GetDatabase();

                cachedData = await cache.StringGetAsync(cacheKey);
            }
            catch (RedisConnectionException ex)
            {
                // Handle Redis connection errors
                // Console.WriteLine($"Redis connection error: {ex.Message}");
            }
            catch (RedisTimeoutException ex)
            {
                // Handle Redis timeout errors
                // Console.WriteLine($"Redis timeout error: {ex.Message}");
            }

            if (cachedData != null)
            {
                // If the response is already cached, return the cached data
                return cachedData;
            }

            string response = await GetResponse();

            var options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(cacheExpiryTimeAtMidnight);

            // Serialize response and store in cache
            if (cache != null)
            {
                await cache.StringSetAsync(cacheKey, response, options.SlidingExpiration);
            }

            return response;
        }

        protected static async Task<string> GetResponse()
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );

            return await client.GetStringAsync(endpointUrl);
        }
    }
}
