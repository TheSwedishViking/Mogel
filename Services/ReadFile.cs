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
using TempData_grupparbete.Services;


namespace TempData_grupparbete.Services
{
    internal class ReadFile
    {
        public static int rowCount = 0;
        public static string path = "../../../File/";
        //public static StringBuilder sbBadData = new StringBuilder();
        public static Stopwatch sw = new Stopwatch();
        public static void ReadAll()
        {
            StringBuilder sb = new StringBuilder();
           
            
            
            //List<(int, string)> badData = new List<(int, string)>();
            Regex inneTemp = new Regex(@"^(?<year>201[6-7])-(?<month>0[1-9]|1[0-2])-(?<day>0[1-9]|[1-2]\d|3[01]) (?<hour>[0-1]\d|2[0-3]):(?<minute>[0-5]\d):(?<second>[0-5]\d),(?<location>Inne),(?<temp>[1-3]\d.\d),(?<rh>[1-8]\d)$",RegexOptions.Compiled);
            Regex uteTemp = new Regex(@"^(?<year>201[6-7])-(?<month>0[1-9]|1[0-2])-(?<day>0[1-9]|[1-2]\d|3[01]) (?<hour>[0-1]\d|2[0-3]):(?<minute>[0-5]\d):(?<second>[0-5]\d),(?<location>Ute),(?<temp>-?[0-3]?\d.\d),(?<rh>100|\d?\d)$",RegexOptions.Compiled);
            //int badDataCount = 0;
            //int badDataRow = 0;
            //string fullBadData;
            sw.Start();
            try
            {
                using (StreamReader reader = new StreamReader(path + "tempdata.txt"))
                {
                    string line = reader.ReadLine();
                    //int rowCount = 0;
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
                            if (data.DateTime.Year==2016 && data.DateTime.Month==5)
                            {
                                continue;
                            }
                            if(data.DateTime.Year ==2017 && data.DateTime.Month == 1)
                            {
                                continue;
                            }
                            Program.WeatherData.Add(data);
                        }
                        else
                        {
                            CollectedDataDisplay.BadData(line);
                            //badDataCount++;
                            //badDataRow = rowCount + badDataCount;
                            //fullBadData = $"{badDataRow,-10} | {line}";
                            //sbBadData.AppendLine(fullBadData);
                            //badData.Add((badDataRow,line));
                        }
                    }

                    //Ta bort förkorta månader, månader med mindre än 2 dagar registerade, justera efter beviljan
                    Program.WeatherData = Program.WeatherData.GroupBy //Gruppera efter år och månad
                        (   m=> new 
                            {
                                m.DateTime.Year, 
                                m.DateTime.Month,
                            }).
                        Where(g=>g.Select(m=>m.DateTime.Date). //Selecta 
                        Distinct(). //Unika datum, så vi inte räknar på 31 Maj 500 gånger
                        Count()>1). //Högre än 1 => Månader med mer än 1 dag registerard
                        SelectMany(r=>r).OrderBy(e=>e.DateTime).ToList(); //Weather data tar emot en ny kopia av listan
                    sw.Stop();
                    
                }
            }
            catch
            {
                Console.WriteLine("Filen finns inte");
            }

        }
    }
}
