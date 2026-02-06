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
        public static void TempRun()
        {
            string url = @"B:\Downloads,Pictures,Videos\Downloads\tempdata5-med fel\tempdata5-med fel.txt";
            string oscarUrl = @"C:\Users\oxlyt\Desktop\tempdata5-med fel.txt";
            string htmlContent = File.ReadAllText(oscarUrl);

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
            Regex regexPlats = new Regex("[A-Za-z]{3,4}");

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
                    //Console.WriteLine(matches[i]);
                    try
                    {
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
                            //Console.WriteLine(matches[i].ToString());
                            //fel format
                            if (matches[i].ToString().StartsWith("24"))
                            {
                                //Console.WriteLine("XXXXXXXXXXXXXXXXXXXXXXX");

                                //Console.WriteLine($"Old time value which should be wrong: {matches[i].ToString()}");

                                var output = Regex.Replace(matches[i].ToString(), "^24", "00");

                                //Console.WriteLine($"Converted value from old: {output}");
                                //Console.ReadLine();
                                tidList.Add(TimeOnly.Parse(output));


                            }
                            //rätt format
                            else
                            {
                                tidList.Add(TimeOnly.Parse(matches[i].ToString()));
                            }

                            continue;
                        }  //TimeMatch
                        if (regexPlats.IsMatch(matches[i].ToString()))
                        {
                            //Console.WriteLine(matches[i].ToString());

                            platsList.Add(matches[i].ToString());
                            continue;
                        }//PlatsMatch
                        if (regexTemp.IsMatch(matches[i].Value))
                        {
                            //Console.WriteLine($"Input temp : {matches[i].Value}");
                            //Console.ReadLine();
                            string addedF = matches[i].Value.ToString() + "f";
                            //Console.WriteLine("Convert value = " + addedF + " " + addedF.ToString().ToString());
                            //Console.WriteLine();
                            float test = 24.5f;
                            //Console.WriteLine(matches[i].ToString());
                            if (float.TryParse(matches[i].ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
                            {
                                //Console.WriteLine("Matched format");
                                //Console.ReadLine();

                                //add to list
                                tempList.Add(value);
                            }
                            else
                            {
                                Console.WriteLine("Did not match format");
                                Console.ReadLine();
                            }

                            continue;
                        }
                        //TempMatch
                        Console.WriteLine("FUKT DELEN!");
                        Console.ReadLine();
                        if (regexFukt.IsMatch(matches[i].Value))
                        {
                            Console.WriteLine(matches[i].ToString());
                            short fuktValue = short.Parse(matches[i].Value);
                            tempList.Add(fuktValue);
                            continue;
                        }//Fukt
                        Console.WriteLine();
                    }
                    catch(ArgumentOutOfRangeException ex)
                    {

                        Console.WriteLine($"Maybe out of range? {ex.Message} í: {i}");
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine($"Excetoption :{e.Message}");
                    }
                    
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
