using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TempData_grupparbete.Models;
using System.Globalization;
using System.Diagnostics;
using System.IO;

namespace TempData_grupparbete.Data
{
    internal class DataExtraction
    {
        public static List<TempStatistics> AverageTempDay(List<WeatherData> data, string location)
        {

            return data
                .Where(d => d.Location == location)
                .GroupBy(d => d.DateTime.Date)
                .Select(group => new TempStatistics
                {
                    Date = group.Key,
                    Temp = group.Average(d => d.Temp),
                    Count = group.Count(),
                    Humidity = group.Average(h => h.Humidity)
                })
                .OrderBy(stats => stats.Date)
                .ToList();
                
            
        }
        public static List<TempStatistics> AverageTempMonth(List<WeatherData> data, string location)
        {

            return data
                .Where(d => d.Location == location)
                .GroupBy(d => new { d.DateTime.Year, d.DateTime.Month })
                .Select(group => new TempStatistics
                {
                    Date = new DateTime(group.Key.Year, group.Key.Month, 1),
                    Temp = group.Average(d => d.Temp),
                    Count = group.Count(),
                    Humidity = group.Average(h => h.Humidity)
                })
                .OrderBy(stats => stats.Date)
                .ToList();
                

        }
    }
}
