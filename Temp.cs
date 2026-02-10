using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Globalization;

namespace VäderUppgift
{
    internal class Temp
    {
        public static string path = "../../../File/";
        public static void TempRun()
        {
            string url = @"B:\Downloads,Pictures,Videos\Downloads\tempdata5-med fel\tempdata5-med fel.txt";
            string oscarUrl = @"C:\Users\oxlyt\Desktop\tempdata5-med fel.txt";

            //-\d{2}- extra för månad
            Regex entireRegexPattern = new Regex(@"^(?<year>[2][0][1][6]|[2][0][1][7])-(?<month>0[1-9]|1[0-2])-(?<day>[0-2][0-9]|3[0-1]) (?<hour>[0-1][0-9]|2[0-3]):(?<minute>[0-5][0-9]):(?<second>[0-5][0-9]),(?<location>[I][n][n][e]),(?<temp>[1-2][0-9]\.[0-9]),(?<rh>1[0-9]|[2-3][0-9]|[4-5][0-9])$|^(?<year>[2][0][1][6]|[2][0][1][7])-(?<month>[0-1][0-9]|1[1-2])-(?<day>[0-2][0-9]|3[0-1]) (?<hour>[0-1][0-9]|2[0-3]):(?<minute>[0-5][0-9]):(?<second>[0-5][0-9]),(?<location>[U][t][e]),(?<temp>([-2][0-9]\.[0-9])|[0-9]\.[0-9]|[1-3][0-9]\.[0-9]),(?<rh>2[0-9]|[1-9][0-9])$");
            Regex entireNewPattern = new Regex(@"(?<year>2016|2017)-(?<month>0[1-9]|1[0-2])-(?<day>(0[1-9]|[1-2][0-9])|3[0-1]) (?<hour>[0-1]\d|2[0-3]):(?<minute>[0-5]\d):(?<second>[0-5]\d),(?<location>Inne),(?<temp>[1-3]\d.\d),(?<rh>[1-8]\d)|(?<year>2016|2017)-(?<month>0[1-9]|1[0-2])-(?<day>(0[1-9]|[1-2][0-9])|3[0-1]) (?<hour>[0-1]\d|2[0-3]):(?<minute>[0-5]\d):(?<second>[0-5]\d),(?<location>Ute),(?<temp>-?[0-3]?\d.\d),(?<rh>100|\d?\d)");
            string weatherData = File.ReadAllText(oscarUrl);
            List<string> weatherLines = weatherData.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            //Datum
            List<DateOnly> datumlist = new();
            Regex regexDatum = new Regex("\\d{4}-\\d{2}-\\d{2}");

            //ExaktTid
            List<TimeOnly> tidList = new();
            Regex regexTid = new Regex("\\d{2}:\\d{2}:\\d{2}");

            //regexPlats
            List<string> platsList = new();
            Regex regexPlats = new Regex("[A-Za-z]{3,4}");

            //Temp
            List<float> tempList = new();
            Regex regexTemp = new Regex("\\d{1,2}\\.\\d{0,1}");

            //Fukt
            List<short> luftfuktighetList = new();
            Regex regexFukt = new Regex("(?<=[0-9]{1,2}\\.[0-9]{1}.)([0-9]{1,2})");

            Regex entireRowFormatDay = new Regex("(\\d{4}-\\d{2}-\\d{2}\\s\\d{1,2}:\\d{1,2}:\\d{1,2},\\D{1,4}[0-9]{1,2}.[0-9]{1,2},\\d[0-9]{1,2})");
            List<string> weatherOutside = new List<string>();

            Dictionary<DateOnly, List<EveryDataBracket>> dataDictionary = new Dictionary<DateOnly, List<EveryDataBracket>>();

            int countCorrect = 0;

            foreach (var weatline in weatherLines)
            {
                try
                {
                    //Passar det inte det exceptionellt långa formatet
                    if (!Regex.IsMatch(weatline, entireNewPattern.ToString()))
                    {
                        //Console.WriteLine($"Rad {weatline} passade inte in på formatet");
                        //Console.ReadKey();
                        //Lägg inte till, gå vidare i loopen
                        continue;
                    }

                    //Standard datum sträng därifrån
                    if (Regex.IsMatch(weatline, regexDatum.ToString()))
                    {
                        countCorrect++;
                        var newDate = regexDatum.Match(weatline);
                        DateOnly onlyDate = DateOnly.Parse(newDate.Value);
                        if (!dataDictionary.TryGetValue(onlyDate, out var testList))
                        {
                            testList = new List<EveryDataBracket>();
                            dataDictionary[onlyDate] = testList;
                        }
                        EveryDataBracket dataBracket = new EveryDataBracket();

                        if (Regex.IsMatch(weatline, regexTid.ToString()))
                        {
                            var time = regexTid.Match(weatline);
                            TimeOnly regTime = TimeOnly.Parse(time.Value);
                            dataBracket.Tid = regTime;
                        }
                        if (Regex.IsMatch(weatline, regexPlats.ToString()))
                        {
                            var place = regexPlats.Match(weatline);
                            bool isInsideBool = false;
                            if (place.Value.ToString() == "Inne")
                            {
                                isInsideBool = true;
                            }
                            dataBracket.IsInside = isInsideBool;
                        }
                        if (Regex.IsMatch(weatline, regexTemp.ToString()))
                        {
                            var tempValue = regexTemp.Match(weatline);
                            if (float.TryParse(tempValue.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out float floatValue))
                            {
                                dataBracket.Temp = floatValue;
                            }
                        }
                        //Fukt
                        if (Regex.IsMatch(weatline, regexFukt.ToString()))
                        {
                            var humidity = regexFukt.Match(weatline);
                            short humidityValue = short.Parse(humidity.ToString());
                            dataBracket.AirHumidity = humidityValue;
                        }

                        dataDictionary[onlyDate].Add(dataBracket);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            Console.WriteLine($"Registrerade {countCorrect} värden som borde vara korrekta enligt regex");
            Console.ReadKey();
            foreach (var day in dataDictionary.Keys)
            {
                Console.WriteLine($"Dag {day} hade: {dataDictionary[day].Count} registrerade poster (inkl tid, plats, temp, luftfuktighet) ");
                //varenda post i listan 
                foreach (var data in dataDictionary[day])
                {
                    //string placeString = data.IsInside ? "Inomhus" : "Utomhus";
                    //if (!data.IsInside)
                    //{
                    //    Console.WriteLine($"\tPlats: {placeString} Tid: {data.Tid}  Temperatur: {data.Temp} Luftfuktighet: {data.AirHumidity}");
                    //}
                }
                Console.WriteLine("=================");
                //poster på en dag
                int postsPerDay = dataDictionary[day].Count;
                float avgTempDay = dataDictionary[day].Sum(d => d.Temp);
                avgTempDay /= dataDictionary[day].Count();

                int countPostsInside = dataDictionary[day].Where(d=>d.IsInside).Count();
                int countPostsOutSide = dataDictionary[day].Where(d => !d.IsInside).Count();

                float avgTempInside = dataDictionary[day].Where(d => d.IsInside).Sum(d=>d.Temp);
                avgTempInside /= countPostsInside;

                float avgTempOutside = dataDictionary[day].Where(d => !d.IsInside).Sum(d => d.Temp);
                avgTempOutside /= countPostsOutSide;
                Console.WriteLine($"Medevärde inom och utomhus för {day} {avgTempDay.ToString("0.0")}, med {postsPerDay} poster den dagen");
                Console.WriteLine($"Inomhusvärdet var {avgTempInside.ToString("0.0")} baserat på {countPostsInside} poster");
                Console.WriteLine($"Utomhusvärdet var {avgTempOutside.ToString("0.0")} baserat på {countPostsOutSide} poster");
                Console.ReadKey();
            }

        }
    }
}
