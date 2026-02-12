using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Globalization;

namespace VäderUppgift
{
    internal class Temp
    {
        public static string path = "../../../File/";
        public static Dictionary<DateOnly, List<EveryDataBracket>> DataDictionary = new Dictionary<DateOnly, List<EveryDataBracket>>();
        public static void TempRun()
        {
            string url = @"B:\Downloads,Pictures,Videos\Downloads\tempdata5-med fel\tempdata5-med fel.txt";
            string oscarUrl = @"C:\Users\oxlyt\Desktop\tempdata5-med fel.txt";

            //-\d{2}- extra för månad
            Regex entireRegexPattern = new Regex(@"^(?<year>[2][0][1][6]|[2][0][1][7])-(?<month>0[1-9]|1[0-2])-(?<day>[0-2][0-9]|3[0-1]) (?<hour>[0-1][0-9]|2[0-3]):(?<minute>[0-5][0-9]):(?<second>[0-5][0-9]),(?<location>[I][n][n][e]),(?<temp>[1-2][0-9]\.[0-9]),(?<rh>1[0-9]|[2-3][0-9]|[4-5][0-9])$|^(?<year>[2][0][1][6]|[2][0][1][7])-(?<month>[0-1][0-9]|1[1-2])-(?<day>[0-2][0-9]|3[0-1]) (?<hour>[0-1][0-9]|2[0-3]):(?<minute>[0-5][0-9]):(?<second>[0-5][0-9]),(?<location>[U][t][e]),(?<temp>([-2][0-9]\.[0-9])|[0-9]\.[0-9]|[1-3][0-9]\.[0-9]),(?<rh>2[0-9]|[1-9][0-9])$");
            Regex entireNewPattern = new Regex(@"(?<year>2016|2017)-(?<month>0[1-9]|1[0-2])-(?<day>(0[1-9]|[1-2][0-9])|3[0-1]) (?<hour>[0-1]\d|2[0-3]):(?<minute>[0-5]\d):(?<second>[0-5]\d),(?<location>Inne),(?<temp>[1-3]\d.\d),(?<rh>[1-8]\d)|(?<year>2016|2017)-(?<month>0[1-9]|1[0-2])-(?<day>(0[1-9]|[1-2][0-9])|3[0-1]) (?<hour>[0-1]\d|2[0-3]):(?<minute>[0-5]\d):(?<second>[0-5]\d),(?<location>Ute),(?<temp>-?[0-3]?\d.\d),(?<rh>100|\d?\d)");
            string weatherData = File.ReadAllText(oscarUrl);
            List<string> weatherLines = weatherData.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            //Datum
            List<DateOnly> datumlist = new();
            Regex regexDatum = new Regex("\\d{4}-\\d{2}-\\d{2}");

            //ExaktTid
            List<TimeOnly> tidList = new();
            Regex regexTid = new Regex("\\d{2}:\\d{2}:\\d{2}");

            //regexPlats
            List<string> platsList = new();
            Regex regexPlats = new Regex("[A-Za-z]{3,4}");

            //Temp
            List<float> tempList = new();
            Regex regexTemp = new Regex("\\d{1,2}\\.\\d{0,1}");

            //Fukt
            List<short> luftfuktighetList = new();
            Regex regexFukt = new Regex("(?<=[0-9]{1,2}\\.[0-9]{1}.)([0-9]{1,2})");

            Regex entireRowFormatDay = new Regex("(\\d{4}-\\d{2}-\\d{2}\\s\\d{1,2}:\\d{1,2}:\\d{1,2},\\D{1,4}[0-9]{1,2}.[0-9]{1,2},\\d[0-9]{1,2})");
            List<string> weatherOutside = new List<string>();

        

            int countCorrect = 0;

            foreach (var weatline in weatherLines)
            {
                try
                {
                    //Passar det inte det exceptionellt långa formatet
                    //Lägger vi inte till det i listan
                    //Orelevant eller oformaterad data, skippa denna rad/post
                    if (!Regex.IsMatch(weatline, entireNewPattern.ToString()))
                    {
                        //Console.WriteLine($"Rad {weatline} passade inte in på formatet");
                        //Console.ReadKey();
                        //Lägg inte till, gå vidare i loopen
                        continue;
                    }

                    //Standard datum sträng därifrån
                    //Kollar första 4 siffrorna, tar emot 2016-2017 t.ex.
                    if (Regex.IsMatch(weatline, regexDatum.ToString()))
                    {
                        //Vi kunde läsa av det korrekt, bra, räkna upp antalet korrekta
                        countCorrect++;
                        //Vi registerara datumet som en variabel
                        var newDate = regexDatum.Match(weatline);
                        //Parse:a här, det kan krascha om februari har t.ex. 30 dagar
                        //Fast onormala interval som yyyy-14-42 kommer inte hit, det slår första regex mönstret ut
                        DateOnly onlyDate = DateOnly.Parse(newDate.Value);

                        //Har vi inte detta datum reda, aka det är en ny dags data vi börjar registerar
                        if (!DataDictionary.TryGetValue(onlyDate, out var testList))
                        {
                            //Öppna en lista av poster för denna NYA dag som vi inte hade förut. Detta gör så att varje ny dag får en lista, som vi kan fylla
                            //Detta gör så att vi sparar varje unikt datum, och inte varenda datum. Vi behöver bara registerear 05-31 en gång, inte 1000 gånger.
                            //Datum en gång, i datumet lagrar vi alla rader kopplade dit
                            testList = new List<EveryDataBracket>();
                            DataDictionary[onlyDate] = testList;
                        }
                        //Objektet som håller datan för varje dag
                        //Vi lägger temperatur, fukt, inomhus/utomhus och tid här
                        //Varje dag har AVG 2000 poster med denna, ungefär hälften inomhus och hutomhus
                        EveryDataBracket dataBracket = new EveryDataBracket();


                        //Vi kollar om tidsformatet är det normala HH:MM:SS
                        if (Regex.IsMatch(weatline, regexTid.ToString()))
                        {
                            //Sen gör vi en variabel av det från raden
                            var time = regexTid.Match(weatline);

                            //Och parse:ar det, som gör att vi kan spara tiden till data bracket
                            TimeOnly regTime = TimeOnly.Parse(time.Value);
                            dataBracket.Tid = regTime;
                        }
                        //Plats koll, mönstret kollar om det är 3-4 bokstäver i raden
                        if (Regex.IsMatch(weatline, regexPlats.ToString()))
                        {
                            //Variabel av bokstäverna i mönstret
                            var place = regexPlats.Match(weatline);

                            //Deklararera en bool som vi ändrar, sätter till false först, minimal skillnad om vi sätter till true, fast den behöver ett start värde
                            bool isInsideBool = false;

                            //Står det inomhus, blir isINsideBool = true
                            //Annars forstätter vi på att det är utomhus
                            if (place.Value.ToString() == "Inne")
                            {
                                isInsideBool = true;
                            }
                            //Sätter det på databracket
                            dataBracket.IsInside = isInsideBool;
                        }
                        //Temperatur värdet, regex kollar efter första biten av raden som har en punkt .
                        if (Regex.IsMatch(weatline, regexTemp.ToString()))
                        {
                            //Variabel
                            var tempValue = regexTemp.Match(weatline);

                            //Konvertera till en float, detta fall har vi decimal skilt med ett komma istället för punk, Sverige annars mer vanligt med punkt
                            //Kanske är jätte enkelt att byta det så det matchar svensk standard, fast jag vet inte rakt av hur man byter det /Oscar
                            if (double.TryParse(tempValue.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out double floatValue))
                            {
                                dataBracket.Temp = floatValue;
                            }
                        }
                        //Fukt registering
                        if (Regex.IsMatch(weatline, regexFukt.ToString()))
                        {
                            //Hitta fukt delen av datan som är i slutet av strängen
                            var humidity = regexFukt.Match(weatline);
                            //Parse som short, då fukt var mellan 0-100 rangen, vi behöver inte 2 miljarder ytrymmet som en int erbjuder
                            short humidityValue = short.Parse(humidity.ToString());
                            //Sätt det på bracket
                            dataBracket.AirHumidity = humidityValue;
                        }

                        //I detta datum => t.ex. 05-31, så lägger vi till denna bost i listan av dataposter. Alla poster för en dag, läggs till i listan, för dagen. Varje dag/datum är en key
                        //Key/Nyckel låser upp dörren som visar värden för dagen, utan nyckel kan vi inte komma in och se värderna
                        DataDictionary[onlyDate].Add(dataBracket);
                    }
                }
                //Standard something broke exception, gissar på att februari månad kan skapa möjliga problem
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            Console.WriteLine($"Registrerade {countCorrect} värden som borde vara korrekta enligt regex");
            Console.ReadKey();
            foreach (var day in DataDictionary.Keys)
            {
                Console.WriteLine($"Dag {day} hade: {DataDictionary[day].Count} registrerade poster (inkl tid, plats, temp, luftfuktighet) ");
                //varenda post i listan 
                foreach (var data in DataDictionary[day])
                {
                    //string placeString = data.IsInside ? "Inomhus" : "Utomhus";
                    //if (!data.IsInside)
                    //{
                    //    Console.WriteLine($"\tPlats: {placeString} Tid: {data.Tid}  Temperatur: {data.Temp} Luftfuktighet: {data.AirHumidity}");
                    //}
                }
                Console.WriteLine("=================");
                //poster på en dag
                int postsPerDay = DataDictionary[day].Count;
                double avgTempDay = DataDictionary[day].Sum(d => d.Temp);
                avgTempDay /= DataDictionary[day].Count();
                int countPostsInside = DataDictionary[day].Where(d=>d.IsInside).Count();
                int countPostsOutSide = DataDictionary[day].Where(d => !d.IsInside).Count();
                double avgTempInside = DataDictionary[day].Where(d => d.IsInside).Average(d=>d.Temp);

                double avgTempOutside = DataDictionary[day].Where(d => !d.IsInside).Average(d => d.Temp);

                short humidityInside = (short)DataDictionary[day].Where(d => d.IsInside).Average(d=>d.AirHumidity);

                short humidityOutisde = (short)DataDictionary[day].Where(d => !d.IsInside).Sum(d => d.AirHumidity);
                Console.WriteLine($"\nMedevärde temperatur inom och utomhus för {day} {avgTempDay.ToString("0.0")}°C, med {postsPerDay} poster den dagen");
                Console.WriteLine($"Inomhusvärdet temperatur var {avgTempInside.ToString("0.0")}°C baserat på {countPostsInside} poster");
                Console.WriteLine($"Luftfuktigheten inomhus var på {humidityInside.ToString("0")} %RH");
                Console.WriteLine("");
                Console.WriteLine($"Utomhusvärdet temperatur var {avgTempOutside.ToString("0.0")}°C baserat på {countPostsOutSide} poster");
                Console.WriteLine($"Luftfuktigheten utomhus var på {humidityOutisde.ToString("0")} %RH\n");
                Console.ReadKey();
            }
            //Hitta varmaste och kallaste medelvärden
            double highestAvgTempOutSide = 0;
            double highestAvgTempInside = 0;

            double coldestAvgTempOutisde = 100;
            double coldestAvgTempInside = 100;

            //Varmaste dagen (utan tid)
            DateOnly warmestOutside = new DateOnly();
            DateOnly warmestInside = new DateOnly();
            TimeSpan w = new TimeSpan();
            //Kallaste dagen
            DateOnly coldestInside = new DateOnly();
            DateOnly coldestOutside = new DateOnly();

            foreach( var day in DataDictionary.Keys)
            {
                //kolla att göra om till enklare linq uttryck utan foreach för att hitta högsta värde nånsin i alla keys
                double highestValue = DataDictionary[day].Max(d => d.Temp);
                double indoorPosts = DataDictionary[day].Where(s => s.IsInside).Count();
                double indoorC = DataDictionary[day].Where(s=>s.IsInside).Sum(s=>s.Temp);
                indoorC /= indoorPosts;
                w = new TimeSpan(2, 14, 18);
                int outDoorPosts = DataDictionary[day].Where(s => !s.IsInside).Count();
                double outDoorC = DataDictionary[day].Where(s => !s.IsInside).Sum(d => d.Temp);
                outDoorC /= outDoorPosts;

                if(indoorC> highestAvgTempInside)
                {

                    highestAvgTempInside = indoorC;
                    warmestInside = day;
                    //Console.WriteLine($"Ny högsta inomhus medeltemperatur på {indoorC}");
                    //Console.ReadKey();
                }
                if(outDoorC> highestAvgTempOutSide)
                {
                    highestAvgTempOutSide = outDoorC;
                    warmestOutside = day;
                    //Console.WriteLine($"Ny högsta utomhus medeltemperatur på {outDoorC}");
                    //Console.ReadKey();
                }
                if (outDoorC < coldestAvgTempOutisde)
                {
                    coldestAvgTempOutisde=outDoorC;
                    coldestOutside = day;
                    //Console.WriteLine($"Ny lägsta utomhus medeltemperatur på {outDoorC}");
                    //Console.ReadKey();
                }
                if (indoorC < coldestAvgTempInside)
                {
                    coldestAvgTempInside = indoorC;
                    coldestInside = day;
                    //Console.WriteLine($"Ny lägsta inomhus medeltemperatur på {indoorC}");
                    //Console.ReadKey();
                }


            }
            Console.WriteLine("==============================");
            Console.WriteLine($"Högsta medelinomhustemperatur för en dag var den {warmestInside} på {highestAvgTempInside.ToString("0.0")}°C, utomhus var den {warmestOutside} på {highestAvgTempOutSide.ToString("0.0")}°C");
            Console.WriteLine($"Lägsta medelinomhustemperatur för en dag var den {coldestInside} på {coldestAvgTempInside.ToString("0.0")}°C, utomhus var den {coldestOutside} på {coldestAvgTempOutisde.ToString("0.0")}°C");
            Console.ReadLine();
        }



    }
}
