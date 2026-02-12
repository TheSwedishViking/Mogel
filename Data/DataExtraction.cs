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
        public static async Task<List<DailyTempStatistics>> AverageTempDay(List<WeatherData> data, string location)
        {
            return await Task.Run(() => data
                .Where(d => d.Location == location)
                .GroupBy(d => d.DateTime.Date)
                .Select(group => new DailyTempStatistics
                {
                    Date = group.Key,
                    Temp = group.Average(d => d.Temp)
                })
                .OrderBy(stats => stats.Date)
                .ToList()
                );
        }
    }
}
