using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempData_grupparbete.Models
{
    internal class TempStatistics
    {
        public DateTime Date { get; set; }
        public double Temp {  get; set; }
        public int Count { get; set; }
        public double Humidity { get; set; }
        public double Mold => GetMold();

        public double GetMold()
        {
            double rh = Humidity;
            double temp = Temp;

            if (rh <= 80 || temp <= 0 || temp >= 50)
            {
                return 0;
            }

            double criticalRh = 100 - temp;

            if (criticalRh < 80) criticalRh = 80;

            double range = 100 - 80;
            double currentAboveBase = rh - 80;

            double riskPercent = (currentAboveBase / range) * 100;

            return Math.Round(riskPercent, 1);
        }
    }
}
