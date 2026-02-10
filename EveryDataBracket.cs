using System;
using System.Collections.Generic;
using System.Text;

namespace VäderUppgift
{
    internal class EveryDataBracket
    {
        //Visar tiden
        public TimeOnly Tid { get; set; }

        //Inomhus? Sant, annars falskt
        public bool IsInside { get; set; }

        //Temperatur som kan vara -39.9 - 39.9
        //Beror på inomhus eller utomhus
        public  float Temp { get; set; }

        //0-100 generellt, 10-80 inne
        public short AirHumidity { get; set; }

    }
}
