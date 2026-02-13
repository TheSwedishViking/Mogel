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
        public static async Task SearchInput(List<WeatherData> data)
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
                            await SearchMonth(data, year, month);
                        }
                        else
                        {
                            await SearchDate(data, year, month, day);
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
        public static async Task SearchDate(List<WeatherData> data, int year, int month, int day)
        {

            if (!DateTime.TryParse($"{year}-{month}-{day}", out DateTime searchDate))
            {
                Console.WriteLine("Ogiltigt datumformat.");
                return;
            }

            List<TempStatistics> inDoorTemp = await DataExtraction.AverageTempDay(data, "Inne");
            List<TempStatistics> outDoorTemp = await DataExtraction.AverageTempDay(data, "Ute");
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
                Console.ReadLine();
            return;        
        }
        public static async Task SearchMonth(List<WeatherData> data, int year, int month)
        {
            DateTime date = new DateTime(year, month, 00);

            List<TempStatistics> inDoorTemp = await DataExtraction.AverageTempDay(data, "Inne");
            List<TempStatistics> outDoorTemp = await DataExtraction.AverageTempDay(data, "Ute");
            StringBuilder sb = new StringBuilder();

            var resultIn = inDoorTemp
                .FirstOrDefault(t => t.Date.Date == date.Date);
            var resultOut = outDoorTemp
                .FirstOrDefault(t => t.Date.Date == date.Date);

            if (resultIn != null && resultOut != null)
            {
                sb.Append($"{resultIn.Date.ToString("yy-MM-dd")} | {resultIn.Temp.ToString("0.0")} | {resultOut.Temp.ToString("0.0")}");
            }
            else
            {
                Console.WriteLine("Hittar inte [Datum]");
            }
            Console.WriteLine(sb);
            Console.ReadLine();
            return;

        }
    }
}
