using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempData_grupparbete.Services
{
    internal class UI
    {
        public static async Task StartMenu()
        {
            Console.WriteLine("Sammanställning av data tog " + ReadFile.sw + "ms");
            ConsoleKey key = Console.ReadKey(true).Key;
        Action action = key switch
        {
            ConsoleKey.D => () => CollectedDataDisplay.DisplayDailyTemp(ReadFile.weatherData),
            ConsoleKey.S => () => { Console.WriteLine("---SÖK---\n"); Search.SearchInput(ReadFile.weatherData); },
            ConsoleKey.B => () => { Console.WriteLine("---Felaktig data---\n");
                Console.WriteLine($"{"Rad",-10} | Data\n"); Console.WriteLine(ReadFile.sbBadData); },
            ConsoleKey.M => () => CollectedDataDisplay.DisplayDailyMonth(ReadFile.weatherData),
            _ => () => { Console.WriteLine("Felaktig inmantning"); Thread.Sleep(1000); }
        };
        action();
        }
    }
}
