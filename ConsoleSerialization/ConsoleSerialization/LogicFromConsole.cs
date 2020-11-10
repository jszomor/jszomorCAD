using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSerialization
{
  class LogicFromConsole
  {
    static void Main()
    {
      //Console.WriteLine("Enter flow:");
      //double flow = Convert.ToDouble(Console.ReadLine());

      Console.WriteLine("Enter number of tank:");
      double numberOfTanks = Convert.ToDouble(Console.ReadLine());

      Console.WriteLine("Enter length:");
      double length = Convert.ToDouble(Console.ReadLine());

      Console.WriteLine("Enter width:");
      double width = Convert.ToDouble(Console.ReadLine());

      Console.WriteLine("Enter wall thickness:");
      double wallThickness = Convert.ToDouble(Console.ReadLine());

      //Console.WriteLine("Enter adsorp volume:");
      //double volAdsorp = Convert.ToDouble(Console.ReadLine());

      //Console.WriteLine("Enter preselec volume:");
      //double volPreselec = Convert.ToDouble(Console.ReadLine());

      //Console.WriteLine("Enter length of decanter:");
      //double lengthOfDec = Convert.ToDouble(Console.ReadLine());

      var coordiates = new Coordiates();
      coordiates.MainZoneCoordinates(length, width, numberOfTanks, wallThickness, coordiates);
    }    
  }
}
