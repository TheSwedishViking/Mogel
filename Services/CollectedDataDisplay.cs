using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using TempData_grupparbete.Data;
using TempData_grupparbete.Models;

namespace TempData_grupparbete.Services
{

    internal class CollectedDataDisplay
    {
        public static StringBuilder sbBadData = new StringBuilder();
        public static List<(int, string)> badData = new List<(int, string)>();
        public static int badDataCount = 0;
        public static int badDataRow = 0;
        public static string fullBadData;
        public static string header = $"{"Datum",-8} | {"Mätpknt.",-8} | {"InneTemp",-7} | {"RH",-4} | {"Mätpknt.",-8} | {"UteTemp",-6} | {"RH%",-5}| Mögel%";

        public static async Task DisplayDailyTemp(List<WeatherData> data)
        {
            List<TempStatistics> inDoorTemp = DataExtraction.AverageTempDay(data, "Inne");
            List<TempStatistics> outDoorTemp = DataExtraction.AverageTempDay(data, "Ute");
            StringBuilder sbd = new StringBuilder();
            var recentTemp = inDoorTemp;
            sbd.AppendLine(header);
            sbd.AppendLine();
            await Writer.Delete("dailytemp.txt");
            await Writer.WriteRow("dailytemp.txt", header);
            await Writer.WriteRow("dailytemp.txt", "");
            foreach (var day in recentTemp)
            {
                var recentMatch = outDoorTemp.First(o => o.Date == day.Date);

                sbd.AppendLine($"{day.Date:yy-MM-dd} | {day.Count,-8} | {day.Temp.ToString("0.0"),-8} | {day.Humidity.ToString("0.0")} | {recentMatch.Count,-8} | {recentMatch.Temp.ToString("0.0"),-7} | {recentMatch.Humidity.ToString("0.0")} | {recentMatch.Mold.ToString("0")}");
                await Writer.WriteRow("dailytemp.txt", $"{day.Date:yy-MM-dd} | {day.Count,-8} | {day.Temp.ToString("0.0"),-8} | {day.Humidity.ToString("0.0")} | {recentMatch.Count,-8} | {recentMatch.Temp.ToString("0.0"),-7} | {recentMatch.Humidity.ToString("0.0")} | {recentMatch.Mold.ToString("0")}");
            }
            Console.WriteLine(sbd);
        }
        public static async Task DisplayDailyMonth(List<WeatherData> data)
        {
            List<TempStatistics> inDoorTemp = DataExtraction.AverageTempMonth(data, "Inne");
            List<TempStatistics> outDoorTemp = DataExtraction.AverageTempMonth(data, "Ute");
            StringBuilder sbm = new StringBuilder();
            var recentTemp = inDoorTemp;
            sbm.AppendLine(header);
            sbm.AppendLine();
            await Writer.Delete("monthlytemp.txt");
            await Writer.WriteRow("monthlytemp.txt", header);
            await Writer.WriteRow("monthlytemp.txt", "");
            foreach (var month in recentTemp)
            {
                var recentMatch = outDoorTemp.First(o => o.Date == month.Date);

                sbm.AppendLine($"{month.Date:yy-MM-dd} | {month.Count,-8} | {month.Temp.ToString("0.0"),-8} | {month.Humidity.ToString("0.0")} | {recentMatch.Count,-8} | {recentMatch.Temp.ToString("0.0"),-7} | {recentMatch.Humidity.ToString("0.0")}| {recentMatch.Mold.ToString("0")}");
                await Writer.WriteRow("monthlytemp.txt", $"{month.Date:yy-MM-dd} | {month.Count,-8} | {month.Temp.ToString("0.0"),-8} | {month.Humidity.ToString("0.0")} | {recentMatch.Count,-8} | {recentMatch.Temp.ToString("0.0"),-7} | {recentMatch.Humidity.ToString("0.0")} | {recentMatch.Mold.ToString("0")}");

            }
            Console.WriteLine(sbm);
        }
        public static async Task BadData(string line)
        {
            badDataCount++;
            badDataRow = ReadFile.rowCount + badDataCount;
            fullBadData = $"{badDataRow,-10} | {line}";
            sbBadData.AppendLine(fullBadData);
            badData.Add((badDataRow, line));
            await Writer.WriteRow("baddata.txt", line);
        }
        public static void DisplayBadData()
        {
            Console.WriteLine(sbBadData);
        }
        internal static async Task SearchForMetrologicalSeasonStart(List<WeatherData> weatherData, int autumnTemp, int countStreak, string season)
        {
            //Hitta höst eller vinter start
            //5 dagar i rad med medelvärde utomhus under 10 celcius
            bool fiveDayStreak = false;
            List<TempStatistics> outDoorTemps = DataExtraction.AverageTempDay(weatherData, "Ute");
            List<TempStatistics> colderDays = new List<TempStatistics>();
            TempStatistics seasonStart = new TempStatistics();
            int fallDayCounter = 0;
            //Loopa
            while (!fiveDayStreak && countStreak > 0)
            {
                fallDayCounter = 0;
                colderDays.Clear();
                //I alla dagar
                foreach (TempStatistics t in outDoorTemps)
                {
                    //Temperatur som är under det som behövs - bra, räkna upp och lägg till
                    if (t.Temp < autumnTemp)
                    {
                        fallDayCounter++;
                        colderDays.Add(t);
                    }
                    //Bryt streak, inte dagar i följd här
                    else
                    {
                        fallDayCounter = 0;
                        colderDays.Clear();
                    }
                    //Har vi så många dagar som behövs för countern
                    if (fallDayCounter == countStreak)
                    {
                        //Är vi färdiga, gå vidare
                        fiveDayStreak = true;
                        break;
                    }
                }
                if (!fiveDayStreak)
                {
                    countStreak--;
                }

            }
            if (countStreak != 5)
            {
                Console.WriteLine($"Hittade inte 5 dagar i rad för säsong {season}, vi hade istället {countStreak} i följd av temperatur under {autumnTemp}°C");
            }
            else
            {
                Console.WriteLine($"Det fanns dagar i rad under {autumnTemp}°C för att metrologisk {season} ska starta!");
            }
            seasonStart = colderDays.Last();
            Console.WriteLine($"{season} startar den {seasonStart.Date.ToString("yyyy-MM-dd")}");
            Console.WriteLine($"{countStreak} dagar i rad från dessa dagar;");
            foreach (var day in colderDays)
            {
                Console.WriteLine($"{day.Date.ToString("yyyy-MM-dd")} {day.Temp.ToString("0.0")}°C");
            }
        }
    }
}
