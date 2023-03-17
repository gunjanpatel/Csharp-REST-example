using StackExchange.Redis;
using System.Net.Http.Headers;
using Microsoft.Extensions.Caching.Distributed;

namespace NorlysElPrice.Service
{
    public class Client
    {
        private static readonly string cacheKey = "norlys-price-cache-key";

        private static readonly string endpointUrl = "https://norlys.dk/api/flexel/getall?days=1&sector=DK2";

        private static readonly ConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect("localhost");

        private static readonly TimeSpan cacheExpiryTimeAtMidnight = TimeSpan.FromHours((DateTime.Now.Date.AddDays(1) - DateTime.Now).TotalHours);

        public static async Task<string> GetData()
        {
            // Create a cache object
            IDatabase cache = connectionMultiplexer.GetDatabase();

            // Check if the response is already cached
            var cachedData = await cache.StringGetAsync(cacheKey);

            if (cachedData.HasValue)
            {
                // If the response is already cached, return the cached data
                return cachedData;
            }

            string response = await GetResponse();

            var options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(cacheExpiryTimeAtMidnight);

            // Serialize response and store in cache
            await cache.StringSetAsync(cacheKey, response, options.SlidingExpiration);

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
