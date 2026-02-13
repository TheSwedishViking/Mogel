using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using TempData_grupparbete.Data;
using TempData_grupparbete.Models;

namespace TempData_grupparbete.Services
{
    internal class Search
    {
        public static void SearchInput(List<WeatherData> data)
        {
                int year;
                int month;
                int day = 0;

                Console.WriteLine("Ange år");
                if (int.TryParse(Console.ReadLine(), out year))
                {
                    Console.WriteLine("Ange månad");
                    if (int.TryParse(Console.ReadLine(), out month))
                    {
                        Console.WriteLine("Ange dag eller [0] för att söka på månad");
                        if (int.TryParse(Console.ReadLine(), out day))
                        {
                        if (day == 0)
                        {
                            SearchMonth(data, year, month);
                        }
                        else
                        {
                            SearchDate(data, year, month, day);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Felaktig inmatning");
                        return;
                    }
                }
                    else
                    {
                        Console.WriteLine("Felaktig inmatning");
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("Felaktig inmatning");
                    return;
                }
        }
        public static void SearchDate(List<WeatherData> data, int year, int month, int day)
        {

            if (!DateTime.TryParse($"{year}-{month}-{day}", out DateTime searchDate))
            {
                Console.WriteLine("Ogiltigt datumformat.");
                
            }

            List<TempStatistics> inDoorTemp = DataExtraction.AverageTempDay(data, "Inne");
            List<TempStatistics> outDoorTemp = DataExtraction.AverageTempDay(data, "Ute");
            StringBuilder sb = new StringBuilder();

            var resultIn = inDoorTemp
                .FirstOrDefault(t => t.Date.Date == searchDate);
            var resultOut = outDoorTemp
                .FirstOrDefault(t => t.Date.Date == searchDate);

            if (resultIn != null && resultOut != null)
            {
                sb.Append($"{resultIn.Date.ToString("yy-MM-dd")} | {resultIn.Temp.ToString("0.0")} | {resultOut.Temp.ToString("0.0")}");
            }
            else
            {
                Console.WriteLine("Hittar inte [Datum]");
            }
                Console.WriteLine(sb);
                
                 
        }
        public static void SearchMonth(List<WeatherData> data, int year, int month)
        {
            if (!DateTime.TryParse($"{year}-{month}-{01}", out DateTime searchDate))
            {
                Console.WriteLine("Ogiltigt datumformat.");

            }

            List<TempStatistics> inDoorTemp = DataExtraction.AverageTempDay(data, "Inne");
            List<TempStatistics> outDoorTemp = DataExtraction.AverageTempDay(data, "Ute");
            StringBuilder sb = new StringBuilder();

            var resultIn = inDoorTemp
                .FirstOrDefault(t => t.Date.Date.Month == searchDate.Date.Month);
            var resultOut = outDoorTemp
                .FirstOrDefault(t => t.Date.Date.Month == searchDate.Date.Month);

            if (resultIn != null && resultOut != null)
            {
                sb.Append($"{resultIn.Date.ToString("yy-MM-dd")} | {resultIn.Temp.ToString("0.0")} | {resultOut.Temp.ToString("0.0")}");
            }
            else
            {
                Console.WriteLine("Hittar inte [Datum]");
            }
            Console.WriteLine(sb);
            return;

        }
    }
}
