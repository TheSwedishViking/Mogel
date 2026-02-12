using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempData_grupparbete.Services
{
    internal class Writer
    {
        public static string path = "../../../File/";
        public static async Task WriteRow(string fileName, string text)
        {
            using (StreamWriter streamWriter = new StreamWriter(path + fileName, true))
            {
                await streamWriter.WriteLineAsync(text);
            }
        }
        public static async Task Delete(string fileName)
        {
            using (StreamWriter streamWriter = new StreamWriter(path + fileName, false))
            {
                
            }
        }
    }
}
