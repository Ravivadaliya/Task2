using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;   
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms;

namespace Task2
{
    public static class Logger
    {
        public static void Writelog(string message)
        {

            //string logpath = ConfigurationSettings.AppSettings["logPath"];

            //using (StreamWriter writer = new StreamWriter(logpath, true))
            //{
            //    writer.WriteLine($"{DateTime.Now} : " + message + "\n");
            //}


            string path = @"D:\Work\logfile.txt";
            
            using (StreamWriter writer = new StreamWriter(path))
             {
               writer.WriteLine($"{DateTime.Now} : " + message + "\n");
             }
        }
    }
}
