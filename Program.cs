using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace LinearAlgebra
{
    class Program
    {
        public static void Main()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            try
            {

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}