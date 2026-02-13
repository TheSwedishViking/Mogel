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
            Console.WriteLine("Grupp 5 Väderdata applikation");
            Console.WriteLine($"[D] för att visa medelvärde av dagliga temperaturer\n" +
                              $"[M] för att visa medelvärde av månadstemperaturer\n" +
                              $"[H] för Meterologisk Höst\n" +
                              $"[V] för Meterologisk Vinter\n" +
                              $"[S] för att söka\n" +
                              $"[B] för att visa felaktig data");
            ConsoleKey key = Console.ReadKey(true).Key;
            Action action = key switch
            {
                ConsoleKey.D => async () => await CollectedDataDisplay.DisplayDailyTemp(ReadFile.weatherData),
                ConsoleKey.S => async () => { Console.WriteLine("---SÖK---\n"); Search.SearchInput(ReadFile.weatherData); },
                ConsoleKey.B => () => { Console.WriteLine("---Felaktig data---\n");
                    Console.WriteLine($"{"Rad",-10} | Data\n"); CollectedDataDisplay.DisplayBadData(); },
                ConsoleKey.M => async () => await CollectedDataDisplay.DisplayDailyMonth(ReadFile.weatherData),
                ConsoleKey.H => () => { },
                ConsoleKey.V => () => { },
                _ => () => { Console.WriteLine("Felaktig inmantning"); Thread.Sleep(1000); }
            };
            Console.Clear();
            action();
            Console.ReadKey(true);
        }
    }
}
