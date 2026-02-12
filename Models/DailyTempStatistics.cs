using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempData_grupparbete.Models
{
    internal class DailyTempStatistics
    {
        public DateTime Date { get; set; }
        public double Temp {  get; set; }
        public int Count { get; set; }
    }
}
