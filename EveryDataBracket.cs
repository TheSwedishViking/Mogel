using System;
using System.Collections.Generic;
using System.Text;

namespace VäderUppgift
{
    internal class EveryDataBracket
    {

        public DateOnly Datum  { get; set; }

        public TimeOnly Tid { get; set; }

        public bool IsInside { get; set; }

        public  float Temp { get; set; }


        public short AirHumidity { get; set; }


    }

}
