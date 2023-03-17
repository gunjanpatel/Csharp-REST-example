using NorlysElPrice.DataObject;
using System;
using System.Text.Json;

namespace NorlysElPrice.Service
{
    public class PriceService
    {
        public static async Task<List<Price>> List()
        {
            return JsonSerializer.Deserialize<List<Price>>(
                    await Client.GetData(),
                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }
                ) ?? new();
        }

        public static List<PriceData> Today(List<Price> prices)
        {
            return prices.FirstOrDefault(
                p => p.PriceDate == DateTime.Today, new Price()
            ).DisplayPrices ?? new List<PriceData>();
        }

        public static PriceData? Lowest(List<PriceData> dataList) => dataList.MinBy(p => p.Value);
        public static PriceData? Highest(List<PriceData> dataList) => dataList.MaxBy(p => p.Value);
        public static PriceData? Now(List<PriceData> dataList) => dataList
                    .Where(
                        p => p.Time != null && Int16.Parse(p.Time) == DateTime.Now.Hour
                    ).FirstOrDefault();
    }
}
