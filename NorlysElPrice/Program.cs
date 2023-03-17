using NorlysElPrice.DataObject;
using NorlysElPrice.Service;

namespace NorlysElPrice;

internal class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length > 0)
        {
            var fromDateOrTime = args[0];
        }

        var displayPrice = PriceService.Today(await PriceService.List());

        if (displayPrice.Count <= 0)
        {
            Console.WriteLine("No matching data found");

            return;
        }

        Console.WriteLine(DateTime.Today.ToString("dddd, dd MMMM yyyy"));

        GenerateOutput("Lowest", PriceService.Lowest(displayPrice));
        GenerateOutput("Highest", PriceService.Highest(displayPrice));
        GenerateOutput(
                DateTime.Now.Hour.ToString(),
                PriceService.Now(displayPrice)
            );
    }

    private static void GenerateOutput(string context, PriceData? priceData)
    {
        if (priceData == null)
        {
            return;
        }

        Console.WriteLine("");
        Console.WriteLine("--- " + context + " ---");
        Console.WriteLine("Time: " + priceData.Time + " Price: " + Math.Round(priceData.Value / 100, 2));
    }
}
