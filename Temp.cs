using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace VäderUppgift
{
    internal class Temp
    {
        public static void TempRun()
        {
            string url = @"B:\Downloads,Pictures,Videos\Downloads\fixedData.txt";
            string htmlContent = File.ReadAllText(url);

            DateTime? previousDateTimeInne = null;
            DateTime? previousDateTimeUte = null;
            DateTime? currentDateTime = null;

            DateOnly? tempDateHolder = null;
            TimeOnly? tempTimeHolder = null;
            DateOnly? tempDateHolderOutside = null;
            TimeOnly? tempTimeHolderOutside = null;

            List<string> weatherLines = htmlContent
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            List<DateOnly> datumlist = new();
            List<TimeOnly> tidList = new();
            List<string> platsList = new();
            List<float> tempList = new();
            List<short> luftfuktighetList = new();

            Regex regexDatum = new Regex("\\d{4}-\\d{2}-\\d{2}");
            Regex regexTid = new Regex("\\d{2}:\\d{2}:\\d{2}");
            Regex regexPlats = new Regex("[A-Za-z]{3,4}");
            Regex regexTemp = new Regex("-?\\d{1,2}\\.\\d{0,1}");
            Regex regexFukt = new Regex(@"(?<=,)\d{1,2}$");

            float? compareValueInside = null;
            float? compareValueOutside = null;
            short? fuktInne = null;
            short? fuktUte = null;

            int RemoveCount = 0;

            for (int i = 0; i < weatherLines.Count; i++)
            {
                DateOnly? parsedDate = null;
                TimeOnly? parsedTime = null;
                string parsedPlace = null;
                float? parsedTemp = null;
                short? parsedFukt = null;

                bool removed = false;

                foreach (var regex in new[] { regexPlats, regexDatum, regexTid, regexTemp, regexFukt })
                {
                    Match match = regex.Match(weatherLines[i]);
                    if (!match.Success)
                        continue;

                    string value = match.Value;

                    if (regex == regexDatum)
                    {
                        if (DateOnly.TryParse(value, out DateOnly d))
                        {
                            parsedDate = d;

                            if (weatherLines[i].Contains("Ute"))
                                tempDateHolderOutside = d;
                            else
                                tempDateHolder = d;
                        }
                        continue;
                    }

                    if (regex == regexTid)
                    {
                        if (TimeOnly.TryParse(value, out TimeOnly t))
                        {
                            parsedTime = t;

                            if (weatherLines[i].Contains("Ute"))
                                tempTimeHolderOutside = t;
                            else
                                tempTimeHolder = t;
                        }
                        continue;
                    }

                    if (regex == regexPlats)
                    {
                        parsedPlace = value;
                        continue;
                    }

                    if (regex == regexTemp)
                    {
                        if (float.TryParse(value, NumberStyles.Float,
                            CultureInfo.InvariantCulture, out float tempValue))
                        {
                            parsedTemp = tempValue;

                            if (weatherLines[i].Contains("Ute"))
                            {
                                if (tempDateHolderOutside.HasValue &&
                                    tempTimeHolderOutside.HasValue)
                                {
                                    currentDateTime =
                                        tempDateHolderOutside.Value
                                        .ToDateTime(tempTimeHolderOutside.Value);
                                }
                            }
                            else
                            {
                                if (tempDateHolder.HasValue &&
                                    tempTimeHolder.HasValue)
                                {
                                    currentDateTime =
                                        tempDateHolder.Value
                                        .ToDateTime(tempTimeHolder.Value);
                                }
                            }

                            bool lessThanOneHourInne = false;
                            bool lessThanOneHourUte = false;

                            if (currentDateTime.HasValue)
                            {
                                if (previousDateTimeInne.HasValue)
                                {
                                    TimeSpan diffInne =
                                        currentDateTime.Value - previousDateTimeInne.Value;

                                    if (diffInne >= TimeSpan.Zero &&
                                        diffInne < TimeSpan.FromHours(1))
                                        lessThanOneHourInne = true;
                                }

                                if (previousDateTimeUte.HasValue)
                                {
                                    TimeSpan diffUte =
                                        currentDateTime.Value - previousDateTimeUte.Value;

                                    if (diffUte >= TimeSpan.Zero &&
                                        diffUte < TimeSpan.FromHours(1))
                                        lessThanOneHourUte = true;
                                }
                            }

                            if (weatherLines[i].Contains("Inne"))
                            {
                                if (compareValueInside != null &&
                                    Math.Abs(tempValue - compareValueInside.Value) > 7 &&
                                    lessThanOneHourInne)
                                {
                                    removed = true;
                                    break;
                                }

                                compareValueInside = tempValue;
                            }

                            if (weatherLines[i].Contains("Ute"))
                            {
                                if (compareValueOutside != null &&
                                    Math.Abs(tempValue - compareValueOutside.Value) > 7 &&
                                    lessThanOneHourUte)
                                {
                                    removed = true;
                                    break;
                                }

                                compareValueOutside = tempValue;
                            }
                        }
                        continue;
                    }

                    if (regex == regexFukt)
                    {
                        if (short.TryParse(value, out short fuktValue))
                        {
                            parsedFukt = fuktValue;

                            bool lessThanOneHourInne = false;
                            bool lessThanOneHourUte = false;

                            if (currentDateTime.HasValue)
                            {
                                if (previousDateTimeInne.HasValue)
                                {
                                    TimeSpan diffInne =
                                        currentDateTime.Value - previousDateTimeInne.Value;

                                    if (diffInne >= TimeSpan.Zero &&
                                        diffInne < TimeSpan.FromHours(1))
                                        lessThanOneHourInne = true;
                                }

                                if (previousDateTimeUte.HasValue)
                                {
                                    TimeSpan diffUte =
                                        currentDateTime.Value - previousDateTimeUte.Value;

                                    if (diffUte >= TimeSpan.Zero &&
                                        diffUte < TimeSpan.FromHours(1))
                                        lessThanOneHourUte = true;
                                }
                            }

                            if (weatherLines[i].Contains("Inne"))
                            {
                                if (fuktInne != null &&
                                    Math.Abs(fuktValue - fuktInne.Value) > 15 &&
                                    lessThanOneHourInne && fuktValue < 55)
                                {
                                    removed = true;
                                    break;
                                }

                                fuktInne = fuktValue;
                            }

                            if (weatherLines[i].Contains("Ute"))
                            {
                                if (fuktUte != null &&
                                    Math.Abs(fuktValue - fuktUte.Value) > 15 &&
                                    lessThanOneHourUte)
                                {
                                    removed = true;
                                    break;
                                }

                                fuktUte = fuktValue;
                            }
                        }
                        continue;
                    }
                }
                Console.WriteLine(weatherLines[i]);
                if (removed)
                {
                    RemoveCount++;
                    Console.WriteLine(" REMOVE " + weatherLines[i] + "        REMOVE COUNT " + RemoveCount);
                    Console.ReadLine();
                    weatherLines.RemoveAt(i);

                    i--;
                    continue;
                }

                if (parsedDate.HasValue &&
                    parsedTime.HasValue &&
                    parsedPlace != null &&
                    parsedTemp.HasValue &&
                    parsedFukt.HasValue)
                {
                    datumlist.Add(parsedDate.Value);
                    tidList.Add(parsedTime.Value);
                    platsList.Add(parsedPlace);
                    tempList.Add(parsedTemp.Value);
                    luftfuktighetList.Add(parsedFukt.Value);

                    if (weatherLines[i].Contains("Inne"))
                        previousDateTimeInne = currentDateTime;

                    if (weatherLines[i].Contains("Ute"))
                        previousDateTimeUte = currentDateTime;
                }
            }

            Console.WriteLine("REMOVED COUNT " + RemoveCount);

                File.WriteAllLines("cleanedDataWithIdAndSeperator.txt",
                Enumerable.Range(0, datumlist.Count).Select(i =>
                $"{i};{datumlist[i]};{tidList[i]:HH:mm:ss};" +
                $"{platsList[i]};" +
                $"{tempList[i].ToString(CultureInfo.InvariantCulture)};" +
                $"{luftfuktighetList[i]}"));

            File.WriteAllLines("datum.txt",
            datumlist.Select(d => d.ToString("yyyy-MM-dd")));

            File.WriteAllLines("tid.txt",
                tidList.Select(t => t.ToString("HH:mm:ss")));

            File.WriteAllLines("plats.txt",
                platsList);

            File.WriteAllLines("temp.txt",
                tempList.Select(t => t.ToString(CultureInfo.InvariantCulture)));

            File.WriteAllLines("fukt.txt",
                luftfuktighetList.Select(f => f.ToString()));



            Console.ReadLine();
        }
    }
}
