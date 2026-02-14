using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
﻿using System.Text;
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

        public static string moldHeader = $"{"Datum",-5} | {"Mätpknt.",-8} | {"UteTemp",-8} | {"RH%",-5}| Mögel%";

        public static string indoorHeader = $"{"Datum",-8} | {"Mätpknt.",-8} | {"InneTemp",-7} | {"RH",-4} | Mögel% | {"Mätpknt.",-8} | {"UteTemp",-8} | {"RH%",-5}| Mögel%";
        public static string outdoorHeader = $"{"Datum",-8} | {"Mätpknt.",-8} | {"UteTemp",-8} | {"RH%",-5}| Mögel% | {"Mätpknt.",-8} | {"InneTemp",-6} | {"RH",-5}| Mögel%";

        public static string indoorHeaderMonth = $"{"Datum",-5} | {"Mätpknt.",-8} | {"InneTemp",-7} | {"RH",-4} | Mögel% | {"Mätpknt.",-8} | {"UteTemp",-8} | {"RH%",-5}| Mögel%";
        public static string outdoorHeaderMonth = $"{"Datum",-5} | {"Mätpknt.",-8} | {"UteTemp",-8} | {"RH%",-5}| Mögel% | {"Mätpknt.",-8} | {"InneTemp",-6} | {"RH",-5}| Mögel%";
        public static string header;
        public static bool YesOrNo(string field)
        {
            while (true)
            {
                Console.WriteLine(field + " J/N");

                var key = Console.ReadKey(true);
                
                if (key.Key == ConsoleKey.J)
                {
                    return true;
                }
                else if (key.Key == ConsoleKey.N)
                {
                    return false;
                }
                else
                {
                    Console.WriteLine("Felaktig inmatning");
                    Thread.Sleep(500);
                    Console.Clear();
                }
            }
        }
        public static async Task DisplayDailyTempWithSorting(List<WeatherData> data)
        {
            List<TempStatistics> inDoorTemp = DataExtraction.AverageTempDay(data, "Inne");
            List<TempStatistics> outDoorTemp = DataExtraction.AverageTempDay(data, "Ute");

            while (true)
            {
                bool IsOutsideDataReqestested = YesOrNo("Sortera enligt utomhus?");
                var recentTemp = outDoorTemp;
                if (IsOutsideDataReqestested)
                {
                    header = outdoorHeader;
                     recentTemp = outDoorTemp;
                }
                else if (!IsOutsideDataReqestested)
                {
                    header = indoorHeader;
                     recentTemp = inDoorTemp;
                }
                Console.WriteLine($"[M] för att sortera enligt mögel\n[T] för att sortera enligt temperatur\n[H] för luftfuktighet\n[D] för datum");
                ConsoleKey key = Console.ReadKey(true).Key;


                switch (key)
                {
                    case ConsoleKey.M:
                        bool answer = YesOrNo("Stigande?");
                        if (answer)
                        {
                            recentTemp = recentTemp.OrderBy(d => d.Mold).ToList();
                        }
                        else if (!answer)
                        {
                           recentTemp = recentTemp.OrderByDescending(d => d.Mold).ToList();
                        }
                        break;

                    case ConsoleKey.T:
                        bool temp = YesOrNo("Stigande temperatur?");
                        if (temp)
                        {
                            recentTemp = recentTemp.OrderBy(d => d.Temp).ToList();
                        }
                        else
                        {
                            recentTemp = recentTemp.OrderByDescending(d => d.Temp).ToList();
                        }
                            break;
                    case ConsoleKey.H:
                        bool humidity = YesOrNo("Stigande luftfuktighet?");
                        if (humidity)
                        {
                            recentTemp = recentTemp.OrderBy(d => d.Humidity).ToList();
                        }
                        else
                        {
                            recentTemp = recentTemp.OrderByDescending(d => d.Humidity).ToList();
                        }
                            break;
                    case ConsoleKey.D:
                        bool date = YesOrNo("Senaste datum?");
                        if (date)
                        {
                            recentTemp = recentTemp.OrderBy(d=>d.Date).ToList();
                        }
                        else
                        {
                            recentTemp = recentTemp.OrderByDescending(d => d.Date).ToList();
                        }
                            
                        break;
                }
                StringBuilder sbd = new StringBuilder();

         
                sbd.AppendLine(header);
                sbd.AppendLine();
                await Writer.Delete("dailytemp.txt");
                await Writer.WriteRow("dailytemp.txt", header);
                await Writer.WriteRow("dailytemp.txt", "");
                foreach (var day in recentTemp)
                {
                    var recentMatch = inDoorTemp.FirstOrDefault(x => x.Date == day.Date);
                    if (IsOutsideDataReqestested)
                    {
                        recentMatch = inDoorTemp.FirstOrDefault(x => x.Date == day.Date);
                    }
                    else
                    {
                        recentMatch = outDoorTemp.FirstOrDefault(x => x.Date == day.Date);
                    }
                    
                    sbd.AppendLine($"{day.Date:yy-MM-dd} | {day.Count,-8} | {day.Temp.ToString("0.0"),-8} | {day.Humidity.ToString("0.0")} | {day.Mold.ToString("0"),-6} | {recentMatch.Count,-8} | {recentMatch.Temp.ToString("0.0"),-8} | {recentMatch.Humidity.ToString("0.0")} | {recentMatch.Mold.ToString("0")}");
                    await Writer.WriteRow("dailytemp.txt", $"{day.Date:yy-MM-dd} | {day.Count,-8} | {day.Temp.ToString("0.0"),-8} | {day.Humidity.ToString("0.0")} | {day.Mold.ToString("0"),-6} | {recentMatch.Count,-8} | {recentMatch.Temp.ToString("0.0"),-8} | {recentMatch.Humidity.ToString("0.0")} | {recentMatch.Mold.ToString("0")}");


                    //sbd.AppendLine($"{day.Date:yy-MM-dd} | {day.Count,-8} | {day.Temp.ToString("0.0"),-8} | {day.Humidity.ToString("0.0")} | {day.Mold.ToString("0"),-6} | {recentMatch.Count,-8} | {recentMatch.Temp.ToString("0.0"),-8} | {recentMatch.Humidity.ToString("0.0")} | {recentMatch.Mold.ToString("0")}");
                    //await Writer.WriteRow("dailytemp.txt", $"{day.Date:yy-MM-dd} | {day.Count,-8} | {day.Temp.ToString("0.0"),-8} | {day.Humidity.ToString("0.0")}  | {day.Mold.ToString("0")} | {recentMatch.Count,-8} | {recentMatch.Temp.ToString("0.0"),-8} | {recentMatch.Humidity.ToString("0.0")} | {recentMatch.Mold.ToString("0")}");
                }
                Console.WriteLine(sbd);
                Console.ReadLine();
                return;
            }
        }
        //public static async Task DisplayDailyTemp(List<WeatherData> data)
        //{
        //    List<TempStatistics> inDoorTemp = DataExtraction.AverageTempDay(data, "Inne");
        //    List<TempStatistics> outDoorTemp = DataExtraction.AverageTempDay(data, "Ute");
        //    StringBuilder sbd = new StringBuilder();
        //    var recentTemp = inDoorTemp;
        //    sbd.AppendLine(header);
        //    sbd.AppendLine();
        //    await Writer.Delete("dailytemp.txt");
        //    await Writer.WriteRow("dailytemp.txt", header);
        //    await Writer.WriteRow("dailytemp.txt", "");
        //    foreach (var day in recentTemp)
        //    {
        //        var recentMatch = outDoorTemp.First(o => o.Date == day.Date);

        //        sbd.AppendLine($"{day.Date:yy-MM-dd} | {day.Count,-8} | {day.Temp.ToString("0.0"),-8} | {day.Humidity.ToString("0.0")} | {recentMatch.Count,-8} | {recentMatch.Temp.ToString("0.0"),-7} | {recentMatch.Humidity.ToString("0.0")} | {recentMatch.Mold.ToString("0")}");
        //        await Writer.WriteRow("dailytemp.txt", $"{day.Date:yy-MM-dd} | {day.Count,-8} | {day.Temp.ToString("0.0"),-8} | {day.Humidity.ToString("0.0")} | {recentMatch.Count,-8} | {recentMatch.Temp.ToString("0.0"),-7} | {recentMatch.Humidity.ToString("0.0")} | {recentMatch.Mold.ToString("0")}");
        //    }
        //    Console.WriteLine(sbd);
        //}
        public static async Task DisplayDailyMonthSorted(List<WeatherData> data)
        {
            List<TempStatistics> inDoorTemp = DataExtraction.AverageTempMonth(data, "Inne");
            List<TempStatistics> outDoorTemp = DataExtraction.AverageTempMonth(data, "Ute");

            while (true)
            {
                bool IsOutsideDataReqestested = YesOrNo("Sortera enligt utomhus?");
                var recentTemp = outDoorTemp;
                if (IsOutsideDataReqestested)
                {
                    header = outdoorHeaderMonth;
                    recentTemp = outDoorTemp;
                }
                else if (!IsOutsideDataReqestested)
                {
                    header = indoorHeaderMonth;
                    recentTemp = inDoorTemp;
                }
                Console.WriteLine($"[M] för att sortera enligt mögel\n[T] för att sortera enligt temperatur\n[H] för luftfuktighet\n[D] för datum");
                ConsoleKey key = Console.ReadKey(true).Key;


                switch (key)
                {
                    case ConsoleKey.M:
                        bool answer = YesOrNo("Stigande?");
                        if (answer)
                        {
                            recentTemp = recentTemp.OrderBy(d => d.Mold).ToList();
                        }
                        else if (!answer)
                        {
                            recentTemp = recentTemp.OrderByDescending(d => d.Mold).ToList();
                        }
                        break;

                    case ConsoleKey.T:
                        bool temp = YesOrNo("Stigande temperatur?");
                        if (temp)
                        {
                            recentTemp = recentTemp.OrderBy(d => d.Temp).ToList();
                        }
                        else
                        {
                            recentTemp = recentTemp.OrderByDescending(d => d.Temp).ToList();
                        }
                        break;
                    case ConsoleKey.H:
                        bool humidity = YesOrNo("Stigande luftfuktighet?");
                        if (humidity)
                        {
                            recentTemp = recentTemp.OrderBy(d => d.Humidity).ToList();
                        }
                        else
                        {
                            recentTemp = recentTemp.OrderByDescending(d => d.Humidity).ToList();
                        }
                        break;
                    case ConsoleKey.D:
                        bool date = YesOrNo("Senaste datum?");
                        if (date)
                        {
                            recentTemp = recentTemp.OrderBy(d => d.Date).ToList();
                        }
                        else
                        {
                            recentTemp = recentTemp.OrderByDescending(d => d.Date).ToList();
                        }

                        break;
                }


                StringBuilder sbm = new StringBuilder();
                sbm.AppendLine(header);
                sbm.AppendLine();
                await Writer.Delete("monthlytemp.txt");
                await Writer.WriteRow("monthlytemp.txt", header);
                await Writer.WriteRow("monthlytemp.txt", "");


                foreach (var month in recentTemp)
                {
                    var recentMatch = inDoorTemp.FirstOrDefault(x => x.Date == month.Date);
                    if (IsOutsideDataReqestested)
                    {
                        recentMatch = inDoorTemp.FirstOrDefault(x => x.Date == month.Date);
                    }
                    else
                    {
                        recentMatch = outDoorTemp.FirstOrDefault(x => x.Date == month.Date);
                    }

                    sbm.AppendLine($"{month.Date:yy-MM} | {month.Count,-8} | {month.Temp.ToString("0.0"),-8} | {month.Humidity.ToString("0.0")} | {month.Mold.ToString("0"), -6} | {recentMatch.Count,-8} | {recentMatch.Temp.ToString("0.0"),-8} | {recentMatch.Humidity.ToString("0.0")} | {recentMatch.Mold.ToString("0")}");
                    await Writer.WriteRow("monthlytemp.txt",$"{month.Date:yy-MM} | {month.Count,-8} | {month.Temp.ToString("0.0"),-8} | {month.Humidity.ToString("0.0")} | {month.Mold.ToString("0"),-6} | {recentMatch.Count,-8} | {recentMatch.Temp.ToString("0.0"),-8} | {recentMatch.Humidity.ToString("0.0")} | {recentMatch.Mold.ToString("0")}");
                    //await Writer.WriteRow("monthlytemp.txt", $"{month.Date:yy-MM-dd} | {month.Count,-8} | {month.Temp.ToString("0.0"),-8} | {month.Humidity.ToString("0.0"), -4} | {recentMatch.Count,-8} | {recentMatch.Temp.ToString("0.0"),-7} | {recentMatch.Humidity.ToString("0.0")} | {recentMatch.Mold.ToString("0")}");

                }
                Console.WriteLine(sbm);
                Console.ReadLine();
                return;
            }
        }
        public static async Task WriteMold(List<WeatherData> data)
        {
            List<TempStatistics> inDoorTemp = DataExtraction.AverageTempMonth(data, "Inne");
            List<TempStatistics> outDoorTemp = DataExtraction.AverageTempMonth(data, "Ute");

            var recentTemp = outDoorTemp;

            recentTemp = recentTemp.OrderBy(d => d.Mold).ToList();

            await Writer.WriteRow("moldinfo.txt", moldHeader);
            await Writer.WriteRow("moldinfo.txt", "");


            foreach (var month in recentTemp)
            {

                await Writer.WriteRow("moldinfo.txt", $"{month.Date:yy-MM} | {month.Count,-8} | {month.Temp.ToString("0.0"),-8} | {month.Humidity.ToString("0.0")} | {month.Mold.ToString("0"),-6}");
                //await Writer.WriteRow("monthlytemp.txt", $"{month.Date:yy-MM-dd} | {month.Count,-8} | {month.Temp.ToString("0.0"),-8} | {month.Humidity.ToString("0.0"), -4} | {recentMatch.Count,-8} | {recentMatch.Temp.ToString("0.0"),-7} | {recentMatch.Humidity.ToString("0.0")} | {recentMatch.Mold.ToString("0")}");

            }
            await Writer.WriteRow("moldinfo.txt", "");
            await Writer.WriteRow("moldinfo.txt", "Mögel risken beräknas med;");
            await Writer.WriteRow("moldinfo.txt", "En risk punkt (kritisk Lf) från 100 - angiven temperatur. Under 0 och över 50 är det ingen mögel chans. \n");
            await Writer.WriteRow("moldinfo.txt", "Risk punkten är som lägst 80, det är lägsta punkten för att mögel ska börja växa inom 8 veckor. \n");
            await Writer.WriteRow("moldinfo.txt", "Vi har ett span inom 100 - 80 (då en mögel intervall av 20), där 100 är maximal mögel tillväxt och 80 lägsta hotfulla tillväxt.\n");
            await Writer.WriteRow("moldinfo.txt", "Överskotten från bas är luftfuktighet - minsta mögelpunkt (80).\n");
            await Writer.WriteRow("moldinfo.txt", "Den total mögelrisken blir då;\n");
            await Writer.WriteRow("moldinfo.txt", "Mögelrisk = (Överskott från bas / Mögel spann) * 100\n");
            await Writer.WriteRow("moldinfo.txt", "Om kritiska luftikghetensnivån är >80 (K.Lf = 100 - temperatur)\n");
            await Writer.WriteRow("moldinfo.txt", "Mögelrisk = ((Lf-80)/20)*100\n");

        }
        //public static async Task DisplayDailyMonth(List<WeatherData> data)
        //{
        //    List<TempStatistics> inDoorTemp = DataExtraction.AverageTempMonth(data, "Inne");
        //    List<TempStatistics> outDoorTemp = DataExtraction.AverageTempMonth(data, "Ute");
        //    StringBuilder sbm = new StringBuilder();
        //    var recentTemp = inDoorTemp;
        //    sbm.AppendLine(header);
        //    sbm.AppendLine();
        //    await Writer.Delete("monthlytemp.txt");
        //    await Writer.WriteRow("monthlytemp.txt", header);
        //    await Writer.WriteRow("monthlytemp.txt", "");
        //    foreach (var month in recentTemp)
        //    {
        //        var recentMatch = outDoorTemp.First(o => o.Date == month.Date);

        //        sbm.AppendLine($"{month.Date:yy-MM} | {month.Count,-8} | {month.Temp.ToString("0.0"),-8} | {month.Humidity.ToString("0.0")} | {recentMatch.Count,-8} | {recentMatch.Temp.ToString("0.0"),-7} | {recentMatch.Humidity.ToString("0.0")}| {recentMatch.Mold.ToString("0")}");
        //        await Writer.WriteRow("monthlytemp.txt", $"{month.Date:yy-MM-dd} | {month.Count,-8} | {month.Temp.ToString("0.0"),-8} | {month.Humidity.ToString("0.0")} | {recentMatch.Count,-8} | {recentMatch.Temp.ToString("0.0"),-7} | {recentMatch.Humidity.ToString("0.0")} | {recentMatch.Mold.ToString("0")}");

        //    }
        //    Console.WriteLine(sbm);
        //}
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
            Console.ReadLine();
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
            Console.ReadLine();
        }


    }
}
