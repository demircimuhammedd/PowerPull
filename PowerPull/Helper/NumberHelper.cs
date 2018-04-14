
using System;

namespace PowerPull.Helper
{
    public class NumberHelper
    {
        public static int InputControl(string number)
        {
            while (string.IsNullOrEmpty(number) && Int32.Parse(number) < 1)
            {
                Console.WriteLine("1 sayfadan küçük olamaz ");
                number = Console.ReadLine();
            }
            return Int32.Parse(number);
        }
    }
}
