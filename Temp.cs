using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VäderUppgift
{
    internal class Temp
    {


        public static void TempRun()
        {
            string url = @"B:\Downloads,Pictures,Videos\Downloads\tempdata5-med fel\tempdata5-med fel.txt";

            string htmlContent = File.ReadAllText(url);

            List <Regex> patterinList = new();


            //\\S+   längre strängar
            //\d{4}-\d{2}-\d{2} datum
            //\d{2}:\d{2}:\d{2} tid
            //[A-Za-z] plats
            //\d{1,2}\.\d{0,1}  temperatur
            //[0-9]{1,2}$  luftfuktighet


            //-\d{2}- extra för månad




            //Datum
            List<DateOnly> datumlist = new();
            Regex regexDatum = new Regex("\\d{4}-\\d{2}-\\d{2}");

            //ExaktTid
            List<TimeOnly> tidList = new();
            Regex regexTid = new Regex("\\d{2}:\\d{2}:\\d{2}"); 

            //regexPlats
            List<string> platsList = new();
            Regex regexPlats = new Regex("[A-Za-z]");

            //Temp
            List<float> tempList = new();
            Regex regexTemp = new Regex("\\d{1,2}\\.\\d{0,1}");

            //Fukt
            List<short> luftfuktighetList = new();
            Regex regexFukt = new Regex("[0-9]{1,2}$");


            patterinList.AddRange(regexDatum, regexTid , regexPlats, regexTemp , regexFukt);

            





            //print
            foreach (var item in patterinList)
            {
                Regex regex = new Regex(item.ToString());
                MatchCollection matches = regex.Matches(htmlContent);

                for (int i = 0; i < 100000; i++)
                {
                    Console.WriteLine(matches[i]);
                    if (regexDatum.IsMatch(matches[i].ToString()))
                    {
                        //Console.WriteLine(item.ToString());
                        if (!datumlist.Contains(DateOnly.Parse(matches[i].ToString())))
                        {
                            datumlist.Add(DateOnly.Parse(matches[i].ToString()));
                        }
                        continue;
                    }        //DateMatch   
                    if (regexTid.IsMatch(matches[i].ToString()))
                    {
                        Console.WriteLine(matches[i].ToString());
                        if(matches[i].ToString().StartsWith("24"))
                        {
                            Console.WriteLine("XXXXXXXXXXXXXXXXXXXXXXX");
                            matches[i].ToString().Replace("24", "00");
                        }
                        tidList.Add(TimeOnly.Parse(matches[i].ToString()));
                        continue;
                    }  //TimeMatch
                    if (regexPlats.IsMatch(matches[i].ToString()))
                    {
                        Console.WriteLine(matches[i].ToString());

                        platsList.Add(matches[i].ToString());
                        continue;
                    }//PlatsMatch
                    if (regexTemp.IsMatch(matches[i].Value))
                    {
                        Console.WriteLine(matches[i].ToString());

                        float value = float.Parse(matches[i].Value);
                        tempList.Add(value);
                        continue;
                    }//TempMatch
                    if (regexFukt.IsMatch(matches[i].Value))
                    {
                        Console.WriteLine(matches[i].ToString());
                        short fuktValue = short.Parse(matches[i].Value);
                        tempList.Add(fuktValue);
                        continue;
                    }//Fukt
                    Console.WriteLine();
                }


                //for(int k = 0; k < datumlist.Count; k++)
                //{
                //    Console.WriteLine(datumlist[k].ToString() + " " + tidList[k].ToString() + " " + platsList[k].ToString() + " " +
                //        tidList[k].ToString() + " " + luftfuktighetList[k].ToString());
                //    Console.ReadLine();
                //}


                //foreach (var date in datumlist)
                //{
                //    Console.WriteLine(item);
                //}            //date writer
                //foreach (var time in tidList)
                //{
                //    Console.WriteLine(item);
                //}            //time writer

                



            }
      

         














        }

    }
}
