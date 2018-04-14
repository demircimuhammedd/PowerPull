using PowerPull.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace PowerPull.Helper
{
    public class FileHelper
    {
        public static void SaveFile(List<Content> listContent, string name)
        {
            Console.WriteLine("Data toplandı."); 
            var json = JsonConvert.SerializeObject(listContent);
            var filePath = string.Format("{0}\\Result_{1}.pop", Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), name);

            File.WriteAllText(filePath, json);
            Console.WriteLine("Dosya kaydedildi. Konum: {0}", filePath);
            Console.WriteLine("End: {0}", DateTime.Now);
            Console.WriteLine("Bitirmek için bir tuşa basınız.");
            Console.ReadKey();
            Console.Clear();
        }
    }
}
