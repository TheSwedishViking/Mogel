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
            Regex entireRegexPattern = new Regex(@"^(?<year>[2][0][1][6]|[2][0][1][7])-(?<month>[0-1][0-9]|1[1-2])-(?<day>[0-2][0-9]|3[0-1]) (?<hour>[0-1][0-9]|2[0-3]):(?<minute>[0-5][0-9]):(?<second>[0-5][0-9]),(?<location>[I][n][n][e]),(?<temp>[1-2][0-9]\.[0-9]),(?<rh>1[0-9]|[2-3][0-9]|[4-5][0-9])$|^(?<year>[2][0][1][6]|[2][0][1][7])-(?<month>[0-1][0-9]|1[1-2])-(?<day>[0-2][0-9]|3[0-1]) (?<hour>[0-1][0-9]|2[0-3]):(?<minute>[0-5][0-9]):(?<second>[0-5][0-9]),(?<location>[U][t][e]),(?<temp>([-2][0-9]\.[0-9])|[0-9]\.[0-9]|[1-3][0-9]\.[0-9]),(?<rh>2[0-9]|[1-9][0-9])$");

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

            Dictionary<DateOnly, List<EveryDataBracket>> dataDictionary = new Dictionary<DateOnly,List< EveryDataBracket>>();

            foreach (var weatline in weatherLines)
            {
                try
                {
                    if(Regex.IsMatch(weatline, entireRegexPattern.ToString()))
                    {

                    }
                    if (Regex.IsMatch(weatline, regexDatum.ToString()))
                    {
                        var newDate = regexDatum.Match(weatline);
                        DateOnly onlyDate = DateOnly.Parse(newDate.Value);
                        //Console.WriteLine("Hittade datum: " + newDate.Value);
                        if(!dataDictionary.TryGetValue(onlyDate, out var testList))
                        {
                            testList = new List<EveryDataBracket>();
                            dataDictionary[onlyDate] = testList;
                        }
                        EveryDataBracket dataBracket = new EveryDataBracket();

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
            foreach(var day in dataDictionary.Keys)
            {
                Console.WriteLine($"Dag {day} count {dataDictionary[day].Count} hade fuktigheter registerade av ");
                foreach(var humidity  in dataDictionary[day])
                {
                    Console.WriteLine($"\tLuftfuktighet: {humidity.AirHumidity}");
                }
            }

            }
        }
    }
