using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Globalization;
using System.Runtime.Serialization;
using TempData_grupparbete.Models;


namespace TempData_grupparbete
{
    internal class ReadFile
    {
        
        public static string path = "../../../File/";
        public static void ReadAll()
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sbBadData = new StringBuilder();
            Stopwatch sw = Stopwatch.StartNew();
            List<WeatherData> weatherData = new List<WeatherData>();
            List<(int, string)> badData = new List<(int, string)>();
            Regex inneTemp = new Regex(@"^(?<year>201[6-7])-(?<month>0[1-9]|1[0-2])-(?<day>0[1-9]|[1-2]\d|3[01]) (?<hour>[0-1]\d|2[0-3]):(?<minute>[0-5]\d):(?<second>[0-5]\d),(?<location>Inne),(?<temp>[1-3]\d.\d),(?<rh>[1-8]\d)$",RegexOptions.Compiled);
            Regex uteTemp = new Regex(@"^(?<year>201[6-7])-(?<month>0[1-9]|1[0-2])-(?<day>0[1-9]|[1-2]\d|3[01]) (?<hour>[0-1]\d|2[0-3]):(?<minute>[0-5]\d):(?<second>[0-5]\d),(?<location>Ute),(?<temp>-?[0-3]?\d.\d),(?<rh>100|\d?\d)$",RegexOptions.Compiled);
            int badDataCount = 0;
            int badDataRow = 0;
            string fullBadData;
            try
            {
                using (StreamReader reader = new StreamReader(path + "tempdata.txt"))
                {
                    string line = reader.ReadLine();
                    int rowCount = 0;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrEmpty(line)) continue;
                        Match match = line.Contains("Inne") ? inneTemp.Match(line) :
                                      line.Contains("Ute") ? uteTemp.Match(line) :
                                      Match.Empty;
                        
                        if (match.Success)
                        {
                            WeatherData data = new WeatherData
                            {
                                DateTime = new DateTime(
                                int.Parse(match.Groups["year"].Value),
                                int.Parse(match.Groups["month"].Value),
                                int.Parse(match.Groups["day"].Value),
                                int.Parse(match.Groups["hour"].Value),
                                int.Parse(match.Groups["minute"].Value),
                                int.Parse(match.Groups["second"].Value)),
                                Location = match.Groups["location"].Value,
                                Temp = double.Parse(match.Groups["temp"].Value,CultureInfo.InvariantCulture),
                                Humidity = int.Parse(match.Groups["rh"].Value)
                            };
                            string time = match.Groups["year"].Value + "-" + match.Groups["month"].Value + "-" + match.Groups["day"].Value + " " + match.Groups["hour"].Value + ":" + match.Groups["minute"].Value + ":" + match.Groups["second"].Value + "," + match.Groups["location"].Value + "," + match.Groups["temp"].Value + "," + match.Groups["rh"].Value;
                            string fullSring = rowCount + " " + time;
                            sb.AppendLine(fullSring);
                            rowCount++;
                            weatherData.Add(data);
                        }
                        else
                        {
                            badDataCount++;
                            badDataRow = rowCount + badDataCount;
                            fullBadData = $"{badDataRow,-10} | {line}";
                            sbBadData.AppendLine(fullBadData);
                            badData.Add((badDataRow,line));
                        }
                    }
                    Console.WriteLine(sb.ToString());
                    sw.Stop();
                    Console.WriteLine(sw.ElapsedMilliseconds+ "ms " + rowCount + " rader");
                    Console.WriteLine("---Felaktig data---");
                    Console.WriteLine();
                    Console.WriteLine($"{"Rad",-10} | Data");
                    Console.WriteLine(sbBadData);
                    Console.WriteLine(badDataCount);
                    CollectedDataDisplay.DisplayDailyTemp(weatherData);
                    Console.ReadKey();
                }
            }
            catch
            {
                Console.WriteLine("Filen finns inte");
            }

        }
    }
}
