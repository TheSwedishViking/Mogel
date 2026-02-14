using TempData_grupparbete.Models;
using TempData_grupparbete.Services;

namespace TempData_grupparbete
{
    internal class Program
    {
        public static List<WeatherData> WeatherData = new List<WeatherData>();

        static async Task Main(string[] args)
        {
            await Writer.Delete("baddata.txt");
            await Writer.Delete("moldinfo.txt");
            ReadFile.ReadAll();
            await CollectedDataDisplay.WriteMold(WeatherData);
            await UI.StartMenu();

        }
    }
}
