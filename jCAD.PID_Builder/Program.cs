using Autodesk.AutoCAD.DatabaseServices;
using JsonParse;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jCAD.PID_Builder
{
  public class Program
  {

    static void Main(string[] args)
    {
      // Specify the data source.
      int[] scores = new int[] { 97, 92, 81, 60 };

      // Define the query expression.
      IEnumerable<int> scoreQuery =
          from score in scores
          where score > 80
          select score;

      // Execute the query.
      foreach (int i in scoreQuery)
      {
        Console.Write(i + " ");
      }

      Console.ReadKey();
    }
  }
}
