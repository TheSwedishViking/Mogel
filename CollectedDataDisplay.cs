using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TempData_grupparbete.Data;
using TempData_grupparbete.Models;

namespace TempData_grupparbete
{
    internal class CollectedDataDisplay
    {
        public static async Task DisplayDailyTemp(List<WeatherData> data)
        {
            List<DailyTempStatistics> inDoorTemp = await DataExtraction.AverageTempDay(data, "Inne");
            List<DailyTempStatistics> outDoorTemp = await DataExtraction.AverageTempDay(data, "Ute");
            StringBuilder sb = new StringBuilder();
            var recentTemp = inDoorTemp;

            foreach (var day in recentTemp)
            {
                var recentMatch = outDoorTemp.FirstOrDefault(o => o.Date == day.Date);
                string outMatch = recentMatch != null ? $"{recentMatch.Temp:F1}" : "N/A";
                sb.AppendLine($"{day.Date:yy-MM-dd} {day.Temp:F1} | {outMatch}");
            }
            Console.WriteLine(sb);
        }
    }
}
