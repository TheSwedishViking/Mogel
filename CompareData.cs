using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace VäderUppgift
{
    internal class CompareData
    {

        public static void Run()
        {  
            string urlData = @"C:\Users\robin\Documents\SystemutvecklingMapp\coolapp2\Februari-2026\VäderUppgift\M-gel\bin\Debug\net9.0\cleanedDataWithIdAndSeperator.txt";
            
            var dataLines = File.ReadAllLines(urlData).Select(line =>
            {
                var data = line.Split(';');

                return new EveryDataBracket
                {
                    Datum = DateOnly.Parse(data[1]),
                    Tid = TimeOnly.Parse(data[2]),
                    IsInside = data[3] == "Inne",
                    Temp = float.Parse(data[4], CultureInfo.InvariantCulture),
                    AirHumidity = short.Parse(data[5])
                };
            }).ToList();

            bool inputIsValid = false;
            string isInside = "";




            while (true)
            {
                List<EveryDataBracket> tempCompareeDateList = new();
                List<GenomsnittPerDag> dailyAVG = new();

                Console.WriteLine("Vilket datum vill du se? yyyy-mm-dd");
                if (!DateOnly.TryParse(Console.ReadLine(), out DateOnly searchDate))
                {
                    Console.WriteLine("Fel datumformat.");
                    return;
                }

                //inom/utomhusData
                var CorrectDateInside = dataLines.Where(l => l.Datum == searchDate).ToList();
                
               
                //AllData
                var CorrectDate = dataLines.Where(l => l.Datum  == searchDate).ToList();
                if(CorrectDate.Count == 0)
                {
                    Console.WriteLine("Inga data för det datumet.");
                    continue;
                }
                
                    foreach (var item in CorrectDate)
                    {
                        Console.WriteLine($"Datum: {item.Datum} Tid: {item.Tid} Inne/ute: " +
                        $"{(item.IsInside ? "Inne" : "Ute")} Temp: {item.Temp} Luftfuktighet: {item.AirHumidity}");
                    }


                Console.WriteLine("Vill du se högsta temperaturen: H? Eller lägsta: L , eller Genomsnitt: G  ");
                Console.WriteLine("Vill du se högsta Inom/utomhus temperaturen: Q? Eller lägsta: W , eller Genomsnitt: E  ");
                Console.WriteLine("Sök mellan tidsspann = T");


                switch (Console.ReadLine().ToUpper())
                {
                    case "H":
                        
                          var maxTemp =  CorrectDate.Max(t => t.Temp);
                            Console.WriteLine(maxTemp.ToString());                       
                        break; //Högsta 
                    case "L":
                        var lowestTemp = CorrectDate.Min(t => t.Temp);
                        Console.WriteLine(lowestTemp.ToString());

                        break;//Lägsta
                    case "G":
                        var avrageTemp = CorrectDate.Average(t => t.Temp);
                        Console.WriteLine(avrageTemp.ToString());

                        break;//Genomsnitt
                    case "T":



                        Console.WriteLine("Fram till vilket datum vill du se? yyyy-mm-dd");
                        if (!DateOnly.TryParse(Console.ReadLine(), out DateOnly fromXToY))
                        {
                            Console.WriteLine("Fel datumformat.");
                            return;
                        }
                        Console.WriteLine("All data? ( A ),     All uniqe day data? ( G),   Utomhusdata? ( U ) ,  Inomhusdata?  ( I )   ");
                        switch (Console.ReadLine().ToUpper())
                        {
                            case "A":
                                var betweenDates = dataLines.Where(d => d.Datum >= searchDate && d.Datum <= fromXToY).ToList();

                                foreach (var item in betweenDates)
                                {
                                    Console.WriteLine(item.Datum + "  temp = " + item.Temp);
                                    tempCompareeDateList.Add(item);
                                }
                                break;

                            case "U":
                                betweenDates = dataLines.Where(d => !d.IsInside && d.Datum >= searchDate && d.Datum <= fromXToY).ToList();
                                foreach (var item in betweenDates)
                                {
                                    Console.WriteLine(item.Datum + "  temp = " + item.Temp);
                                    tempCompareeDateList.Add(item);
                                }
                                break;
                            case "I":
                                betweenDates = dataLines.Where(d => d.IsInside && d.Datum >= searchDate && d.Datum <= fromXToY).ToList();
                                foreach (var item in betweenDates)
                                {
                                    Console.WriteLine(item.Datum + "  temp = " + item.Temp);
                                    tempCompareeDateList.Add(item);
                                }
                                break;
                            case "G":
                                    Console.WriteLine("Inside = inne     , Ute = ute");
                                    isInside = Console.ReadLine().ToLower();
                                
                                if (isInside == "inne")
                                {
                                    inputIsValid = true;

                                }
                                else
                                {
                                    inputIsValid = false;

                                }





                                var datesToCompare = dataLines.Where(
                                d=> d.IsInside == inputIsValid && 
                                d.Datum >= searchDate &&
                                d.Datum <= fromXToY)
                                    .ToList();

                                var averagePerDay = datesToCompare
                                    .GroupBy(d => d.Datum).Select(g => new GenomsnittPerDag
                                    {
                                          Datum = g.Key,AverageTemp = g.Average(x => x.Temp)
                                    }).ToList();


                                foreach (var item in averagePerDay.OrderBy(t => t.AverageTemp))
                                {
                                    Console.WriteLine(item.Datum + "  genomsnitt temp = " + item.AverageTemp);
                                    dailyAVG.Add(item);
                                }

                                break;
                        }
                            

                        Console.WriteLine("Vill du se högsta temperaturen: H? Eller lägsta: L , eller Genomsnitt: G   , sorterat efter temp X");

                        switch (Console.ReadLine().ToUpper())
                        {
                            case "H":

                                maxTemp = tempCompareeDateList.Max(t => t.Temp);
                                Console.WriteLine(maxTemp.ToString());


                                break;
                            case "L":
                                lowestTemp = tempCompareeDateList.Min(t => t.Temp);
                                Console.WriteLine(lowestTemp.ToString());

                                break;
                            case "G":
                                avrageTemp = tempCompareeDateList.Average(t => t.Temp);
                                Console.WriteLine(avrageTemp.ToString());

                                break;
                            case "X":



                                var OrderTemp = dailyAVG;
                                foreach (var item in OrderTemp.OrderBy(t => t.AverageTemp ))
                                {
                                    Console.WriteLine(item.Datum + " + temp: "+item.AverageTemp);
                                }

                                break;

                        }
                        break; //Date -> Date   /WITH InsideOutside

                    case "Q":
                        Console.WriteLine("Inside = inne     , Ute = ute");
                        string insideOutside = Console.ReadLine().ToLower();
                        bool insideStatus;
                        if (insideOutside == "inne")
                        {
                            insideStatus = true;

                        }
                        else
                        {
                            insideStatus = false;

                        }
                        maxTemp = CorrectDateInside.Where(d => d.IsInside == insideStatus).Max(t => t.Temp);
                        Console.WriteLine(maxTemp.ToString() + " Är högsta temperaturen " + insideOutside);
                        break;//InomUtom Högsta
                    case "W":
                        Console.WriteLine("Inside = inne     , Ute = ute");
                        insideOutside = Console.ReadLine().ToLower();
                        if (insideOutside == "inne")
                        {
                            insideStatus = true;

                        }
                        else
                        {
                            insideStatus = false;

                        }
                        lowestTemp = CorrectDateInside.Where(d => d.IsInside == insideStatus).Max(t => t.Temp);
                        Console.WriteLine(lowestTemp.ToString() + " Är lägsta temperaturen " + insideOutside);
                        break;////InomUtom Lägsta
                    case "E":
                        Console.WriteLine("Inside = inne     , Ute = ute");
                        insideOutside = Console.ReadLine().ToLower();    
                        if (insideOutside == "inne")
                        {
                            insideStatus = true;

                        }
                        else
                        {
                            insideStatus = false;

                        }
                        avrageTemp = CorrectDateInside.Where(d => d.IsInside == insideStatus).Average(t => t.Temp);
                        Console.WriteLine(avrageTemp.ToString() + " Är den genomsnittliga temperaturen " + insideOutside);
                        break;//InomUtom Genomsnitt
                    default:
                        Console.WriteLine("Ogiltigt val.");
                        break;
                        Console.ReadKey(true);
                }
            }





            //Fukt frågor   +  fuktighet per dag






        }











    }
}
