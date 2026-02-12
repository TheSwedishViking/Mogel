using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempData_grupparbete.Models
{
    internal class WeatherData
    {
        public DateTime DateTime { get; set; }
        public double Temp { get; set; }
        public string Location { get; set; }
        public int Humidity {  get; set; }
    }
}
