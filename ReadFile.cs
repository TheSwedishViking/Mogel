using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TempData_grupparbete
{
    internal class ReadFile
    {
        
        public static string path = "../../../File/";
        public static void ReadAll()
        {
            Regex regex = new Regex(@"^(?<year>[2][0][1][6]|[2][0][1][7])-(?<month>[0-1][0-9]|1[1-2])-(?<day>[0-2][0-9]|3[0-1]) (?<hour>[0-1][0-9]|2[0-3]):(?<minute>[0-5][0-9]):(?<second>[0-5][0-9]),(?<location>[I][n][n][e]),(?<temp>[1-2][0-9]\.[0-9]),(?<rh>1[0-9]|[2-3][0-9]|[4-5][0-9])$|^(?<year>[2][0][1][6]|[2][0][1][7])-(?<month>[0-1][0-9]|1[1-2])-(?<day>[0-2][0-9]|3[0-1]) (?<hour>[0-1][0-9]|2[0-3]):(?<minute>[0-5][0-9]):(?<second>[0-5][0-9]),(?<location>[U][t][e]),(?<temp>([-2][0-9]\.[0-9])|[0-9]\.[0-9]|[1-3][0-9]\.[0-9]),(?<rh>2[0-9]|[1-9][0-9])$");
            try
            {
                using (StreamReader reader = new StreamReader(path + "tempdata.txt"))
                {
                    string line = reader.ReadLine();
                    int rowCount = 0;
                    int validationErrorsCount = 0;
                    while ((line = reader.ReadLine()) != null)
                    {
                        Match match = regex.Match(line);
                        if (match.Success)
                        {
                            string time = match.Groups["year"].Value + "-" + match.Groups["month"].Value + "-" + match.Groups["day"].Value + " " + match.Groups["hour"].Value + ":" + match.Groups["minute"].Value + ":" + match.Groups["second"].Value + "," + match.Groups["location"].Value + "," + match.Groups["temp"].Value + "," + match.Groups["rh"].Value;
                            //if (match.Groups["location"].Value  == "Ute")
                            Console.WriteLine(time);
                            rowCount++;
                        }
                        else
                        {
                            Console.WriteLine("\nNågot udda med datan nedan");
                            Console.WriteLine(line + "<--------------\n");
                            validationErrorsCount++;
                            Console.ReadKey();
                        }
                    }
                    Console.WriteLine($"Antal rader korrekta enligt formatet {rowCount}, antal fel enligt formatet {validationErrorsCount}");
                }
            }
            catch
            {
                Console.WriteLine("Filen finns inte");
            }
        }
    }
}
