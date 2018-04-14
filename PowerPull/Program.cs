using PowerPull.Scraping;
using System;

namespace PowerPull
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("PowerPull V2.0");

           
            while (true)
            {
                Console.WriteLine("Sistem seçiniz: \r\n" +
                    "1- Hypebeast \r\n" +
                    "2- VanityFair \r\n" +
                    "3- FashioneMagazine \r\n" +
                    "4- Elle \r\n" +
                    "5- Harpersbazaar \r\n" +
                    "6- Qz \r\n" +
                    "7- Haber7)");

                string selectedSystem = Console.ReadLine();
                if (!string.IsNullOrEmpty(selectedSystem) && Int32.Parse(selectedSystem) > 0)
                {
                    if (selectedSystem.Equals("1"))
                    {
                        Console.WriteLine("Hypebeast");
                        Hypebeast.Run();
                    }

                    if (selectedSystem.Equals("2"))
                    {
                        Console.WriteLine("VanityFair");
                        VanityFair.Run();
                    }

                    if (selectedSystem.Equals("3"))
                    {
                        Console.WriteLine("FashioneMagazine");
                        FashioneMagazine.Run();
                    }

                    if (selectedSystem.Equals("4"))
                    {
                        Console.WriteLine("Elle");
                        Elle.Run();
                    }

                    if (selectedSystem.Equals("5"))
                    {
                        Console.WriteLine("Harpersbazaar");
                        Harpersbazaar.Run();
                    }
                    if (selectedSystem.Equals("6"))
                    {
                        Console.WriteLine("Qz");
                        Qz.Run();
                    }
                    if (selectedSystem.Equals("7"))
                    {
                        Console.WriteLine("Haber7");
                        Haber7.Run();
                    }
                }
                else
                {
                    Console.WriteLine("Yanlış seçim yaptınız!");
                }
            }
        }
    }
}
