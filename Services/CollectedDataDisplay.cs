using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TempData_grupparbete.Data;
using TempData_grupparbete.Models;

namespace TempData_grupparbete.Services
{
    internal class CollectedDataDisplay
    {
        public static string header = $"{"Datum",-8} | {"Mätpknt.",-8} | {"InneTemp",-7} | {"RH",-4} | {"Mätpknt.",-8} | {"UteTemp",-6} | RH";

        public static async Task DisplayDailyTemp(List<WeatherData> data)
        {
            List<TempStatistics> inDoorTemp = await DataExtraction.AverageTempDay(data, "Inne");
            List<TempStatistics> outDoorTemp = await DataExtraction.AverageTempDay(data, "Ute");
            StringBuilder sbd = new StringBuilder();
            var recentTemp = inDoorTemp;
            sbd.AppendLine(header);
            sbd.AppendLine();
            Writer.Delete("dailytemp.txt");
            Writer.WriteRow("dailytemp.txt", header);
            Writer.WriteRow("dailytemp.txt", "");
            foreach (var day in recentTemp)
            {
                var recentMatch = outDoorTemp.First(o => o.Date == day.Date);
                
                sbd.AppendLine($"{day.Date:yy-MM-dd} | {day.Count,-8} | {day.Temp.ToString("0.0"),-8} | {day.Humidity.ToString("0.0")} | {recentMatch.Count,-8} | {recentMatch.Temp.ToString("0.0"),-7} | {recentMatch.Humidity.ToString("0.0")}");
                Writer.WriteRow("dailytemp.txt", $"{day.Date:yy-MM-dd} | {day.Count,-8} | {day.Temp.ToString("0.0"),-8} | {day.Humidity.ToString("0.0")} | {recentMatch.Count,-8} | {recentMatch.Temp.ToString("0.0"),-7} | {recentMatch.Humidity.ToString("0.0")}");
            }
            Console.WriteLine(sbd);
        }

        public static async Task DisplayDailyMonth(List<WeatherData> data)
        {
            List<TempStatistics> inDoorTemp = await DataExtraction.AverageTempMonth(data, "Inne");
            List<TempStatistics> outDoorTemp = await DataExtraction.AverageTempMonth(data, "Ute");
            StringBuilder sbm = new StringBuilder();
            var recentTemp = inDoorTemp;
            sbm.AppendLine(header);
            sbm.AppendLine();
            Writer.Delete("monthlytemp.txt");
            Writer.WriteRow("monthlytemp.txt", header);
            Writer.WriteRow("monthlytemp.txt", "");
            foreach (var month in recentTemp)
            {
                var recentMatch = outDoorTemp.First(o => o.Date == month.Date);
                
                sbm.AppendLine($"{month.Date:yy-MM-dd} | {month.Count,-8} | {month.Temp.ToString("0.0"),-8} | {month.Humidity.ToString("0.0")} | {recentMatch.Count,-8} | {recentMatch.Temp.ToString("0.0"),-7} | {recentMatch.Humidity.ToString("0.0")}");
                Writer.WriteRow("monthlytemp.txt", $"{month.Date:yy-MM-dd} | Mätpinkter {month.Count} | {month.Temp.ToString("0.0")} {month.Humidity.ToString("0.0")} | {recentMatch.Count} Mätpunkter {recentMatch.Temp.ToString("0.0")} {recentMatch.Humidity.ToString("0.0")}");

            }
            Console.WriteLine(sbm);
        }
    }
}
