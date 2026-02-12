using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TempData_grupparbete.Data;
using TempData_grupparbete.Models;

namespace TempData_grupparbete.Services
{
    internal class Search
    {
        public static async Task SearchDate(List<WeatherData> data)
        {
            int year = 2016;
            int month = 11;
            int day = 06;
             DateTime date = new DateTime(year, month, day);
            
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
        
        }
    }
}
