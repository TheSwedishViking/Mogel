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

            //Luftfuktighet behöver vara 80 eller över, temp över 0 och över 50 klarar inte mögel av, ingel mögel chans
            if (rh <= 80 || temp <= 0 || temp >= 50)
            {
                return 0;
            }

            //Kritisk gräns för luftfuktighet, gränsen blir högre desto svalare det är
            double criticalRh = 100 - temp;

            if (criticalRh < 80) criticalRh = 80; //Kritisk luftfuktighet är orelevant om den är under 80, den får som lägst vara 80

            double range = 100 - 80; //Mögel intervall av effekivt 20, mellan 80-100 luftfuktighet växer det
            double currentAboveBase = rh - 80;  //Överskott från bas är luftfuktighet - 80, ett litet tal som vi dividerar med spann senare

            //Risk procenten blir då effektivt Överskott/Mögelspann, där * 100 ger oss en %
            double riskPercent = (currentAboveBase / range) * 100;

            //16-11-16: Temp ute medel 6,5,  Luftfuktighet: 93,8 
            //Kritiskluftfuktighets nivå = 100-6.3 = 93.4
            //Överskott från bas = Luftfuktighet 93.8-80 = 13.8
            //Mögelrisk = (13.8/20) * 100 = 69% mögelrisk
            //T.ex. skulle 100% mögelrisk förekomma om det var runt 1-5 grader med 100 luftfuktighet

            return Math.Round(riskPercent, 1);
        }
    }
}
